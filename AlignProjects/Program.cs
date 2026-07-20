using System.Text;
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
/// This is the project-setup skeleton; the alignment steps are stubbed as TODOs.
/// </summary>
public static class Program
{
    /// <summary>Folder inside each produced tree holding that run's <c>*.Files.xml</c> provenance manifests.</summary>
    private const string ManifestsFolder = "_manifests";

    /// <summary>Solution-directory file listing every conflict the operator must resolve by hand.</summary>
    private const string ConflictLogName = "conflicts.log";

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
        var targetCommit = Environment.GetEnvironmentVariable("ALIGN_TARGET_COMMIT")
                           ?? SourceRepositorySync.DefaultTrackingBranch;
        // The target (gitee-merged) upstream source to scan for Chinese - whole submodule (Source + Samples).
        var sourceDir = Environment.GetEnvironmentVariable("ALIGN_SOURCE_DIR")
                        ?? Path.Combine(repoRoot, "gtksystem-windows-forms");

        Console.WriteLine("AlignProjects - align Net4x.* with the latest gitee upstream");
        Console.WriteLine($"  base commit   : {baseCommit}");
        Console.WriteLine($"  target commit : {targetCommit}");
        Console.WriteLine($"  source dir    : {sourceDir}");

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
        PauseForManualTranslation(dictAdded, resxAdded, translationsXml, resourcesDir);

        // --- Phase 3: the alignment driver (see AlignProjects-Plan.md §3) ---
        // Produce BASE/THEIRS by running the shared transform in isolated staging roots (§3.1/§3.2),
        // or accept pre-produced trees via ALIGN_BASE_TREE/ALIGN_THEIRS_TREE (test seam).
        var baseTreeEnv = Environment.GetEnvironmentVariable("ALIGN_BASE_TREE");
        var theirsTreeEnv = Environment.GetEnvironmentVariable("ALIGN_THEIRS_TREE");

        string baseTree;
        string theirsTree;
        string? buildDir = null;
        if (!string.IsNullOrEmpty(baseTreeEnv) && !string.IsNullOrEmpty(theirsTreeEnv))
        {
            baseTree = baseTreeEnv!;
            theirsTree = theirsTreeEnv!;
        }
        else
        {
            // Output trees: a stable, gitignored location inside the repo (not %TEMP%), because the
            // conflict pause hands these paths to the operator as the diffed pair - they must be short,
            // predictable, and still there after the run ends. Cleared on entry; removed on exit only
            // when nothing is left to resolve (see below).
            buildDir = Path.Combine(repoRoot, "AlignProjects", "build");
            if (Directory.Exists(buildDir))
            {
                Directory.Delete(buildDir, true);
            }

            // Staging roots: OUTSIDE the repo. MirrorPlan junctions the repo's top-level directories into
            // each staging root, so a staging root placed under the repo becomes reachable through its own
            // mirror (<staging>/AlignProjects -> repo/AlignProjects -> build/base-root -> <staging> -> ...)
            // and any SearchOption.AllDirectories walk recurses until the path length blows up. They are
            // transient (disposed after each transform), so nothing is lost by keeping them in %TEMP%.
            var stagingParent = Path.Combine(Path.GetTempPath(), "alignprojects-staging-" + Guid.NewGuid().ToString("N"));

            ConsoleColor.Green.Write("Producing transformed BASE and THEIRS trees (runs the transform twice)...", true);
            try
            {
                (baseTree, theirsTree) = await ProduceTransformedTrees(
                    repoRoot, "gtksystem-windows-forms", baseCommit, targetCommit, buildDir, stagingParent);
            }
            finally
            {
                try { Directory.Delete(stagingParent, true); } catch { /* best-effort */ }
            }
        }

        // Provenance (generated -> raw source) for the conflict pause. Prefer the purpose-built manifest
        // when RunTransform emits one (§3.1); otherwise derive it from the *.Files.xml manifests the
        // transform already writes, resolved against the LIVE submodule - which sits at the target
        // commit, so the paths are real files the operator can open (the staging worktrees are gone).
        var provenance = ProvenanceManifest.Load(
            Environment.GetEnvironmentVariable("ALIGN_PROVENANCE") ?? Path.Combine(theirsTree, "provenance.tsv"));
        if (provenance.Count == 0)
        {
            provenance = FilesXmlProvenance.Load(
                Path.Combine(theirsTree, ManifestsFolder),
                Path.Combine(sourceDir, "Source", "GTKSystem.Windows.Forms"));
            Console.WriteLine($"Provenance from *.Files.xml: {provenance.Count} generated file(s) mapped to upstream source.");
        }

