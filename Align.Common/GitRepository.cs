using System.Diagnostics;
using System.Text;

namespace Align.Common;

/// <summary>
/// The small amount of git the alignment tools need outside the merge itself: resolving the configured
/// target ref to a real commit, and committing a specific set of files (the checkpoint, the enriched
/// dictionaries) without touching anything else that happens to be staged.
/// </summary>
public static class GitRepository
{
    /// <summary>Runs git in <paramref name="workingDir"/> and returns its stdout and exit code.</summary>
    public static (string output, int exitCode) Run(string workingDir, string arguments)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            }
        };
        // Never block on an editor or a credential prompt.
        process.StartInfo.Environment["GIT_EDITOR"] = "true";
        process.StartInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (output, process.ExitCode);
    }

    /// <summary>
    /// Resolves <paramref name="reference"/> to a full commit id in <paramref name="repoDir"/>, trying the
    /// ref itself and then the usual remotes. Returns null when nothing matches.
    /// </summary>
    /// <remarks>
    /// Both alignment tools default their target to a BRANCH name, but a submodule checked out by
    /// <c>git submodule update</c> sits on a detached HEAD and often has no local branch of that name at
    /// all - <c>git worktree add … main</c> then fails with "invalid reference: main". Resolving up front
    /// fixes that, and matters just as much for the checkpoint: recording a moving branch name would make
    /// the next run's BASE mean "wherever main is now" instead of "the commit actually absorbed".
    /// </remarks>
    public static string? ResolveCommit(string repoDir, string reference)
    {
        if (string.IsNullOrWhiteSpace(reference) || !Directory.Exists(repoDir))
        {
            return null;
        }

        foreach (var candidate in new[] { reference, "origin/" + reference, "gitee/" + reference })
        {
            var (output, exitCode) = Run(repoDir, $"rev-parse --verify -q \"{candidate}^{{commit}}\"");
            var commit = output.Trim();
            if (exitCode == 0 && commit.Length > 0)
            {
                return commit;
            }
        }

        return null;
    }

    /// <summary>True when any of <paramref name="repoRelativePaths"/> differs from HEAD (staged or not).</summary>
    public static bool HasChanges(string repoRoot, IEnumerable<string> repoRelativePaths)
    {
        var pathspec = Pathspec(repoRelativePaths);
        return pathspec.Length > 0 && Run(repoRoot, $"status --porcelain -- {pathspec}").output.Trim().Length > 0;
    }

    /// <summary>
    /// Commits exactly <paramref name="repoRelativePaths"/> (pathspec-scoped, so nothing else that is
    /// staged rides along). Returns true when a commit was actually made.
    /// </summary>
    public static bool CommitPaths(string repoRoot, string message, IEnumerable<string> repoRelativePaths)
    {
        var paths = repoRelativePaths.ToList();
        if (!HasChanges(repoRoot, paths))
        {
            return false;
        }

        var pathspec = Pathspec(paths);
        Run(repoRoot, $"add -- {pathspec}");
        var (_, exitCode) = Run(repoRoot, $"commit --no-verify -m \"{message.Replace("\"", "'")}\" -- {pathspec}");
        return exitCode == 0;
    }

    /// <summary>Repo-relative path of <paramref name="fullPath"/>, or null when it lies outside the repo.</summary>
    public static string? RelativeTo(string repoRoot, string fullPath)
    {
        var root = Path.GetFullPath(repoRoot).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(fullPath);
        return full.StartsWith(root, StringComparison.OrdinalIgnoreCase)
            ? full.Substring(root.Length).Replace('\\', '/')
            : null;
    }

    private static string Pathspec(IEnumerable<string> paths) =>
        string.Join(" ", paths.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().Select(p => "\"" + p + "\""));
}
