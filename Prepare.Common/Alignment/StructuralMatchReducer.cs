namespace Prepare.Common.Alignment;

/// <summary>Residual structural differences after removing those that patches legitimately explain.</summary>
public sealed class StructuralMatchReduction
{
    public StructuralMatchReduction(IReadOnlyList<string> baseOnly, IReadOnlyList<string> oursOnly)
    {
        BaseOnly = baseOnly;
        OursOnly = oursOnly;
    }

    public IReadOnlyList<string> BaseOnly { get; }
    public IReadOnlyList<string> OursOnly { get; }
    public bool HasResidual => BaseOnly.Count > 0 || OursOnly.Count > 0;
}

/// <summary>
/// Reduces a <see cref="StructuralMatchResult"/> by dropping differences that the patch series
/// legitimately produces (since <c>OURS = f(base) + patches</c>): a file that simply <b>moved</b>
/// (same filename in another directory), the <c>Net4x.</c> prefix on <c>.props</c>/<c>.targets</c>
/// (<b>rename</b>), and file <b>splits</b> (<c>X.cs</c> ↔ <c>X.*.cs</c>). What remains is the
/// likely-genuine residual (patch deletions, committed settings) — reported, never fatal.
/// </summary>
public static class StructuralMatchReducer
{
    public static StructuralMatchReduction Reduce(StructuralMatchResult match)
    {
        var oursNames = new HashSet<string>(match.OnlyInOurs.Select(NormName), StringComparer.OrdinalIgnoreCase);
        var baseNames = new HashSet<string>(match.OnlyInBase.Select(NormName), StringComparer.OrdinalIgnoreCase);
        var oursStems = new HashSet<string>(match.OnlyInOurs.Select(FirstSegment), StringComparer.OrdinalIgnoreCase);
        var baseStems = new HashSet<string>(match.OnlyInBase.Select(StemWithoutExtension), StringComparer.OrdinalIgnoreCase);

        var baseOnly = match.OnlyInBase
            .Where(p => !oursNames.Contains(NormName(p))                    // moved / prefix-renamed
                     && !oursStems.Contains(StemWithoutExtension(NormName(p)))) // split: X.cs -> X.<seg>.cs
            .ToList();

        var oursOnly = match.OnlyInOurs
            .Where(p => !baseNames.Contains(NormName(p))                    // moved / prefix-renamed
                     && !baseStems.Contains(FirstSegment(p)))              // split: X.<seg>.cs -> X.cs
            .ToList();

        return new StructuralMatchReduction(baseOnly, oursOnly);
    }

    /// <summary>Filename with the <c>Net4x.</c> prefix stripped from .props/.targets.</summary>
    private static string NormName(string relPath)
    {
        var name = Path.GetFileName(relPath);
        if ((name.EndsWith(".props", StringComparison.OrdinalIgnoreCase) ||
             name.EndsWith(".targets", StringComparison.OrdinalIgnoreCase)) &&
            name.StartsWith("Net4x.", StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring("Net4x.".Length);
        }

        return name;
    }

    private static string StemWithoutExtension(string relPath) =>
        Path.GetFileNameWithoutExtension(Path.GetFileName(relPath));

    private static string FirstSegment(string relPath) => Path.GetFileName(relPath).Split('.')[0];
}