        var result = AlignFromTrees(repoRoot, baseTree, theirsTree, provenance, checkpointFile, targetCommit);

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
    /// Pauses so the newly-added blank entries can be translated by hand, then waits for the user
    /// to confirm before the pipeline continues. Skips the pause when there is nothing new to
    /// translate, or when input is redirected (non-interactive run).
    /// </summary>
    private static void PauseForManualTranslation(int dictAdded, int resxAdded, string translationsXml, string resourcesDir)
    {
        if (dictAdded == 0 && resxAdded == 0)
        {
            Console.WriteLine("No new Chinese to translate - continuing.");
            return;
        }

        var separator = new string('=', 72);
        ConsoleColor.Yellow.Write(separator, true);
        ConsoleColor.Yellow.Write("MANUAL TRANSLATION REQUIRED", true);
        ConsoleColor.Yellow.Write(separator, true);

        if (dictAdded > 0)
        {
            Console.WriteLine($"{dictAdded} new blank-English entr{(dictAdded == 1 ? "y was" : "ies were")} added to:");
            Console.WriteLine($"  {translationsXml}");
        }

        if (resxAdded > 0)
        {
            Console.WriteLine($"{resxAdded} blank resx entr{(resxAdded == 1 ? "y was" : "ies were")} added under:");
            Console.WriteLine($"  {resourcesDir}");
        }

        Console.WriteLine();
        Console.WriteLine("Fill in the empty <English></English> values (and any blank resx <value/>),");
        Console.WriteLine("then return here to continue.");

        if (Console.IsInputRedirected)
        {
            ConsoleColor.Yellow.Write("Input is redirected (non-interactive) - not pausing; continuing.", true);
            return;
        }

        Console.Write("Press ENTER when the translations are done (or Ctrl+C to abort)... ");
        Console.ReadLine();
        ConsoleColor.Green.Write("Continuing.", true);
    }

    /// <summary>
    /// The Phase 3 alignment: structural preflight (§3.4) → 3-way merge (§3.5) → conflict pause (§3.6)
    /// → residual-Chinese report (§3.7) → advance the checkpoint (§3.8). Returns a process exit code
    /// (0 clean, 1 clean-with-residual-Chinese, &gt;=2 aborted).
    /// </summary>
    private static int AlignFromTrees(string repoRoot, string baseTree, string theirsTree,
        IReadOnlyDictionary<string, string>? provenance, string checkpointFile, string targetCommit)
    {
        string[] scope = { "Net4x.Gtk.Windows.Forms", "Net4x.Gtk.Drawing" };

        // §3.4 Structural-match preflight (informational, non-fatal). OURS = f(base) + patches, and
        // patches legitimately rename (.props/.targets prefix), move, split, and delete files - so
        // f(base) and OURS differ structurally by design. We ignore obj/bin, reduce the differences
        // that renames/moves/splits explain, and only *report* what's left; we never abort. The 3-way
        // merge + conflict pause are the real safety net, and f-drift is already validated.
        foreach (var p in scope)
        {
            var match = StructuralMatch.Compare(Path.Combine(baseTree, p), Path.Combine(repoRoot, p));
            var reduced = StructuralMatchReducer.Reduce(match);
            if (reduced.HasResidual)
            {
                var baseOnly = reduced.BaseOnly;
                var oursOnly = reduced.OursOnly;
                ConsoleColor.Yellow.Write(
                    $"Note: '{p}' has layout differences not explained by rename/move/split " +
                    "(usually patch deletions or committed settings) - proceeding:", true);
                foreach (var only in baseOnly)
                {
                    Console.WriteLine($"  only in f(base): {p}/{only}");
                }
                foreach (var only in oursOnly)
                {
                    Console.WriteLine($"  only in OURS   : {p}/{only}");
                }
            }
        }

        // §3.5 The 3-way merge.
        var merge = new DeltaMerge(repoRoot, scope);
        var result = merge.Apply(baseTree, theirsTree);
        if (!result.Clean)
        {
            var conflicts = ConflictReporter.Describe(repoRoot, baseTree, theirsTree, provenance, result.Conflicts);
            if (!PauseForConflictResolution(merge, conflicts, repoRoot))
            {
                RedConsole.Write("Conflicts unresolved - alignment aborted; checkpoint not advanced.");
                return 3;
            }
        }

        // §3.7 Report + residual-Chinese check.
        var residual = ResidualChineseScanner.Scan(Path.Combine(repoRoot, scope[0]), Path.Combine(repoRoot, scope[1]));
        if (residual.Count > 0)
        {
            ConsoleColor.Yellow.Write($"{residual.Count} file(s) still contain untranslated Chinese (blank dictionary entries):", true);
            foreach (var r in residual)
            {
                Console.WriteLine($"  {r.Key} ({r.Value} match(es))");
            }
        }

        // §3.8 Advance the checkpoint.
        AlignmentCheckpoint.Write(checkpointFile, targetCommit);
        ConsoleColor.Green.Write($"Aligned to {targetCommit}; checkpoint advanced.", true);
        return residual.Count > 0 ? 1 : 0;
    }

