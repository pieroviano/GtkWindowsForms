using System.Diagnostics;
using Prepare.Common.Projects;

namespace Prepare.Common.Alignment;

/// <summary>
/// A private, isolated "repo root" for running the transform against one commit without touching the
/// live repo (AlignProjects-Plan.md §3.1 option B / §3.2). It holds: the source submodule as a detached
/// <c>git worktree</c> at the wanted commit; read-only shared dirs (e.g. <c>mono</c>) as directory
/// <b>junctions</b> (no elevation on Windows) so reads see identical content; <b>copied</b> dirs for
/// anything the transform <i>writes</i> into (notably <c>Resources</c>, where
/// <c>ProjectUtility.PersistProjectFiles</c> emits the <c>*.Files.xml</c> source→destination maps — a
/// junction there would write straight through into the live repo); a copied <c>.editorconfig</c>; and
/// empty <c>Net4x.*</c> output dirs. Dispose removes the worktree and the junctions (leaving their
/// targets intact) and deletes the root.
/// </summary>
public sealed class StagingRoot : IDisposable
{
    private readonly string _sourceRepo;
    private readonly string _worktreePath;
    private readonly List<string> _junctionPaths = new();

    private StagingRoot(string rootPath, string sourceRepo, string worktreePath)
    {
        RootPath = rootPath;
        _sourceRepo = sourceRepo;
        _worktreePath = worktreePath;
    }

    /// <summary>The staging root directory (use as the transform's <c>repoRoot</c>).</summary>
    public string RootPath { get; }

    /// <summary>
    /// Creates the staging root: a worktree of <paramref name="sourceRepo"/> at <paramref name="commit"/>
    /// under <paramref name="sourceLinkName"/>, plus the given junctions (linkName → target dir), copied
    /// files (name → source file), copied directories (name → source dir, for dirs the transform writes
    /// into), and empty output dirs.
    /// </summary>
    public static StagingRoot Create(
        string rootPath,
        string sourceRepo,
        string sourceLinkName,
        string commit,
        IReadOnlyDictionary<string, string> junctions,
        IReadOnlyDictionary<string, string> files,
        IEnumerable<string> emptyDirs,
        IReadOnlyDictionary<string, string>? copiedDirs = null)
    {
        rootPath = Path.GetFullPath(rootPath);
        sourceRepo = Path.GetFullPath(sourceRepo);
        Directory.CreateDirectory(rootPath);

        var worktreePath = Path.Combine(rootPath, sourceLinkName);
        RunOrThrow("git", $"worktree add --detach --force \"{worktreePath}\" {commit}", sourceRepo);

        var staging = new StagingRoot(rootPath, sourceRepo, worktreePath);

        foreach (var kv in junctions)
        {
            var link = Path.Combine(rootPath, kv.Key);
            CreateJunction(link, Path.GetFullPath(kv.Value));
            staging._junctionPaths.Add(link);
        }

        foreach (var kv in files)
        {
            var dest = Path.Combine(rootPath, kv.Key);
            var dir = Path.GetDirectoryName(dest);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(Path.GetFullPath(kv.Value), dest, true);
        }

        foreach (var kv in copiedDirs ?? new Dictionary<string, string>())
        {
            CopyDirectory(Path.GetFullPath(kv.Value), Path.Combine(rootPath, kv.Key));
        }

        foreach (var d in emptyDirs)
        {
            Directory.CreateDirectory(Path.Combine(rootPath, d));
        }

        return staging;
    }

    /// <summary>Recursively copies a directory into the staging root (writes here stay isolated).</summary>
    private static void CopyDirectory(string source, string dest)
    {
        if (!Directory.Exists(source))
        {
            return;
        }

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(dest, file.Substring(source.Length).TrimStart('\\', '/'));
            var dir = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.Copy(file, target, true);
        }
    }

    public void Dispose()
    {
        // Remove junctions with `rmdir` (removes the link only, never the target contents).
        foreach (var junction in _junctionPaths)
        {
            TryRun("cmd", $"/c rmdir \"{junction}\"");
        }

        TryRun("git", $"worktree remove --force \"{_worktreePath}\"", _sourceRepo);
        TryRun("git", "worktree prune", _sourceRepo);

        try
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, true);
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }

    private static void CreateJunction(string link, string target)
    {
        var (_, error, code) = Run("cmd", $"/c mklink /J \"{link}\" \"{target}\"");
        if (code != 0)
        {
            throw new InvalidOperationException($"mklink /J failed for '{link}' -> '{target}': {error}");
        }
    }

    private static void RunOrThrow(string file, string args, string? workingDir = null)
    {
        var (_, error, code) = Run(file, args, workingDir);
        if (code != 0)
        {
            throw new InvalidOperationException($"{file} {args} failed (exit {code}): {error}");
        }
    }

    private static void TryRun(string file, string args, string? workingDir = null)
    {
        try
        {
            Run(file, args, workingDir);
        }
        catch
        {
            // best-effort
        }
    }

    private static (string output, string error, int exitCode) Run(string file, string args, string? workingDir = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                CreateNoWindow = true
            }
        };
        if (!string.IsNullOrEmpty(workingDir))
        {
            process.StartInfo.WorkingDirectory = workingDir;
        }

        var (output, error) = process.StartAndWaitForExit(false);
        return (output, error, process.ExitCode);
    }
}
