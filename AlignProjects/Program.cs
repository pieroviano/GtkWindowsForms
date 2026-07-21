using Align.Common;
using Prepare.Common.Alignment;
using Prepare.Common.IO;
using Prepare.Common.Languages;
using Prepare.Common.Logging;
using PrepareProjects;

namespace AlignProjects;

/// <summary>
/// Post-pipeline tool: forward-ports the latest gitee upstream onto the generated
/// <c>Net4x.*</c> library projects via a transformed-space delta merge
/// (<c>f(base)</c> vs <c>f(target)</c>, applied onto the live projects).
/// See <c>AlignProjects-Plan.md</c>.
///
/// The alignment machinery itself lives in <see cref="AlignmentDriver"/> (shared with AlignSamples);
/// this entry point only supplies the library-specific configuration: the scope, the transform
/// <c>f</c>, the translation enrichment/pause and the provenance fallback.
/// </summary>
public static class Program
{
    /// <summary>Repo-relative directories this tool aligns.</summary>
    private static readonly string[] Scope = { "Net4x.Gtk.Windows.Forms", "Net4x.Gtk.Drawing" };

    private const string SubmoduleName = "gtksystem-windows-forms";

    public static async Task<int> Main()
    {
        // Resolve paths against the exe location (repo root is 4 levels up), so the tool works
        // regardless of the current working directory.
        var repoRoot = Path.GetFullPath(Environment.GetEnvironmentVariable("ALIGN_REPO_ROOT")
                                        ?? Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
        var resourcesDir = Path.Combine(repoRoot, "Resources");
        var translationsXml = Path.Combine(resourcesDir, "Translations.xml");
        var checkpointFile = Path.Combine(repoRoot, "AlignProjects", "last-aligned.txt");

        // --- Configuration (env-var overridable) ---
        // Base = the last commit already absorbed (checkpoint), else the bootstrap generation base.
        // Each run aligns base..target incrementally (see AlignProjects-Plan.md, Incremental & repeatable).
        var baseCommit = Environment.GetEnvironmentVariable("ALIGN_BASE_COMMIT")
                         ?? AlignmentCheckpoint.Read(checkpointFile, SourceRepositorySync.GenerationBaseCommit);
        // Target = the gitee-merged upstream (PrepareProjects leaves the submodule on this branch).
        var targetRef = Environment.GetEnvironmentVariable("ALIGN_TARGET_COMMIT")
                        ?? SourceRepositorySync.DefaultTrackingBranch;
        // The target (gitee-merged) upstream source to scan for Chinese - whole submodule (Source + Samples).
        var sourceDir = Environment.GetEnvironmentVariable("ALIGN_SOURCE_DIR")
                        ?? Path.Combine(repoRoot, SubmoduleName);

        // Resolve the ref to a real commit: a submodule on a detached HEAD frequently has no local branch
        // of that name (only origin/<name>), and the checkpoint must record the commit absorbed, not a
        // branch that keeps moving.
        var targetCommit = GitRepository.ResolveCommit(sourceDir, targetRef);
        if (targetCommit == null)
        {
            RedConsole.Write($"Cannot resolve target '{targetRef}' in {sourceDir} - is the submodule initialized and fetched?");
            return 5;
        }

        Console.WriteLine("AlignProjects - align Net4x.* with the latest gitee upstream");
        Console.WriteLine($"  base commit   : {baseCommit}");
        Console.WriteLine($"  target commit : {targetCommit} ({targetRef})");
        Console.WriteLine($"  source dir    : {sourceDir}");

        var driver = new AlignmentDriver(BuildOptions(repoRoot, checkpointFile, sourceDir));

        // AlignProjects-Plan.md §3.1 determinism self-check. The delta is only meaningful if f is
        // reproducible: if f(X) differs between two runs, `f(theirs) - f(base)` carries transform noise
        // that is indistinguishable from real upstream change. Run with ALIGN_SELFCHECK=1 (or a commit)
        // to transform ONE commit twice and compare byte for byte.
        var selfCheck = Environment.GetEnvironmentVariable("ALIGN_SELFCHECK");
        if (!string.IsNullOrEmpty(selfCheck))
        {
            return await driver.Producer.RunDeterminismCheckAsync(selfCheck == "1" ? targetCommit : selfCheck);
        }

        // Phase 2 - ensure new Chinese from the target snapshot is captured as BLANK entries so it
        //           can be filled once and cached. Additive merge: existing entries are preserved;
        //           only new Chinese is appended. Resource literals -> ResX locales synced to zh-Hans.
        Console.WriteLine("Enriching Translations.xml with new Chinese (blank English)...");
        var dictAdded = TranslationEnrichment.EnrichTranslationsDictionary(translationsXml, new[] { sourceDir });
        Console.WriteLine($"Translations.xml blank entries added: {dictAdded}");
        var resxAdded = TranslationEnrichment.EnrichResxLocales(resourcesDir);
        Console.WriteLine($"Resx locale blank entries added: {resxAdded}");

        // Pause for the one-time manual translation of the newly-added blank entries before the
        // transform/diff/apply that depends on them being translated.
        ManualTranslationPause.Run(dictAdded, resxAdded, translationsXml, resourcesDir);

        // --- Phase 3: the alignment driver (see AlignProjects-Plan.md §3) ---
        // ALIGN_BASE_TREE/ALIGN_THEIRS_TREE feed pre-produced trees in (test seam); otherwise the driver
        // runs f twice in isolated staging roots.
        return await driver.RunAsync(baseCommit, targetCommit,
            Environment.GetEnvironmentVariable("ALIGN_BASE_TREE"),
            Environment.GetEnvironmentVariable("ALIGN_THEIRS_TREE"),
            Environment.GetEnvironmentVariable("ALIGN_PROVENANCE"));
    }

    /// <summary>
    /// The library-specific half of the alignment: what to merge, how to run <c>f</c>, and how to map a
    /// generated file back to its raw upstream source when the transform emits no provenance manifest.
    /// </summary>
    private static AlignmentOptions BuildOptions(string repoRoot, string checkpointFile, string sourceDir) => new()
    {
        RepoRoot = repoRoot,
        ToolName = "AlignProjects",
        Scope = Scope,
        CheckpointFile = checkpointFile,
        SubmoduleName = SubmoduleName,
        BranchPrefix = "align",
        Transform = RunTransformIn,
        // Derived from the *.Files.xml manifests the transform already writes, resolved against the LIVE
        // submodule - which sits at the target commit, so the paths are real files the operator can open
        // (the staging worktrees are gone by then).
        ProvenanceFallback = theirsTree => FilesXmlProvenance.Load(
            Path.Combine(theirsTree, TransformedTreeProducer.ManifestsFolder),
            Path.Combine(sourceDir, "Source", "GTKSystem.Windows.Forms"))
    };

    /// <summary>
    /// Runs the shared transform <c>f</c> against a staging root. <c>RunTransform</c> reads and writes
    /// through <see cref="RepoPaths.Root"/>, so the root is swapped for the duration and restored after -
    /// that is what keeps the transform off the live repo.
    /// </summary>
    private static async Task RunTransformIn(string stagingRoot)
    {
        var previousRoot = RepoPaths.Root;
        try
        {
            RepoPaths.Root = stagingRoot;
            await TransformPipeline.RunTransform();
        }
        finally
        {
            RepoPaths.Root = previousRoot;
        }
    }
}
