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

        foreach (var patch in patchFiles)
        {
            ConsoleColor.Cyan.Write($"Applying patch: {Path.GetFileName(patch)}", true);

            var (_, error) = RunGit($"apply --3way \"{patch}\"", repoPath, printOutput);
        }
    }

    private static bool HasStagedChanges(string repoPath, bool printOutput)
    {
        var process = CreateGitProcess("diff --cached --quiet", repoPath);
        process.StartAndWaitForExit(printOutput);

        return process.ExitCode != 0; // 0 = no changes, 1 = has changes
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