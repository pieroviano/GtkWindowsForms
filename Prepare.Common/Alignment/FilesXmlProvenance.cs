using System.Xml.Linq;

namespace Prepare.Common.Alignment;

/// <summary>
/// Builds the generated→source provenance map from the <c>*.Files.xml</c> manifests the transform
/// already emits (<c>ProjectUtility.PersistProjectFiles</c>), so the conflict pause can name the raw
/// upstream file without waiting on the purpose-built <c>provenance.tsv</c> (AlignProjects-Plan.md §3.1).
///
/// Each manifest entry is <c>&lt;Item Source="GtkControls\ListView.cs"
/// Destination="System\Windows\Forms\ListView.cs"&gt;</c>, where <c>Source</c> is relative to the upstream
/// project inside the submodule and <c>Destination</c> is relative to the generated project. This class
/// re-keys them to repo-relative generated paths (<c>Gtk.Windows.Forms/System/Windows/Forms/ListView.cs</c>)
/// and resolves <c>Source</c> against the live submodule.
///
/// Two deliberate limitations: entries whose resolved source does not exist on disk are dropped (this
/// filters the <c>Gtk.Drawing</c> manifest, whose <c>Source == Destination</c> identity rows carry
/// no upstream provenance), and files produced by <c>TypeSplitService</c> are absent because the split
/// step records nothing — for those the transformed <c>DIFFED base/theirs</c> pair remains the reference.
/// </summary>
public static class FilesXmlProvenance
{
    /// <summary>
    /// Loads every <c>*Files.xml</c> in <paramref name="manifestsDirectory"/> and returns
    /// generated-relative-path (forward slashes) → raw source full path, resolving sources against
    /// <paramref name="upstreamProjectDirectory"/>. Later manifests win, so pass a directory whose
    /// <c>LatestFiles</c>/<c>IntermediateFiles</c> variants reflect the final layout. Returns an empty
    /// map when the directory is missing.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Load(string manifestsDirectory, string upstreamProjectDirectory)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(manifestsDirectory))
        {
            return map;
        }

        // Files -> IntermediateFiles -> LatestFiles: read in that order so the most advanced layout wins.
        foreach (var manifest in Directory.GetFiles(manifestsDirectory, "*Files.xml").OrderBy(RankOf).ThenBy(f => f))
        {
            var projectName = ProjectNameOf(manifest);
            if (projectName == null)
            {
                continue;
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(manifest);
            }
            catch
            {
                continue; // a malformed manifest must not break the conflict report
            }

            foreach (var item in doc.Root?.Elements("Item") ?? Enumerable.Empty<XElement>())
            {
                var source = item.Attribute("Source")?.Value;
                var destination = item.Attribute("Destination")?.Value;
                if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
                {
                    continue;
                }

                var sourceFullPath = Path.GetFullPath(Path.Combine(upstreamProjectDirectory,
                    source!.Replace('/', Path.DirectorySeparatorChar)));

                // Identity/derived rows (e.g. the Drawing manifest) point at files that never existed
                // upstream - drop them rather than print a path the operator cannot open.
                if (!File.Exists(sourceFullPath))
                {
                    continue;
                }

                var key = projectName + "/" + destination!.Replace('\\', '/');
                map[key] = sourceFullPath;
            }
        }

        return map;
    }

    /// <summary>The generated project a manifest describes: <c>Gtk.Drawing.LatestFiles.xml</c> → <c>Gtk.Drawing</c>.</summary>
    private static string? ProjectNameOf(string manifestPath)
    {
        var name = Path.GetFileNameWithoutExtension(manifestPath); // e.g. Gtk.Drawing.LatestFiles
        var lastDot = name.LastIndexOf('.');
        return lastDot <= 0 ? null : name.Substring(0, lastDot);
    }

    private static int RankOf(string manifestPath)
    {
        var name = Path.GetFileNameWithoutExtension(manifestPath);
        if (name.EndsWith(".LatestFiles", StringComparison.OrdinalIgnoreCase))
        {
            return 2;
        }

        return name.EndsWith(".IntermediateFiles", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
    }
}
