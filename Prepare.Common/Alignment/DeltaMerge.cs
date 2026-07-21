using System.Diagnostics;
using Prepare.Common.Projects;

namespace Prepare.Common.Alignment;

/// <summary>Result of a <see cref="DeltaMerge.Apply"/> call.</summary>
public sealed class DeltaMergeResult
{
    public DeltaMergeResult(bool clean, IReadOnlyList<string> conflicts)
    {
        Clean = clean;
        Conflicts = conflicts;
    }

    /// <summary>True when the delta applied with no conflicts (cherry-pick committed, or delta was empty).</summary>
    public bool Clean { get; }

    /// <summary>Repo-relative paths (forward slashes) of files left unmerged; empty when <see cref="Clean"/>.</summary>
    public IReadOnlyList<string> Conflicts { get; }
}

/// <summary>
/// Applies the transformed-space upstream delta <c>(THEIRS − BASE)</c> onto the live working tree
/// (<c>OURS</c>) via git's real 3-way machinery (AlignProjects-Plan.md §3.5):
/// stage <c>BASE</c> and <c>THEIRS</c> as throwaway commits (<c>align/base</c>, <c>align/theirs</c>)
/// so the base blobs exist, then <c>git cherry-pick align/theirs</c> — yielding
/// <c>OURS + (THEIRS − BASE)</c>. Conflicts are left in the working tree for the operator to resolve
/// (then <see cref="Continue"/>); a clean pick is committed automatically. Only the given
/// <c>scopePaths</c> are ever replaced.
/// </summary>
public sealed class DeltaMerge
{
    private readonly string _repoDir;
    private readonly string[] _scope;
    private readonly string _baseBranch;
    private readonly string _theirsBranch;
    private string _originalBranch = string.Empty;

    /// <param name="repoDir">The repository whose working tree holds <c>OURS</c>.</param>
    /// <param name="scopePaths">Repo-relative directories the delta may replace.</param>
    /// <param name="branchPrefix">
    /// Prefix of the throwaway branches (<c>&lt;prefix&gt;/base</c>, <c>&lt;prefix&gt;/theirs</c>). Each
    /// alignment tool uses its own so a pending run of one never collides with the other.
    /// </param>
    public DeltaMerge(string repoDir, IEnumerable<string> scopePaths, string branchPrefix = "align")
    {
        _repoDir = Path.GetFullPath(repoDir);
        _scope = scopePaths.ToArray();
        _baseBranch = branchPrefix + "/base";
        _theirsBranch = branchPrefix + "/theirs";
    }

    /// <summary>
    /// Builds the throwaway commits from <paramref name="baseDir"/> / <paramref name="theirsDir"/>
    /// (each containing the scope subtrees) and cherry-picks the delta onto the current branch.
    /// Precondition: a clean working tree on a real branch.
    /// </summary>
    public DeltaMergeResult Apply(string baseDir, string theirsDir)
    {
        _originalBranch = Git("rev-parse --abbrev-ref HEAD").output.Trim();
        DeleteBranch(_baseBranch);
        DeleteBranch(_theirsBranch);

        // align/base = OURS HEAD with the scope replaced by BASE.
        GitOrThrow($"checkout -b {_baseBranch}");
        ReplaceScope(baseDir);
        StageScope();
        GitOrThrow("commit --allow-empty --no-verify -m \"align base\"");

        // align/theirs = align/base with the scope replaced by THEIRS (diff vs base = pure upstream delta).
        GitOrThrow($"checkout -b {_theirsBranch}");
        ReplaceScope(theirsDir);
        StageScope();
        GitOrThrow("commit --allow-empty --no-verify -m \"align theirs\"");

        // Back to OURS.
        GitOrThrow($"checkout {_originalBranch}");

        // Empty delta (base == theirs in scope) => nothing to align; clean no-op.
        if (Git($"diff --quiet {_baseBranch} {_theirsBranch}").exitCode == 0)
        {
            Cleanup();
            return new DeltaMergeResult(true, Array.Empty<string>());
        }

        if (Git($"cherry-pick {_theirsBranch}").exitCode == 0)
        {
            Cleanup();
            return new DeltaMergeResult(true, Array.Empty<string>());
        }

        // Conflicts: leave the cherry-pick in progress (temp branches kept) for the operator to resolve.
        var conflicts = Git("diff --name-only --diff-filter=U").output
            .Split('\n')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();
        return new DeltaMergeResult(false, conflicts);
    }

