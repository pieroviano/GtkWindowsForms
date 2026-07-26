using Prepare.Common.Alignment;
using Prepare.Common.Logging;

namespace Align.Common;

/// <summary>
/// The alignment driver shared by AlignProjects (Gtk.* libraries) and AlignSamples (the sample apps):
/// produce the transformed <c>BASE</c>/<c>THEIRS</c> trees (§3.1/§3.2) → structural preflight (§3.4) →
/// 3-way delta merge (§3.5) → conflict pause (§3.6) → report + residual-Chinese scan (§3.7) → advance the
/// checkpoint (§3.8). Everything tool-specific lives in <see cref="AlignmentOptions"/>; nothing here
/// knows which transform ran or which directories are being aligned.
/// </summary>
public sealed class AlignmentDriver
{
    private readonly AlignmentOptions _options;
    private readonly TransformedTreeProducer _producer;

    public AlignmentDriver(AlignmentOptions options)
    {
        _options = options;
        _producer = new TransformedTreeProducer(options);
    }

    /// <summary>The tree producer, exposed for the determinism self-check.</summary>
    public TransformedTreeProducer Producer => _producer;

    /// <summary>
    /// Produces the trees (unless pre-produced ones are supplied as a test seam), aligns, and returns a
    /// process exit code: 0 clean · 1 clean but residual Chinese / post-merge check failures ·
    /// 3 conflicts left unresolved.
    /// </summary>
    /// <param name="baseCommit">Upstream commit already absorbed (the checkpoint).</param>
    /// <param name="targetCommit">Upstream commit to align to; also the new checkpoint value on success.</param>
    /// <param name="baseTreeOverride">Pre-produced BASE tree (skips the transform); test seam.</param>
    /// <param name="theirsTreeOverride">Pre-produced THEIRS tree (skips the transform); test seam.</param>
    /// <param name="provenancePathOverride">Explicit <c>provenance.tsv</c> path; defaults to the THEIRS tree's.</param>
    public async Task<int> RunAsync(string baseCommit, string targetCommit,
        string? baseTreeOverride = null, string? theirsTreeOverride = null, string? provenancePathOverride = null)
    {
        string baseTree;
        string theirsTree;
        string? buildDir = null;

        if (!string.IsNullOrEmpty(baseTreeOverride) && !string.IsNullOrEmpty(theirsTreeOverride))
        {
            baseTree = baseTreeOverride!;
            theirsTree = theirsTreeOverride!;
        }
        else
        {
            // Output trees: a stable, gitignored location inside the repo (not %TEMP%), because the
            // conflict pause hands these paths to the operator as the diffed pair - they must be short,
            // predictable, and still there after the run ends. Cleared on entry; removed on exit only
            // when nothing is left to resolve (see below).
            buildDir = _options.BuildDirectory;
            if (Directory.Exists(buildDir))
            {
                Directory.Delete(buildDir, true);
            }

            // Staging roots: OUTSIDE the repo. MirrorPlan junctions the repo's top-level directories into
            // each staging root, so a staging root placed under the repo becomes reachable through its own
            // mirror (<staging>/Tool -> repo/Tool -> build/base-root -> <staging> -> ...) and any
            // SearchOption.AllDirectories walk recurses until the path length blows up. They are transient
            // (disposed after each transform), so nothing is lost by keeping them in %TEMP%.
            var stagingParent = Path.Combine(Path.GetTempPath(),
                _options.ToolName.ToLowerInvariant() + "-staging-" + Guid.NewGuid().ToString("N"));

            ConsoleColor.Green.Write("Producing transformed BASE and THEIRS trees (runs the transform twice)...", true);
            try
            {
                (baseTree, theirsTree) = await _producer.ProduceTransformedTreesAsync(
                    baseCommit, targetCommit, buildDir, stagingParent);
            }
            finally
            {
                try { Directory.Delete(stagingParent, true); } catch { /* best-effort */ }
            }
        }

        // Provenance (generated -> raw source) for the conflict pause. Prefer the purpose-built manifest
        // when the transform emits one (§3.1); otherwise fall back to the tool-specific derivation.
        var provenance = RebaseOntoRepo(ProvenanceManifest.Load(
            provenancePathOverride ?? Path.Combine(theirsTree, TransformedTreeProducer.ProvenanceFileName)));
        if (provenance.Count == 0 && _options.ProvenanceFallback != null)
        {
            provenance = _options.ProvenanceFallback(theirsTree);
            Console.WriteLine($"Provenance from *.Files.xml: {provenance.Count} generated file(s) mapped to upstream source.");
        }

        var result = AlignFromTrees(baseTree, theirsTree, provenance, targetCommit);

        // Keep the transformed trees whenever conflicts are still unresolved (exit >= 2): they are the
        // pair the delta was computed from, and the pause just told the operator to diff them by path.
        // Deleting here would hand out paths that no longer exist.
        if (buildDir != null)
        {
            if (result >= 2)
            {
                ConsoleColor.Yellow.Write(
                    $"Transformed BASE/THEIRS retained for manual resolution: {buildDir}", true);
                Console.WriteLine("  (removed automatically on the next successful alignment)");
            }
            else
            {
                try { Directory.Delete(buildDir, true); } catch { /* best-effort */ }
            }
        }

        return result;
    }

