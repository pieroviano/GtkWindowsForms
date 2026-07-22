namespace GtkWinFormsApp.UITests.Infrastructure;

/// <summary>
/// Resolves the on-disk locations the screenshot oracle needs. Baselines live in the project SOURCE tree
/// (they are committed and must be writable by the update command), while failure artifacts go under the
/// test OUTPUT dir. Walking up from <see cref="AppContext.BaseDirectory"/> to the csproj works because
/// these tests are dev-machine-only (SampleUITests-Plan.md §9), so the source tree is always present.
/// </summary>
internal static class TestPaths
{
    private const string ProjectFileName = "GtkWinFormsApp.UITests.csproj";

    internal static string ProjectDir { get; } = FindProjectDir();

    /// <summary>Committed golden images: <c>Baselines/&lt;Form&gt;/&lt;checkpoint&gt;.png</c>.</summary>
    internal static string BaselinesDir => Path.Combine(ProjectDir, "Baselines");

    /// <summary>Where actual/diff PNGs are written when a comparison fails.</summary>
    internal static string ArtifactsDir => Path.Combine(AppContext.BaseDirectory, "TestResults", "Screenshots");

    private static string FindProjectDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, ProjectFileName)))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? AppContext.BaseDirectory;
    }
}