    /// <summary>
    /// §3.6 conflict handling: auto-merges the upstream delta into every conflicted file first, then
    /// writes ALL remaining hand-resolution work to <c>conflicts.log</c> in the solution directory and
    /// pauses ONCE. Stopping per file does not scale - a large upstream delta produces hundreds of
    /// conflicts, and the operator needs the whole list up front to work through in an editor. Re-checks
    /// after the pause and rewrites the log with whatever is still outstanding (partial resolution).
    /// Returns false when non-interactive (leaves conflicts in place) or on failure.
    /// </summary>
    private static bool PauseForConflictResolution(DeltaMerge merge, IReadOnlyList<ConflictInfo> conflicts, string repoRoot)
    {
        var logPath = Path.GetFullPath(Path.Combine(repoRoot, ConflictLogName));

        // Files a patch moved: the old path is already resolved via `git rm`, so they never appear in
        // CurrentConflicts() - track them separately or their markers would go unnoticed.
        var forkCopiesToVerify = new List<string>();
        var entries = new List<string>();

        // One directory walk up front instead of one per conflict (FindForkCopies used to enumerate both
        // project trees for every single file - the dominant cost at 625 conflicts).
        var forkIndex = BuildForkFileIndex(repoRoot);

        var total = conflicts.Count;
        ConsoleColor.Green.Write($"Merging the upstream delta into {total} conflicted file(s)...", true);
        Console.WriteLine($"Conflicts needing manual work are appended to {logPath} as they are found.");

        // Written incrementally: at this volume the operator can start working through the log while the
        // pass is still running, and a crash mid-pass still leaves the work found so far on disk.
        using (var log = BeginConflictLog(logPath))
        {
            var index = 0;
            var manual = 0;
            foreach (var c in conflicts)
            {
                index++;
                var entry = MergeOrDescribe(merge, c, repoRoot, forkCopiesToVerify, forkIndex);
                if (entry != null)
                {
                    entries.Add(entry);
                    manual++;
                    AppendConflictEntry(log, manual, entry);
                    ConsoleColor.Yellow.Write($"[{index}/{total}] MANUAL  {c.RelativePath}", true);
                }
                else
                {
                    Console.WriteLine($"[{index}/{total}] merged  {c.RelativePath}");
                }
            }

            ConsoleColor.Green.Write($"Auto-merged {total - manual}/{total}; {manual} need manual resolution.", true);
        }

        // Fork copies whose merge came out clean can be staged now; the rest need the operator.
        foreach (var forkCopy in forkCopiesToVerify.Where(f => !FileHasConflictMarkers(f)).ToList())
        {
            merge.StageFile(forkCopy);
            forkCopiesToVerify.Remove(forkCopy);
        }

        while (true)
        {
            var outstanding = Outstanding(merge, entries, forkCopiesToVerify);
            if (outstanding.Count == 0)
            {
                break;
            }

            WriteConflictLog(logPath, outstanding);

            var separator = new string('=', 72);
            ConsoleColor.Yellow.Write(separator, true);
            ConsoleColor.Yellow.Write($"MANUAL MERGE RESOLUTION REQUIRED - {outstanding.Count} conflict(s)", true);
            ConsoleColor.Yellow.Write(separator, true);
            Console.WriteLine("Every conflict is described, with full paths, in:");
            ConsoleColor.White.Write($"    {logPath}", true);
            Console.WriteLine();
            Console.WriteLine("Resolve ALL conflicts logged in that file (and `git add` each resolved file)");
            Console.WriteLine("before proceeding.");

            if (Console.IsInputRedirected)
            {
                ConsoleColor.Yellow.Write("Input is redirected (non-interactive) - conflicts left in place.", true);
                return false;
            }

            Console.Write("Press ENTER when they are ALL resolved (Ctrl+C to abort)... ");
            Console.ReadLine();
        }

        DeleteConflictLog(logPath);
        merge.Continue();
        ConsoleColor.Green.Write("All conflicts resolved - merge continued.", true);
        return true;
    }