    /// <summary>
    /// Commits the advanced checkpoint (that file only). Skipped when the checkpoint lives outside the
    /// repository - the case in tests, and for a caller keeping its checkpoint elsewhere.
    /// </summary>
    private void CommitCheckpoint(string targetCommit)
    {
        var relative = GitRepository.RelativeTo(_options.RepoRoot, _options.CheckpointFile);
        if (relative == null)
        {
            return;
        }

        if (GitRepository.CommitPaths(_options.RepoRoot,
                $"{_options.ToolName}: advance checkpoint to {Shorten(targetCommit)}", new[] { relative }))
        {
            ConsoleColor.Green.Write($"Committed {relative}.", true);
        }
    }

    private static string Shorten(string commit) => commit.Length > 10 ? commit.Substring(0, 10) : commit;

    /// <summary>
    /// Resolves relative provenance sources against the live repo. A transform that runs in a staging root
    /// records sources relative to that root (the root itself is deleted before the operator reads the
    /// manifest); the live repo holds the same upstream paths, at the target commit, so the operator gets
    /// a file that actually opens. Absolute entries are left as they are.
    /// </summary>
    private IReadOnlyDictionary<string, string> RebaseOntoRepo(IReadOnlyDictionary<string, string> provenance)
    {
        if (provenance.Count == 0)
        {
            return provenance;
        }

        var rebased = new Dictionary<string, string>(provenance.Count, StringComparer.Ordinal);
        foreach (var kv in provenance)
        {
            rebased[kv.Key] = Path.IsPathRooted(kv.Value)
                ? kv.Value
                : Path.GetFullPath(Path.Combine(_options.RepoRoot, kv.Value.Replace('/', Path.DirectorySeparatorChar)));
        }

        return rebased;
    }

    /// <summary>
    /// The alignment proper: structural preflight (§3.4) → 3-way merge (§3.5) → conflict pause (§3.6)
    /// → residual-Chinese + post-merge report (§3.7) → advance the checkpoint (§3.8). Returns a process
    /// exit code (0 clean, 1 clean-with-findings, &gt;=2 aborted).
    /// </summary>
    public int AlignFromTrees(string baseTree, string theirsTree,
        IReadOnlyDictionary<string, string>? provenance, string targetCommit)
    {
        var repoRoot = _options.RepoRoot;
        var scope = _options.Scope.ToArray();

        // §3.4 Structural-match preflight (informational, non-fatal). OURS = f(base) + patches, and
        // patches legitimately rename (.props/.targets prefix), move, split, and delete files - so
        // f(base) and OURS differ structurally by design. We ignore obj/bin, reduce the differences
        // that renames/moves/splits explain, and only *report* what's left; we never abort. The 3-way
        // merge + conflict pause are the real safety net, and f-drift is already validated.
        var drifted = false;
        foreach (var p in scope)
        {
            var match = StructuralMatch.Compare(Path.Combine(baseTree, p), Path.Combine(repoRoot, p));
            var reduced = StructuralMatchReducer.Reduce(match);
            if (!reduced.HasResidual)
            {
                continue;
            }

            drifted = true;
            ConsoleColor.Yellow.Write(
                $"Note: '{p}' has layout differences not explained by rename/move/split " +
                "(usually patch deletions or committed settings) - proceeding:", true);
            foreach (var only in reduced.BaseOnly)
            {
                Console.WriteLine($"  only in f(base): {p}/{only}");
            }
            foreach (var only in reduced.OursOnly)
            {
                Console.WriteLine($"  only in OURS   : {p}/{only}");
            }
        }

        if (drifted && _options.AbortOnStructuralDrift)
        {
            RedConsole.Write("Structural drift between f(base) and OURS - alignment aborted; checkpoint not advanced.");
            return 2;
        }

        // §3.5 The 3-way merge - only the scope directories are ever replaced.
        var merge = new DeltaMerge(repoRoot, scope, _options.BranchPrefix);
        var result = merge.Apply(baseTree, theirsTree);
        if (!result.Clean)
        {
            var conflicts = ConflictReporter.Describe(repoRoot, baseTree, theirsTree, provenance, result.Conflicts);
            if (!new ConflictResolutionSession(_options).Resolve(merge, conflicts))
            {
                RedConsole.Write("Conflicts unresolved - alignment aborted; checkpoint not advanced.");
                return 3;
            }
        }

        // §3.7 Report + residual-Chinese check.
        var findings = 0;
        var residual = ResidualChineseScanner.Scan(scope.Select(p => Path.Combine(repoRoot, p)).ToArray());
        if (residual.Count > 0)
        {
            findings += residual.Count;
            ConsoleColor.Yellow.Write($"{residual.Count} file(s) still contain untranslated Chinese (blank dictionary entries):", true);
            foreach (var r in residual)
            {
                Console.WriteLine($"  {r.Key} ({r.Value} match(es))");
            }
        }

        // Tool-specific verification on top (AlignSamples' culture-completeness check).
        var extra = _options.PostMergeChecks?.Invoke() ?? Array.Empty<string>();
        if (extra.Count > 0)
        {
            findings += extra.Count;
            ConsoleColor.Yellow.Write($"{extra.Count} post-merge check failure(s):", true);
            foreach (var failure in extra)
            {
                Console.WriteLine($"  {failure}");
            }
        }

        // §3.8 Advance the checkpoint - and commit it, so the next run really starts from what this run
        // absorbed. An uncommitted checkpoint is the same bug as no checkpoint: the file gets reverted
        // with the next `git checkout`/`restore` and the following run recomputes a delta that is already
        // applied, double-applying it as spurious conflicts.
        AlignmentCheckpoint.Write(_options.CheckpointFile, targetCommit, _options.ToolName);
        ConsoleColor.Green.Write($"Aligned to {targetCommit}; checkpoint advanced.", true);
        CommitCheckpoint(targetCommit);
        return findings > 0 ? 1 : 0;
    }
}