    /// <summary>
    /// Incorporates the upstream delta (<paramref name="baseFile"/> → <paramref name="theirsFile"/>)
    /// into the live file at <paramref name="relativePath"/> as a 3-way merge with the fork's version
    /// (git stage :2:), leaving conflict markers only where they overlap. The file stays unmerged in
    /// the index (the operator resolves the markers, then <c>git add</c>). Returns false when the fork
    /// side is absent (the file was moved/deleted by a patch) so the caller can point at the fork copy.
    /// </summary>
    public bool IncorporateUpstreamInto(string relativePath, string baseFile, string theirsFile)
    {
        var rel = relativePath.Replace('\\', '/');
        if (!File.Exists(baseFile) || !File.Exists(theirsFile))
        {
            return false;
        }

        var (ours, code) = GitRaw($"show :2:{rel}");
        if (code != 0)
        {
            return false; // no fork (stage 2) version - moved/deleted by a patch
        }

        var tmp = Path.Combine(Path.GetTempPath(), "align-mf-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        try
        {
            // All three inputs MUST share line endings. `git show` emits the normalized (LF) blob while
            // the transformed trees are CRLF on disk; mixing them makes every line differ and collapses
            // the merge into one whole-file conflict. Normalize to LF, merge, then restore the target's
            // own convention.
            var oursTmp = Path.Combine(tmp, "ours");
            var baseTmp = Path.Combine(tmp, "base");
            var theirsTmp = Path.Combine(tmp, "theirs");
            var target = Path.Combine(_repoDir, rel.Replace('/', Path.DirectorySeparatorChar));
            var useCrLf = UsesCrLf(baseFile);

            File.WriteAllText(oursTmp, ToLf(ours));
            File.WriteAllText(baseTmp, ToLf(File.ReadAllText(baseFile)));
            File.WriteAllText(theirsTmp, ToLf(File.ReadAllText(theirsFile)));

            // 3-way merge in place: ours + (theirs - base), conflict markers only where they overlap.
            Git($"merge-file -L \"OURS (fork)\" -L \"base\" -L \"THEIRS (upstream)\" \"{oursTmp}\" \"{baseTmp}\" \"{theirsTmp}\"");

            var merged = File.ReadAllText(oursTmp);
            File.WriteAllText(target, useCrLf ? ToCrLf(merged) : merged);
            return true;
        }
        finally
        {
            try { Directory.Delete(tmp, true); } catch { /* best-effort */ }
        }
    }

    /// <summary>
    /// For a file a patch moved: merges the upstream delta (<paramref name="baseFile"/> →
    /// <paramref name="theirsFile"/>) into the fork's actual copy at <paramref name="forkCopyFullPath"/>,
    /// then <c>git rm</c>s the spurious old path <paramref name="oldRelativePath"/> (resolving that
    /// conflict). Returns true if the fork copy still has conflict markers (caller pauses, then
    /// <see cref="StageFile"/>); false if the merge was clean (already staged).
    /// </summary>
    public bool IncorporateIntoForkCopy(string oldRelativePath, string forkCopyFullPath, string baseFile, string theirsFile)
    {
        var oldRel = oldRelativePath.Replace('\\', '/');
        var tmp = Path.Combine(Path.GetTempPath(), "align-mf-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmp);
        int markers;
        try
        {
            // Same line-ending normalization as IncorporateUpstreamInto - see the note there.
            var oursTmp = Path.Combine(tmp, "ours");
            var baseTmp = Path.Combine(tmp, "base");
            var theirsTmp = Path.Combine(tmp, "theirs");
            var useCrLf = UsesCrLf(forkCopyFullPath);

            File.WriteAllText(oursTmp, ToLf(File.ReadAllText(forkCopyFullPath)));
            File.WriteAllText(baseTmp, ToLf(File.ReadAllText(baseFile)));
            File.WriteAllText(theirsTmp, ToLf(File.ReadAllText(theirsFile)));

            markers = Git($"merge-file -L \"OURS (fork)\" -L \"base\" -L \"THEIRS (upstream)\" \"{oursTmp}\" \"{baseTmp}\" \"{theirsTmp}\"").exitCode;

            var merged = File.ReadAllText(oursTmp);
            File.WriteAllText(forkCopyFullPath, useCrLf ? ToCrLf(merged) : merged);
        }
        finally
        {
            try { Directory.Delete(tmp, true); } catch { /* best-effort */ }
        }

        // Remove the spurious old-path file and resolve the cherry-pick conflict there.
        Git($"rm -f -- {oldRel}");

        if (markers <= 0)
        {
            StageFile(forkCopyFullPath); // clean merge -> stage the fork copy
            return false;
        }

        return true; // markers remain; caller resolves, then stages
    }

    /// <summary>Collapses CRLF to LF so the three merge inputs are comparable line by line.</summary>
    private static string ToLf(string text) => text.Replace("\r\n", "\n");

    /// <summary>Restores CRLF on a merged result whose target uses Windows line endings.</summary>
    private static string ToCrLf(string text) => ToLf(text).Replace("\n", "\r\n");

    /// <summary>True when the file's first line break is CRLF (its prevailing convention).</summary>
    private static bool UsesCrLf(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        var text = File.ReadAllText(path);
        var lf = text.IndexOf('\n');
        return lf > 0 && text[lf - 1] == '\r';
    }

    /// <summary>
    /// True when the fork still has its own version of the path (git stage :2:) — i.e. an ordinary
    /// content conflict rather than a path a patch moved or deleted. Lets the caller report the real
    /// reason it could not merge automatically.
    /// </summary>
    public bool HasForkSide(string relativePath) =>
        GitRaw($"show :2:{relativePath.Replace('\\', '/')}").exitCode == 0;

    /// <summary>Stages a resolved file (by absolute path) in the index.</summary>
    public void StageFile(string fullPath)
    {
        var rel = Path.GetFullPath(fullPath).Substring(_repoDir.Length).TrimStart('\\', '/').Replace('\\', '/');
        Git($"add -- \"{rel}\"");
    }

    /// <summary>Repo-relative paths of files currently unmerged (used to re-check partial resolutions).</summary>
    public IReadOnlyList<string> CurrentConflicts() =>
        Git("diff --name-only --diff-filter=U").output
            .Split('\n')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();

    /// <summary>Finalizes the cherry-pick after the operator resolved and staged the conflicts.</summary>
    public void Continue()
    {
        GitOrThrow("cherry-pick --continue");
        Cleanup();
    }

    /// <summary>Aborts the in-progress cherry-pick and restores the pre-merge state.</summary>
    public void Abort()
    {
        Git("cherry-pick --abort");
        Cleanup();
    }

    /// <summary>Returns to the original branch and deletes the throwaway branches.</summary>
    public void Cleanup()
    {
        if (!string.IsNullOrEmpty(_originalBranch))
        {
            Git($"checkout {_originalBranch}");
        }

        DeleteBranch(_baseBranch);
        DeleteBranch(_theirsBranch);
    }

    private void ReplaceScope(string sourceRoot)
    {
        foreach (var rel in _scope)
        {
            var target = Path.Combine(_repoDir, rel);
            if (Directory.Exists(target))
            {
                Directory.Delete(target, true);
            }

            var source = Path.Combine(sourceRoot, rel);
            if (Directory.Exists(source))
            {
                CopyDirectory(source, target);
            }
        }
    }

    private void StageScope()
    {
        var quoted = string.Join(" ", _scope.Select(p => "\"" + p + "\""));
        GitOrThrow("add -A -- " + quoted);
    }

    private void DeleteBranch(string name) => Git($"branch -D {name}");

    private static void CopyDirectory(string source, string dest)
    {
        var src = Path.GetFullPath(source);
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

    // Raw stdout capture (no line-ending normalization), for reading exact blob content.
    private (string output, int exitCode) GitRaw(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = _repoDir,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            }
        };
        process.StartInfo.Environment["GIT_EDITOR"] = "true";
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (output, process.ExitCode);
    }

    private (string output, string error, int exitCode) Git(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                WorkingDirectory = _repoDir,
                CreateNoWindow = true
            }
        };
        // Never block on an editor (cherry-pick --continue) or a credential prompt.
        process.StartInfo.Environment["GIT_EDITOR"] = "true";
        process.StartInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";

        var (output, error) = process.StartAndWaitForExit(false);
        return (output, error, process.ExitCode);
    }

    private void GitOrThrow(string args)
    {
        var result = Git(args);
        if (result.exitCode != 0)
        {
            throw new InvalidOperationException($"git {args} failed (exit {result.exitCode}): {result.error}");
        }
    }
}
