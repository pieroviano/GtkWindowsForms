namespace Prepare.Common.IO;

/// <summary>
/// Resolves repo-root-relative paths for the transform. <see cref="Root"/> defaults to the
/// conventional location — 4 levels up from the executing assembly, i.e. the repo root — which
/// reproduces the pre-refactor behavior (the scattered <c>..\..\..\..</c> literals resolved from the
/// exe's <c>bin</c> dir). AlignProjects overrides <see cref="Root"/> per staging root so the same
/// transform runs in isolation without touching the live repo (AlignProjects-Plan.md §3.1 option B).
/// </summary>
public static class RepoPaths
{
    private static string? _root;

    /// <summary>The repo root the transform reads/writes under. Assigning null restores the default.</summary>
    public static string Root
    {
        get => _root ??= Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        set => _root = value == null ? null : Path.GetFullPath(value);
    }

    /// <summary>Full path of <paramref name="relative"/> resolved under <see cref="Root"/>.</summary>
    public static string At(string relative) => Path.GetFullPath(Path.Combine(Root, relative));
}
