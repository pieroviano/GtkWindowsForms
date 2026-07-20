using System.Text;

namespace Prepare.Common.Alignment;

/// <summary>Full paths describing one merge-conflicted file, for the operator to resolve by hand.</summary>
public sealed class ConflictInfo
{
    public ConflictInfo(string relativePath, string net4xFullPath, string? sourceFullPath,
        string theirsRefFullPath, string baseRefFullPath)
    {
        RelativePath = relativePath;
        Net4xFullPath = net4xFullPath;
        SourceFullPath = sourceFullPath;
        TheirsRefFullPath = theirsRefFullPath;
        BaseRefFullPath = baseRefFullPath;
    }

    /// <summary>Repo-relative path (forward slashes) of the conflicted file.</summary>
    public string RelativePath { get; }

    /// <summary>The conflicted file in the live working tree — where the operator applies the change.</summary>
    public string Net4xFullPath { get; }

    /// <summary>The raw <c>gtksystem-windows-forms</c> source (from the provenance manifest), or null if unresolved.</summary>
    public string? SourceFullPath { get; }

    /// <summary>The new upstream version already in <c>Net4x.*</c> layout (<c>build/theirs</c>).</summary>
    public string TheirsRefFullPath { get; }

    /// <summary>The previous baseline in <c>Net4x.*</c> layout (<c>build/base</c>).</summary>
    public string BaseRefFullPath { get; }
}

/// <summary>
/// Builds operator-facing conflict descriptions for the §3.6 pause: for each unmerged file it resolves
/// the live <c>Net4x.*</c> path, the exact raw upstream source (via the provenance manifest), and the
/// <c>build/theirs</c> / <c>build/base</c> reference paths. See AlignProjects-Plan.md §3.6.
/// </summary>
public static class ConflictReporter
{
    public static IReadOnlyList<ConflictInfo> Describe(
        string repoDir, string baseTreeDir, string theirsTreeDir,
        IReadOnlyDictionary<string, string>? provenance,
        IEnumerable<string> conflictRelativePaths)
    {
        var list = new List<ConflictInfo>();
        foreach (var rel in conflictRelativePaths)
        {
            var norm = rel.Replace('\\', '/');
            var osRel = norm.Replace('/', Path.DirectorySeparatorChar);

            string? source = null;
            if (provenance != null && provenance.TryGetValue(norm, out var s))
            {
                source = s;
            }

            // Rename-aware: the fork's layout differs from the transform's wherever a patch moved a file,
            // and git reports the conflict under the FORK path. Joining it onto the tree root would point
            // at a file that does not exist. Fall back to the naive join only so the printed path shows
            // what was looked for (FormatOne marks it "not found").
            var theirsRef = TreePathResolver.Resolve(theirsTreeDir, norm)
                            ?? Path.GetFullPath(Path.Combine(theirsTreeDir, osRel));
            var baseRef = TreePathResolver.Resolve(baseTreeDir, norm)
                          ?? Path.GetFullPath(Path.Combine(baseTreeDir, osRel));

            list.Add(new ConflictInfo(norm, Path.GetFullPath(Path.Combine(repoDir, osRel)), source, theirsRef, baseRef));
        }

        return list;
    }

    /// <summary>
    /// Renders a single conflict's paths (the block shown for one file at the pause). The two
    /// <b>transformed</b> trees are listed first and labelled as the diffed pair, because the delta git
    /// tried to apply is exactly <c>base -> theirs</c> between them — both already formatted, split,
    /// translated and reorganized like the fork. The raw <c>gtksystem-windows-forms</c> file is shown
    /// last and marked as context only: it is unformatted and in the pre-transform layout, so hunks
    /// cannot be reconstructed from it.
    /// </summary>
    public static string FormatOne(ConflictInfo c)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  * {c.RelativePath}");
        sb.AppendLine($"      edit here (fork)          : {c.Net4xFullPath}");
        sb.AppendLine($"      DIFFED base   (formatted) : {c.BaseRefFullPath}{Missing(c.BaseRefFullPath)}");
        sb.AppendLine($"      DIFFED theirs (formatted) : {c.TheirsRefFullPath}{Missing(c.TheirsRefFullPath)}");
        sb.AppendLine($"      ^ the delta to apply is exactly: base -> theirs (diff these two)");
        sb.AppendLine($"      raw upstream (context only, unformatted/pre-transform layout):");
        sb.AppendLine($"                                  {c.SourceFullPath ?? "(unresolved - use the diffed pair above)"}");
        return sb.ToString();
    }

    /// <summary>Flags a reference path that could not be located in its tree, so it is never read as usable.</summary>
    private static string Missing(string path) => File.Exists(path) ? string.Empty : "   [NOT FOUND in tree]";

    /// <summary>Renders the whole conflict list (used for the non-interactive summary).</summary>
    public static string FormatNotice(IReadOnlyList<ConflictInfo> conflicts)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{conflicts.Count} file(s) could not be merged automatically and need manual resolution:");
        foreach (var c in conflicts)
        {
            sb.AppendLine();
            sb.Append(FormatOne(c));
        }

        return sb.ToString();
    }
}
