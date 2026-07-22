using System.Diagnostics;
using Prepare.Common.Projects;

namespace Prepare.Common.IO;

public static class GitPatchApplier
{
    public static void CommitAndApplyPatches(bool printOutput = true)
    {
        string repoPath = Path.GetFullPath(@"..\..\..\..\");
        string patchesPath = Path.Combine(repoPath, "patches");

        // 1. Stage everything
        RunGit("add .", repoPath, printOutput);

        // 2. Commit only if there are staged changes
        if (HasStagedChanges(repoPath, printOutput))
        {
            RunGit("commit -m \"Auto commit before applying patches\"", repoPath, printOutput);
        }
        else
        {
            Console.WriteLine("No changes to commit.");
        }

        // 3. Apply patches
        if (!Directory.Exists(patchesPath))
        {
            Console.WriteLine("Patches directory not found.");
            return;
        }

        var patchFiles = Directory.GetFiles(patchesPath, "*.patch");
        Array.Sort(patchFiles); // important for ordered patches

        // Two patches sharing an ordinal (0007-Fix-trackbar / 0007-Resolved-...) still apply in a
        // deterministic order, but that order comes from the *description* text rather than from any
        // intent - a trap the first time two same-numbered patches touch the same hunk.
        EnsureOrdinalsAreUnique(patchFiles);

        var failed = new List<string>();

        foreach (var patch in patchFiles)
        {
            var name = Path.GetFileName(patch);
            ConsoleColor.Cyan.Write($"Applying patch: {name}", true);

            if (!TryRunGit($"apply --3way \"{patch}\"", repoPath, printOutput))
            {
                failed.Add(name);
            }
        }

        // A patch that fails to apply used to be indistinguishable from one that applied: the result was
        // discarded and the loop moved on, so the pipeline could publish a library silently missing a fix.
        if (failed.Count > 0)
        {
            ConsoleColor.Red.Write(
                $"{failed.Count} of {patchFiles.Length} patches did NOT apply:", true);
            foreach (var name in failed)
            {
                ConsoleColor.Red.Write($"  - {name}", true);
            }

            ConsoleColor.Red.Write(
                "The generated library is missing the fixes above. Rebase or drop them before publishing.",
                true);
        }
        else
        {
            ConsoleColor.Green.Write($"All {patchFiles.Length} patches applied.", true);
        }
    }

    /// <summary>
    /// Throws when two patches share a leading ordinal, so the ambiguity is reported at the start of the
    /// run rather than discovered as a mis-ordered hunk later. Distinguish them with a letter suffix
    /// (<c>0007-...</c>, <c>0007a-...</c>), which keeps the intended order under a plain filename sort.
    /// </summary>
    public static void EnsureOrdinalsAreUnique(IEnumerable<string> patchFiles)
    {
        var duplicates = patchFiles
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .GroupBy(name => name!.Split('-')[0], StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .ToList();

        if (duplicates.Count == 0)
        {
            return;
        }

        var detail = string.Join("; ",
            duplicates.Select(group => $"{group.Key}: {string.Join(", ", group)}"));

        throw new InvalidOperationException(
            $"Patch ordinals must be unique, but these are shared - {detail}. " +
            "Rename one of each pair with a letter suffix (e.g. 0007a-...) to make the apply order explicit.");
    }

    private static bool HasStagedChanges(string repoPath, bool printOutput)
    {
        var process = CreateGitProcess("diff --cached --quiet", repoPath);
        process.StartAndWaitForExit(printOutput);

        return process.ExitCode != 0; // 0 = no changes, 1 = has changes
    }

    /// <summary>Runs git and reports whether it succeeded, instead of only printing on failure.</summary>
    private static bool TryRunGit(string args, string workingDir, bool printOutput)
    {
        var process = CreateGitProcess(args, workingDir);
        process.StartAndWaitForExit(printOutput);

        return process.ExitCode == 0;
    }

    private static (string output, string error) RunGit(string args, string workingDir, bool printOutput)
    {
        var process = CreateGitProcess(args, workingDir);
        var result = process.StartAndWaitForExit(printOutput);

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Git command failed: git {args}");
        }

        return result;
    }

    private static Process CreateGitProcess(string args, string workingDir)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = workingDir,
                CreateNoWindow = true
            }
        };
    }
}