    /// <summary>
    /// Merges the upstream delta into one conflicted file. Returns null when nothing is left for the
    /// operator, otherwise the log entry describing what to do. Fork copies that may still carry markers
    /// are added to <paramref name="forkCopiesToVerify"/>.
    /// </summary>
    private static string? MergeOrDescribe(DeltaMerge merge, ConflictInfo c, string repoRoot,
        List<string> forkCopiesToVerify, IReadOnlyDictionary<string, List<string>> forkIndex)
    {
        // 1) Same-path conflict: merge the upstream delta (theirs - base) straight into 'edit here'.
        // A successful merge leaves the file unmerged in the index by design (the operator resolves any
        // markers), so ask the FILE whether work remains rather than re-querying git per conflict.
        if (merge.IncorporateUpstreamInto(c.RelativePath, c.BaseRefFullPath, c.TheirsRefFullPath))
        {
            if (FileHasConflictMarkers(c.Net4xFullPath))
            {
                return Describe(c, "The upstream change was merged into 'edit here'. Resolve the <<<<<<< markers, then `git add` it.");
            }

            // Clean merge - stage it here. The path is still unmerged in the INDEX (IncorporateUpstreamInto
            // only rewrites the working file), and with no log entry nobody would ever `git add` it, so
            // Continue() would refuse to finalize the cherry-pick.
            merge.StageFile(c.Net4xFullPath);
            return null;
        }

        // 2) A patch moved the file: merge the delta into the fork's actual copy, drop the old path.
        // Only when the fork genuinely lacks this path - IncorporateIntoForkCopy `git rm`s it, and
        // step 1 can also fail merely because the reference files were missing, in which case the
        // conflicted path is a live file that must not be deleted.
        var forkCopies = FindForkCopies(forkIndex, repoRoot, c.RelativePath);
        if (forkCopies.Count == 1 && !merge.HasForkSide(c.RelativePath))
        {
            var forkCopy = forkCopies[0];
            var hasMarkers = merge.IncorporateIntoForkCopy(
                c.RelativePath, forkCopy, c.BaseRefFullPath, c.TheirsRefFullPath);

            if (!hasMarkers)
            {
                merge.StageFile(forkCopy);
                return null;
            }

            forkCopiesToVerify.Add(forkCopy);
            return Describe(c,
                "MOVED FILE: the upstream change was merged into the fork's copy and the old path removed." +
                $"{Environment.NewLine}      Resolve the <<<<<<< markers in: {forkCopy}{Environment.NewLine}" +
                "      then `git add` THAT file.");
        }

        // 3) Ambiguous (no fork copy, or several): can't auto-merge - guide the operator by hand.
        // Report the ACTUAL reason: a missing reference file in the trees is not the same thing as a
        // path the fork no longer has, and conflating them sends the operator hunting for the wrong
        // problem (see TreePathResolver).
        var refsMissing = !File.Exists(c.BaseRefFullPath) || !File.Exists(c.TheirsRefFullPath);
        var hasForkSide = merge.HasForkSide(c.RelativePath);

        string guidance;
        if (refsMissing)
        {
            guidance = "This path could not be located in the transformed trees, so the upstream delta " +
                       "could not be merged for you. Resolve the markers by hand, then `git add` it.";
        }
        else if (hasForkSide)
        {
            guidance = "Automatic merge of the upstream delta failed. Resolve the markers by hand using " +
                       "the DIFFED pair above, then `git add` it.";
        }
        else
        {
            guidance = "A patch moved/deleted this path. Apply the (THEIRS vs base) change to the fork's " +
                       "copy, then `git rm` this path (or `git add` the resolution).";
        }

        if (forkCopies.Count == 0 && !hasForkSide)
        {
            guidance += $"{Environment.NewLine}      (no same-named file found in the fork - it may have been deleted; `git rm` this path if so)";
        }
        else
        {
            guidance = forkCopies.Aggregate(guidance,
                (current, q) => current + $"{Environment.NewLine}      fork copy: {q}");
        }

        return Describe(c, guidance);
    }

