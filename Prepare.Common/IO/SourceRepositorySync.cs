using System.Diagnostics;
using Prepare.Common.Logging;
using Prepare.Common.Projects;

namespace Prepare.Common.IO;

/// <summary>
/// Keeps the <c>gtksystem-windows-forms</c> submodule (the upstream fork) in a known state
/// around a PrepareProjects run.
/// On entry it pins the working tree to a fixed generation base commit, so the pipeline always
/// transforms the same source and the <c>patches/</c> series (anchored to that base) applies.
/// On exit it advances the fork's tracking branch to the latest gitee upstream.
/// </summary>
public static class SourceRepositorySync
{
    /// <summary>
    /// Full id of the frozen generation base: the third custom commit after
    /// <c>337d7df</c> (i.e. <c>e5227ae</c>) - the exact commit the current <c>Net4x.*</c>
    /// projects and the <c>patches/</c> series were generated from.
    /// </summary>
    public const string GenerationBaseCommit = "e5227aeb608d27affc5d348a4b165c5cc63d8bf3";

    /// <summary>Branch that tracks gitee and holds any manually-resolved upstream merge.</summary>
    public const string DefaultTrackingBranch = "main";

    private const string GiteeRemote = "gitee";
    private const string GiteeBranch = "master";

    private static string SourceRepositoryPath =>
        Path.Combine(Path.GetFullPath(@"..\..\..\..\"), "gtksystem-windows-forms");

    /// <summary>
    /// Force-checks out <see cref="GenerationBaseCommit"/> in DETACHED HEAD, discarding any
    /// working-tree changes left by a previous run. Detached on purpose: the tracking branch
    /// (which may hold your manually-resolved gitee merge) is never moved or lost.
    /// Returns the branch that was checked out, so <see cref="AdvanceToLatestGitee"/> can restore it.
    /// </summary>
    public static string CheckoutGenerationBase(bool printOutput = true)
    {
        var repo = SourceRepositoryPath;
        if (!IsGitRepository(repo))
        {
            YellowConsole.Write($"'{repo}' is not a git repository - skipping generation-base checkout.");
            return DefaultTrackingBranch;
        }

        var branch = RunGit("rev-parse --abbrev-ref HEAD", repo, printOutput).output.Trim();
        if (string.IsNullOrWhiteSpace(branch) || branch == "HEAD")
            branch = DefaultTrackingBranch;

        YellowConsole.Write(
            $"Resetting gtksystem-windows-forms working tree to base {GenerationBaseCommit.Substring(0, 7)} " +
            $"(force, detached from '{branch}').");
        RunGit($"checkout -f --detach {GenerationBaseCommit}", repo, printOutput);
        return branch;
    }

    /// <summary>
    /// Returns to <paramref name="trackingBranch"/> (discarding the pipeline's in-place source
    /// mutations), aligns it with its own remote via <c>git pull</c> (latest committed changes),
    /// then runs <c>git pull gitee master</c> to merge the latest gitee upstream on top. Only the
    /// new gitee commits are merged, so an existing resolved merge is preserved. On failure
    /// (network or merge conflict) each pull aborts any in-progress merge and warns instead of
    /// leaving the submodule half-merged.
    /// </summary>
    public static void AdvanceToLatestGitee(string trackingBranch, bool printOutput = true)
    {
        var repo = SourceRepositoryPath;
        if (!IsGitRepository(repo))
        {
            YellowConsole.Write($"'{repo}' is not a git repository - skipping gitee pull.");
            return;
        }

        // Return to the tracking branch, discarding the pipeline's in-place source mutations.
        RunGit($"checkout -f {trackingBranch}", repo, printOutput);

        // Align the branch with its own remote first, then merge the latest gitee upstream.
        PullOrAbort("pull", $"git pull ({trackingBranch})", repo, printOutput);
        PullOrAbort($"pull {GiteeRemote} {GiteeBranch}", $"git pull {GiteeRemote} {GiteeBranch}", repo, printOutput);
    }

    private static void PullOrAbort(string args, string description, string repo, bool printOutput)
    {
        var (_, _, exitCode) = RunGit(args, repo, printOutput);
        if (exitCode != 0)
        {
            RedConsole.Write(
                $"'{description}' failed (network or merge conflict). " +
                "Aborting any in-progress merge; advance gtksystem-windows-forms manually.");
            RunGit("merge --abort", repo, printOutput);
        }
    }

    private static bool IsGitRepository(string repo) =>
        Directory.Exists(Path.Combine(repo, ".git")) || File.Exists(Path.Combine(repo, ".git"));

    private static (string output, string error, int exitCode) RunGit(string args, string workingDir, bool printOutput)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = workingDir,
                CreateNoWindow = true
            }
        };

        var (output, error) = process.StartAndWaitForExit(printOutput);
        if (process.ExitCode != 0)
            Console.WriteLine($"Git command exited {process.ExitCode}: git {args}");

        return (output, error, process.ExitCode);
    }
}
