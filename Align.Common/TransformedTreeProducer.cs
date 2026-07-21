using Prepare.Common.Alignment;
using Prepare.Common.Logging;

namespace Align.Common;

/// <summary>
/// Produces the transformed <c>BASE</c>/<c>THEIRS</c> trees the delta is computed from: checks the
/// upstream submodule out at a commit into an isolated staging root, runs the tool's transform there,
/// and captures the resulting scope directories (plus that run's <c>*.Files.xml</c> manifests).
/// Extracted from AlignProjects so AlignSamples runs the same machinery with <c>fs</c>.
/// </summary>
public sealed class TransformedTreeProducer
{
    /// <summary>Folder inside each produced tree holding that run's <c>*.Files.xml</c> provenance manifests.</summary>
    public const string ManifestsFolder = "_manifests";

    /// <summary>The transform-emitted generated→source manifest, carried from the staging root into the tree.</summary>
    public const string ProvenanceFileName = "provenance.tsv";

    private readonly AlignmentOptions _options;

    public TransformedTreeProducer(AlignmentOptions options) => _options = options;

    /// <summary>
    /// Produces both transformed trees by running the transform against the base and target commits in
    /// isolated staging roots (AlignProjects-Plan.md §3.2). The staging roots live under
    /// <paramref name="stagingParent"/> (which MUST be outside the repo — see the cycle note in
    /// <see cref="MirrorPlan"/>); only the resulting trees land under <paramref name="buildDir"/>.
    /// </summary>
    public async Task<(string baseTree, string theirsTree)> ProduceTransformedTreesAsync(
        string baseCommit, string targetCommit, string buildDir, string stagingParent)
    {
        var baseTree = await ProduceOneAsync(baseCommit,
            Path.Combine(stagingParent, "base-root"), Path.Combine(buildDir, "base"));
        var theirsTree = await ProduceOneAsync(targetCommit,
            Path.Combine(stagingParent, "theirs-root"), Path.Combine(buildDir, "theirs"));
        return (baseTree, theirsTree);
    }

    /// <summary>Transforms one commit into <paramref name="outTreeDir"/>; returns that directory.</summary>
    public async Task<string> ProduceOneAsync(string commit, string stagingRootPath, string outTreeDir)
    {
        var (junctions, files, copiedDirs) = MirrorPlan();
        var outputs = _options.Scope;

        using (var staging = StagingRoot.Create(stagingRootPath,
                   Path.Combine(_options.RepoRoot, _options.SubmoduleName),
                   _options.SubmoduleName, commit, junctions, files, outputs.ToArray(), copiedDirs))
        {
            await _options.Transform(staging.RootPath);

            if (Directory.Exists(outTreeDir))
            {
                Directory.Delete(outTreeDir, true);
            }

            foreach (var output in outputs)
            {
                CopyDir(Path.Combine(staging.RootPath, output), Path.Combine(outTreeDir, output));
            }

            // The transform's own provenance manifest, when it emits one (fs does): it is written at the
            // staging root, which is deleted on Dispose, so it must travel with the captured tree - that
            // is where AlignmentDriver.RunAsync looks for it.
            var stagedProvenance = Path.Combine(staging.RootPath, ProvenanceFileName);
            if (File.Exists(stagedProvenance))
            {
                File.Copy(stagedProvenance, Path.Combine(outTreeDir, ProvenanceFileName), true);
            }

            // Keep this run's *.Files.xml (source→destination per generated file) alongside the tree:
            // the staging root - and its Resources copy - is deleted on Dispose, but the conflict pause
            // needs the mapping afterwards. See FilesXmlProvenance.
            var stagedResources = Path.Combine(staging.RootPath, "Resources");
            var manifestsDir = Path.Combine(outTreeDir, ManifestsFolder);
            if (Directory.Exists(stagedResources))
            {
                Directory.CreateDirectory(manifestsDir);
                foreach (var manifest in Directory.GetFiles(stagedResources, "*Files.xml"))
                {
                    File.Copy(manifest, Path.Combine(manifestsDir, Path.GetFileName(manifest)), true);
                }
            }
        }

        return outTreeDir;
    }

