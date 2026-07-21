using Prepare.Common.Alignment;
using Prepare.Common.Logging;

namespace Align.Common;

/// <summary>
/// §3.6 conflict handling, shared by every alignment tool: auto-merges the upstream delta into each
/// conflicted file, writes whatever still needs hand resolution to the solution-directory conflict log,
/// and pauses ONCE for the operator. Stopping per file does not scale - a large upstream delta produces
/// hundreds of conflicts, and the operator needs the whole list up front to work through in an editor.
/// </summary>
public sealed class ConflictResolutionSession
{
    private readonly AlignmentOptions _options;

    public ConflictResolutionSession(AlignmentOptions options) => _options = options;

    /// <summary>
    /// Runs the auto-merge pass, then loops on the pause until nothing is unmerged. Re-checks after each
    /// round and rewrites the log with whatever is still outstanding (partial resolution). Returns false
    /// when non-interactive (leaves conflicts in place) or on failure.
    /// </summary>
    public bool Resolve(DeltaMerge merge, IReadOnlyList<ConflictInfo> conflicts)
    {
        var repoRoot = _options.RepoRoot;
        var logPath = Path.GetFullPath(Path.Combine(repoRoot, _options.ConflictLogName));

        // Files a patch moved: the old path is already resolved via `git rm`, so they never appear in
        // CurrentConflicts() - track them separately or their markers would go unnoticed.
        var forkCopiesToVerify = new List<string>();
        var entries = new List<string>();

        // One directory walk up front instead of one per conflict (FindForkCopies used to enumerate both
        // project trees for every single file - the dominant cost at 625 conflicts).
        var forkIndex = BuildForkFileIndex();

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
                var entry = MergeOrDescribe(merge, c, forkCopiesToVerify, forkIndex);
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
    private string? MergeOrDescribe(DeltaMerge merge, ConflictInfo c,
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
        var forkCopies = FindForkCopies(forkIndex, c.RelativePath);
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
    private StreamWriter BeginConflictLog(string logPath)
    {
        var log = new StreamWriter(logPath, false) { AutoFlush = true };
        log.WriteLine(new string('=', 72));
        log.WriteLine($"{_options.ToolName} - conflicts requiring manual resolution");
        log.WriteLine(new string('=', 72));
        log.WriteLine();
        log.WriteLine("Resolve EVERY conflict listed below, `git add` each resolved file, then return to");
        log.WriteLine($"the {_options.ToolName} console and press ENTER to continue the merge.");
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
    private void WriteConflictLog(string logPath, IReadOnlyList<string> entries)
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
    /// Indexes the live scope directories once as filename → full paths (excluding obj/bin), so the
    /// per-conflict fork-copy lookup is a dictionary hit instead of a recursive directory walk.
    /// </summary>
    private Dictionary<string, List<string>> BuildForkFileIndex()
    {
        var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var project in _options.Scope)
        {
            var dir = Path.Combine(_options.RepoRoot, project);
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
    /// Files in the live scope directories with the same filename as the conflicted path (excluding that
    /// path itself) — the fork's copy of a file a patch moved.
    /// </summary>
    private IReadOnlyList<string> FindForkCopies(IReadOnlyDictionary<string, List<string>> forkIndex,
        string conflictRelPath)
    {
        if (!forkIndex.TryGetValue(Path.GetFileName(conflictRelPath), out var candidates))
        {
            return Array.Empty<string>();
        }

        var self = Path.GetFullPath(Path.Combine(_options.RepoRoot,
            conflictRelPath.Replace('/', Path.DirectorySeparatorChar)));
        return candidates.Where(f => !string.Equals(f, self, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
