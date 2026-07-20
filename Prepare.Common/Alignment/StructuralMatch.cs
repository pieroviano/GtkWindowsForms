namespace Prepare.Common.Alignment;

/// <summary>Result of a <see cref="StructuralMatch"/> comparison.</summary>
public sealed class StructuralMatchResult
{
    public StructuralMatchResult(IReadOnlyList<string> onlyInBase, IReadOnlyList<string> onlyInOurs)
    {
        OnlyInBase = onlyInBase;
        OnlyInOurs = onlyInOurs;
    }

    /// <summary>Relative paths present in the BASE tree but not in OURS (would be added as orphans).</summary>
    public IReadOnlyList<string> OnlyInBase { get; }

    /// <summary>Relative paths present in OURS but not in the BASE tree (would be left dangling).</summary>
    public IReadOnlyList<string> OnlyInOurs { get; }

    /// <summary>True when both trees hold exactly the same set of file paths (layout matches).</summary>
    public bool Matches => OnlyInBase.Count == 0 && OnlyInOurs.Count == 0;
}

/// <summary>
/// Preflight guard for the 3-way merge: <c>f(base)</c> must have the <b>same set of file paths</b> as
/// <c>OURS</c> (the live Net4x.*), differing only <i>inside</i> files. If the transform's layout has
/// drifted (files added/removed/renamed), a cherry-pick would create duplicates/orphans instead of
/// merging — so the run must abort. See <c>AlignProjects-Plan.md</c> §3.4.
/// </summary>
public static class StructuralMatch
{
    /// <summary>Compares the file-path sets of the BASE tree and the OURS tree (recursively).</summary>
    public static StructuralMatchResult Compare(string baseDir, string oursDir)
    {
        var baseSet = RelativeFiles(baseDir);
        var oursSet = RelativeFiles(oursDir);

        var onlyInBase = baseSet.Where(p => !oursSet.Contains(p))
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
        var onlyInOurs = oursSet.Where(p => !baseSet.Contains(p))
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();

        return new StructuralMatchResult(onlyInBase, onlyInOurs);
    }

    private static HashSet<string> RelativeFiles(string root)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var full = Path.GetFullPath(root);
        if (!Directory.Exists(full))
        {
            return set;
        }

        var prefixLength = full.Length + (full.EndsWith(Path.DirectorySeparatorChar.ToString()) ? 0 : 1);
        foreach (var file in Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories))
        {
            var rel = file.Substring(prefixLength).Replace('\\', '/');
            // Ignore build artifacts - they are never part of the source layout comparison.
            if (rel.StartsWith("obj/") || rel.Contains("/obj/") || rel.StartsWith("bin/") || rel.Contains("/bin/"))
            {
                continue;
            }

            set.Add(rel);
        }

        return set;
    }
}