    /// <summary>
    /// §3.1 determinism self-check: transforms ONE commit twice, into two independent staging roots,
    /// and compares the outputs byte for byte. Any difference is transform non-determinism — it would
    /// otherwise enter the alignment delta disguised as an upstream change. Writes to a dedicated
    /// build-selfcheck directory so a pending alignment's trees are left alone.
    /// Returns 0 when identical, 4 when not.
    /// </summary>
    public async Task<int> RunDeterminismCheckAsync(string commit)
    {
        var buildDir = _options.SelfCheckDirectory;
        if (Directory.Exists(buildDir))
        {
            Directory.Delete(buildDir, true);
        }

        var stagingParent = Path.Combine(Path.GetTempPath(),
            _options.ToolName.ToLowerInvariant() + "-selfcheck-" + Guid.NewGuid().ToString("N"));
        ConsoleColor.Green.Write($"Determinism self-check: transforming {commit} TWICE and comparing...", true);

        string runA, runB;
        try
        {
            ConsoleColor.Green.Write("  run 1/2...", true);
            runA = await ProduceOneAsync(commit, Path.Combine(stagingParent, "run1-root"), Path.Combine(buildDir, "run1"));
            ConsoleColor.Green.Write("  run 2/2...", true);
            runB = await ProduceOneAsync(commit, Path.Combine(stagingParent, "run2-root"), Path.Combine(buildDir, "run2"));
        }
        finally
        {
            try { Directory.Delete(stagingParent, true); } catch { /* best-effort */ }
        }

        var filesA = RelativeFiles(runA);
        var filesB = RelativeFiles(runB);

        var onlyA = filesA.Except(filesB, StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToList();
        var onlyB = filesB.Except(filesA, StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToList();
        var common = filesA.Intersect(filesB, StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToList();

        var namespaceStyle = new List<string>();
        var otherDiff = new List<string>();
        foreach (var rel in common)
        {
            var a = Path.Combine(runA, rel);
            var b = Path.Combine(runB, rel);
            if (FilesEqual(a, b))
            {
                continue;
            }

            (NamespaceStyleOf(a) != NamespaceStyleOf(b) ? namespaceStyle : otherDiff).Add(rel);
        }

        Console.WriteLine();
        ConsoleColor.White.Write($"Compared {common.Count} file(s) produced by both runs.", true);
        Console.WriteLine($"  identical                        : {common.Count - namespaceStyle.Count - otherDiff.Count}");
        Console.WriteLine($"  differ (namespace style flipped) : {namespaceStyle.Count}");
        Console.WriteLine($"  differ (other)                   : {otherDiff.Count}");
        Console.WriteLine($"  only in run 1                    : {onlyA.Count}");
        Console.WriteLine($"  only in run 2                    : {onlyB.Count}");

        var clean = namespaceStyle.Count == 0 && otherDiff.Count == 0 && onlyA.Count == 0 && onlyB.Count == 0;
        if (clean)
        {
            ConsoleColor.Green.Write("DETERMINISTIC: the two runs are byte-identical.", true);
            return 0;
        }

        RedConsole.Write("NON-DETERMINISTIC: the transform does not reproduce. Alignment deltas are unreliable.");
        Console.WriteLine($"Trees kept for inspection: {buildDir}");
        foreach (var rel in namespaceStyle.Take(10))
        {
            Console.WriteLine($"  [namespace] {rel}  ({NamespaceStyleOf(Path.Combine(runA, rel))} -> {NamespaceStyleOf(Path.Combine(runB, rel))})");
        }

        foreach (var rel in otherDiff.Take(10))
        {
            Console.WriteLine($"  [other]     {rel}");
            Console.WriteLine(FirstDifference(Path.Combine(runA, rel), Path.Combine(runB, rel)));
        }

        foreach (var rel in onlyA.Take(5))
        {
            Console.WriteLine($"  [only run1] {rel}");
        }

        foreach (var rel in onlyB.Take(5))
        {
            Console.WriteLine($"  [only run2] {rel}");
        }

        return 4;
    }

    /// <summary>
    /// Staging-root mirror plan: junction every top-level repo directory (except .git/.vs, the source
    /// worktree, this tool's own directory, and the scope dirs) and copy every top-level file, so the
    /// transform sees the full repo layout (props, Nuget.Config, snk, sibling projects) but writes in
    /// isolation.
    /// </summary>
    public (Dictionary<string, string> junctions, Dictionary<string, string> files,
        Dictionary<string, string> copiedDirs) MirrorPlan()
    {
        var repoRoot = _options.RepoRoot;
        var junctions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 'Resources' is COPIED, not junctioned: the transform writes the *.Files.xml source→destination
        // manifests there (ProjectUtility.PersistProjectFiles → <repoRoot>/Resources/), and a junction
        // would send those writes straight through into the live repo - clobbering the committed
        // manifests twice per run and breaking the isolation the staging root exists to provide.
        var copiedDirs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var resourcesDir = Path.Combine(repoRoot, "Resources");
        if (Directory.Exists(resourcesDir))
        {
            copiedDirs["Resources"] = resourcesDir;
        }

        // The tool's own directory is never mirrored: the transform has no use for it, and mirroring it
        // would expose build/{base,theirs} - the previous run's captured trees - to the transform's
        // whole-root walks (post-processing would rewrite files inside them), as well as re-introducing
        // a directory cycle if a staging root ever lands under the repo again.
        var skip = new HashSet<string>(
            new[] { ".git", ".vs", _options.SubmoduleName, "Resources", Path.GetFileName(_options.ToolDirectory) },
            StringComparer.OrdinalIgnoreCase);
        foreach (var scoped in _options.Scope)
        {
            skip.Add(scoped);
        }

        foreach (var dir in Directory.GetDirectories(repoRoot))
        {
            var name = Path.GetFileName(dir);
            if (skip.Contains(name))
            {
                continue;
            }

            var target = dir;
            try
            {
                var resolved = new DirectoryInfo(dir).ResolveLinkTarget(true);
                if (resolved != null)
                {
                    target = resolved.FullName;
                }
            }
            catch
            {
                // not a link
            }

            junctions[name] = target;
        }

        foreach (var file in Directory.GetFiles(repoRoot))
        {
            files[Path.GetFileName(file)] = file;
        }

        return (junctions, files, copiedDirs);
    }

    /// <summary>
    /// Relative paths of every produced SOURCE file: excludes the captured provenance manifests and all
    /// build intermediates. <c>obj/</c> holds timestamped, machine-specific MSBuild output
    /// (<c>project.assets.json</c>, <c>*.assets.cache</c>, …) which never reproduces and is not part of
    /// the transform's result.
    /// </summary>
    private static List<string> RelativeFiles(string root) =>
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => f.Substring(root.Length).TrimStart('\\', '/').Replace('\\', '/'))
            .Where(rel => !rel.StartsWith(ManifestsFolder + "/", StringComparison.OrdinalIgnoreCase)
                          && !IsBuildIntermediate(rel))
            .ToList();

    /// <summary>True for MSBuild output paths (<c>obj/</c>, <c>bin/</c>) at any depth.</summary>
    public static bool IsBuildIntermediate(string relativePath)
    {
        var p = "/" + relativePath.Replace('\\', '/') + "/";
        return p.Contains("/obj/", StringComparison.OrdinalIgnoreCase)
               || p.Contains("/bin/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool FilesEqual(string a, string b)
    {
        var fa = new FileInfo(a);
        var fb = new FileInfo(b);
        if (fa.Length != fb.Length)
        {
            return false;
        }

        return File.ReadAllBytes(a).AsSpan().SequenceEqual(File.ReadAllBytes(b));
    }

    /// <summary>"file-scoped", "block-scoped" or "none" — the namespace form a generated file uses.</summary>
    private static string NamespaceStyleOf(string path)
    {
        foreach (var line in File.ReadLines(path))
        {
            var t = line.TrimStart();
            if (!t.StartsWith("namespace ", StringComparison.Ordinal))
            {
                continue;
            }

            return t.TrimEnd().EndsWith(";", StringComparison.Ordinal) ? "file-scoped" : "block-scoped";
        }

        return "none";
    }

    /// <summary>The first differing line pair, for a compact non-determinism report.</summary>
    private static string FirstDifference(string a, string b)
    {
        var la = File.ReadAllLines(a);
        var lb = File.ReadAllLines(b);
        for (var i = 0; i < Math.Max(la.Length, lb.Length); i++)
        {
            var x = i < la.Length ? la[i] : "<eof>";
            var y = i < lb.Length ? lb[i] : "<eof>";
            if (!string.Equals(x, y, StringComparison.Ordinal))
            {
                return $"                line {i + 1}:{Environment.NewLine}" +
                       $"                  run1: {x.Trim()}{Environment.NewLine}" +
                       $"                  run2: {y.Trim()}";
            }
        }

        return "                (identical text; differs in bytes only - encoding or line endings)";
    }

    /// <summary>Copies a produced scope directory into the captured tree, skipping build intermediates.</summary>
    private static void CopyDir(string source, string dest)
    {
        var src = Path.GetFullPath(source);
        if (!Directory.Exists(src))
        {
            return;
        }

        foreach (var file in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
        {
            var relative = file.Substring(src.Length).TrimStart('\\', '/');

            // Build intermediates are not transform output: they never reproduce between runs and would
            // enter the alignment delta as pure noise.
            if (IsBuildIntermediate(relative))
            {
                continue;
            }

            var target = Path.Combine(dest, relative);
            var dir = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(file, target, true);
        }
    }
}
