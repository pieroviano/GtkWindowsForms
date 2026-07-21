using Prepare.Common.Alignment;
using Prepare.Common.IO;
using Prepare.Common.Logging;
using PrepareProjects.Preparation.Compilation;
using PrepareProjects.ProjectHandling;

namespace PrepareProjects;

/// <summary>How <c>fs</c> is being run: as part of the full pipeline, or to produce an alignment tree.</summary>
public sealed class SampleTransformOptions
{
    /// <summary>PrepareProjects' own use: the samples are being regenerated for real, so version churn is wanted.</summary>
    public static SampleTransformOptions Pipeline { get; } = new();

    /// <summary>
    /// Query nuget.org for the newest GtkSharp and rewrite the csproj. Left off under AlignSamples: no
    /// network-dependent step may run, or whatever the feed served that minute enters the delta as if it
    /// were an upstream change.
    /// </summary>
    public bool EnsureLatestGtkSharp { get; init; } = true;

    /// <summary>Optional manifest that records generated-file → source-file as the transform runs.</summary>
    public ProvenanceManifest? Provenance { get; init; }
}

/// <summary>One upstream sample and the generated project it becomes.</summary>
public sealed record SampleProject(string SourceProjectName, string TargetProjectName);

/// <summary>
/// The sample transform <c>fs</c>: the counterpart of <see cref="TransformPipeline"/> (<c>f</c>) for the
/// two upstream-derived sample apps. Extracted so PrepareProjects and AlignSamples run the identical
/// transform — AlignSamples applies it to a staging root at two upstream commits and merges the delta
/// onto the live samples. See AlignSamples-Plan.md Phase 1.
/// </summary>
public static class SampleTransformPipeline
{
    /// <summary>
    /// The samples in scope, in a fixed order (the alignment scope and the delta both depend on this being
    /// stable). <c>AutoClosingMessageBoxApp</c> is fork-only and deliberately absent.
    /// </summary>
    public static readonly IReadOnlyList<SampleProject> Samples = new[]
    {
        new SampleProject("GTKWinFormsApp", "GtkWinFormsApp"),
        new SampleProject("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11")
    };

    /// <summary>Name of the provenance manifest <c>fs</c> writes at the repo root when one is requested.</summary>
    public const string ProvenanceFileName = "provenance.tsv";

    /// <summary>
    /// Runs <c>fs</c> for ONE sample against <paramref name="repoRoot"/>: reads
    /// <c>&lt;repoRoot&gt;/gtksystem-windows-forms/Samples/&lt;source&gt;</c> plus
    /// <c>&lt;repoRoot&gt;/Resources</c>, and writes <c>&lt;repoRoot&gt;/&lt;target&gt;</c>. Returns the
    /// generated csproj path.
    /// </summary>
    /// <remarks>
    /// Isolation is by repo root (AlignProjects-Plan.md §3.1, option B): every path inside the transform
    /// resolves through <see cref="RepoPaths"/>, which is pointed at <paramref name="repoRoot"/> for the
    /// duration and restored afterwards — so a staging root is read and written, never the live tree.
    /// </remarks>
    public static async Task<string> RunSampleTransformAsync(string repoRoot,
        string sourceProjectName, string targetProjectName, SampleTransformOptions? options = null)
    {
        var previousRoot = RepoPaths.Root;
        try
        {
            RepoPaths.Root = Path.GetFullPath(repoRoot);
            var csprojPath = await GtkSampleAppProjectsHandler.PrepareAsync(
                sourceProjectName, targetProjectName, options ?? SampleTransformOptions.Pipeline);

            // The sample's strongly-typed resource accessor is regenerated from the resx that was just
            // overlaid, so the generated Designer tracks the resource file rather than the template.
            ResGenService.RegenerateSampleDesigner(csprojPath);

            // After regeneration the Designer's content comes from the overlaid resx, not from the
            // template it was rendered from - so the manifest points there. Both sides stay repo-relative
            // (the staging root is transient; the driver rebases onto the live repo).
            var designer = Path.Combine(Path.GetDirectoryName(csprojPath)!, "Properties", "GtkWinFormsAppResources.Designer.cs");
            options?.Provenance?.Add(
                Path.GetRelativePath(RepoPaths.Root, designer).Replace('\\', '/'),
                "Resources/GtkWinFormsAppResources.xml");

            return csprojPath;
        }
        finally
        {
            RepoPaths.Root = previousRoot;
        }
    }

    /// <summary>
    /// Runs <c>fs</c> for every sample in <see cref="Samples"/> and returns the provenance manifest for the
    /// whole run, also written to <c>&lt;repoRoot&gt;/provenance.tsv</c>. This is the entry point
    /// AlignSamples hands to the alignment driver as its transform delegate.
    /// </summary>
    public static async Task<ProvenanceManifest> RunAllSampleTransformsAsync(string repoRoot,
        bool ensureLatestGtkSharp = false)
    {
        var provenance = new ProvenanceManifest();
        var options = new SampleTransformOptions
        {
            EnsureLatestGtkSharp = ensureLatestGtkSharp,
            Provenance = provenance
        };

        foreach (var sample in Samples)
        {
            ConsoleColor.Green.Write($@"Preparing sample {sample.TargetProjectName}", true);
            await RunSampleTransformAsync(repoRoot, sample.SourceProjectName, sample.TargetProjectName, options);
        }

        provenance.Save(Path.Combine(Path.GetFullPath(repoRoot), ProvenanceFileName));
        return provenance;
    }
}