    /// <summary>Formats one log entry: the conflict's paths followed by what the operator must do.</summary>
    private static string Describe(ConflictInfo c, string guidance) =>
        $"{ConflictReporter.FormatOne(c)}      -> {guidance}{Environment.NewLine}";

    /// <summary>
    /// The entries still needing attention: those whose path is unmerged, plus any moved-file fork copy
    /// that still carries markers (those never show up in the index as unmerged).
    /// </summary>
    private static List<string> Outstanding(DeltaMerge merge, List<string> entries, List<string> forkCopiesToVerify)
    {
        var unmerged = new HashSet<string>(
            merge.CurrentConflicts().Select(r => r.Replace('\\', '/')), StringComparer.OrdinalIgnoreCase);
        var pendingCopies = forkCopiesToVerify.Where(FileHasConflictMarkers).ToList();

        return entries
            .Where(e => unmerged.Any(u => e.Contains(u, StringComparison.OrdinalIgnoreCase))
                        || pendingCopies.Any(p => e.Contains(p, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Opens the solution-directory log and writes its header. Entries are appended as they are found
    /// (see <see cref="AppendConflictEntry"/>) rather than buffered to the end of the pass, so the
    /// operator can start work immediately and nothing is lost if the pass dies partway.
    /// </summary>
    private static StreamWriter BeginConflictLog(string logPath)
    {
        var log = new StreamWriter(logPath, false) { AutoFlush = true };
        log.WriteLine(new string('=', 72));
        log.WriteLine("AlignProjects - conflicts requiring manual resolution");
        log.WriteLine(new string('=', 72));
        log.WriteLine();
        log.WriteLine("Resolve EVERY conflict listed below, `git add` each resolved file, then return to");
        log.WriteLine("the AlignProjects console and press ENTER to continue the merge.");
        log.WriteLine();
        log.WriteLine("For each entry: 'edit here' is the file to change; the DIFFED pair is exactly what");
        log.WriteLine("the upstream delta was computed from (diff those two to see the change to apply).");
        log.WriteLine();
        log.WriteLine("NOTE: this file grows while the merge pass runs; it is complete once the console");
        log.WriteLine("      reports the auto-merge summary.");
        log.WriteLine();
        return log;
    }

    /// <summary>Appends one conflict to the open log, flushed immediately.</summary>
    private static void AppendConflictEntry(StreamWriter log, int number, string entry)
    {
        log.WriteLine(new string('-', 72));
        log.WriteLine($"[{number}]");
        log.Write(entry);
        log.WriteLine();
    }

    /// <summary>Rewrites the log with only what is still outstanding, after a resolution round.</summary>
    private static void WriteConflictLog(string logPath, IReadOnlyList<string> entries)
    {
        using var log = BeginConflictLog(logPath);
        var index = 0;
        foreach (var entry in entries)
        {
            index++;
            AppendConflictEntry(log, index, entry);
        }
    }

    /// <summary>Removes the log once nothing is outstanding, so a stale file never reads as current work.</summary>
    private static void DeleteConflictLog(string logPath)
    {
        try
        {
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
        }
        catch
        {
            // best-effort
        }
    }


    /// <summary>True if the file still contains a git conflict marker (a merge left unresolved).</summary>
    private static bool FileHasConflictMarkers(string fullPath) =>
        File.Exists(fullPath) && File.ReadAllText(fullPath).Contains("<<<<<<<");

    /// <summary>
    /// Indexes the live Net4x.* projects once as filename → full paths (excluding obj/bin), so the
    /// per-conflict fork-copy lookup is a dictionary hit instead of a recursive directory walk.
    /// </summary>
    private static Dictionary<string, List<string>> BuildForkFileIndex(string repoRoot)
    {
        var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var project in new[] { "Net4x.Gtk.Windows.Forms", "Net4x.Gtk.Drawing" })
        {
            var dir = Path.Combine(repoRoot, project);
            if (!Directory.Exists(dir))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                var normalized = file.Replace('/', '\\');
                if (normalized.Contains("\\bin\\") || normalized.Contains("\\obj\\"))
                {
                    continue;
                }

                var name = Path.GetFileName(file);
                if (!index.TryGetValue(name, out var list))
                {
                    index[name] = list = new List<string>();
                }

                list.Add(Path.GetFullPath(file));
            }
        }

        return index;
    }

    /// <summary>
    /// Files in the live Net4x.* projects with the same filename as the conflicted path (excluding that
    /// path itself) — the fork's copy of a file a patch moved.
    /// </summary>
    private static IReadOnlyList<string> FindForkCopies(IReadOnlyDictionary<string, List<string>> forkIndex,
        string repoRoot, string conflictRelPath)
    {
        if (!forkIndex.TryGetValue(Path.GetFileName(conflictRelPath), out var candidates))
        {
            return Array.Empty<string>();
        }

        var self = Path.GetFullPath(Path.Combine(repoRoot, conflictRelPath.Replace('/', Path.DirectorySeparatorChar)));
        return candidates.Where(f => !string.Equals(f, self, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// Produces the two transformed trees by running the shared transform against the base and target
    /// commits in isolated staging roots (§3.1/§3.2). The staging roots live under
    /// <paramref name="stagingParent"/> (outside the repo — see the cycle note at the call site); only the
    /// resulting trees land under <paramref name="buildDir"/>. Returns the dirs holding the generated Net4x.*.
    /// </summary>
    private static async Task<(string baseTree, string theirsTree)> ProduceTransformedTrees(
        string repoRoot, string submoduleName, string baseCommit, string targetCommit,
        string buildDir, string stagingParent)
    {
        var baseTree = await ProduceOne(repoRoot, submoduleName, baseCommit,
            Path.Combine(stagingParent, "base-root"), Path.Combine(buildDir, "base"));
        var theirsTree = await ProduceOne(repoRoot, submoduleName, targetCommit,
            Path.Combine(stagingParent, "theirs-root"), Path.Combine(buildDir, "theirs"));
        return (baseTree, theirsTree);
    }

    private static async Task<string> ProduceOne(string repoRoot, string submoduleName, string commit,
        string stagingRootPath, string outTreeDir)
    {
        var (junctions, files, copiedDirs) = MirrorPlan(repoRoot, submoduleName);
        string[] outputs = { "Net4x.Gtk.Windows.Forms", "Net4x.Gtk.Drawing" };

        using (var staging = StagingRoot.Create(stagingRootPath, Path.Combine(repoRoot, submoduleName),
                   submoduleName, commit, junctions, files, outputs, copiedDirs))
        {
            var previousRoot = RepoPaths.Root;
            try
            {
                RepoPaths.Root = staging.RootPath;
                await TransformPipeline.RunTransform();
            }
            finally
            {
                RepoPaths.Root = previousRoot;
            }

            if (Directory.Exists(outTreeDir))
            {
                Directory.Delete(outTreeDir, true);
            }

            foreach (var output in outputs)
            {
                CopyDir(Path.Combine(staging.RootPath, output), Path.Combine(outTreeDir, output));
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
    /// Staging-root mirror plan: junction every top-level repo directory (except .git/.vs, the source
    /// worktree, and the two output dirs) and copy every top-level file, so the transform sees the full
    /// repo layout (props, Nuget.Config, snk, sibling projects) but writes in isolation.
    /// </summary>
    private static (Dictionary<string, string> junctions, Dictionary<string, string> files,
        Dictionary<string, string> copiedDirs) MirrorPlan(string repoRoot, string worktreeName)
    {
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

        // 'AlignProjects' is never mirrored: the transform has no use for this tool's own directory, and
        // mirroring it would expose build/{base,theirs} - the previous run's captured trees - to the
        // transform's whole-root walks (post-processing would rewrite files inside them), as well as
        // re-introducing a directory cycle if a staging root ever lands under the repo again.
        var skip = new HashSet<string>(
            new[]
            {
                ".git", ".vs", worktreeName, "Resources", "AlignProjects",
                "Net4x.Gtk.Windows.Forms", "Net4x.Gtk.Drawing"
            },
            StringComparer.OrdinalIgnoreCase);

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
