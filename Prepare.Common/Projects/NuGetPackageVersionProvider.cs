using System.Net;
using System.Text.Json;

namespace Prepare.Common.Projects;

public static class NuGetPackageVersionProvider
{
    private static readonly HttpClient HttpClient =
        new(new HttpClientHandler
        {
            AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate
        });

    public static Version? LatestVersion { get; set; }

    public static bool Called { get; set; }

    public static async Task<Version?> GetLatestStableGtkSharpVersionAsync()
    {
        if (Called)
        {
            return LatestVersion;
        }

        const string url =
            "https://api.nuget.org/v3/registration5-gz-semver2/gtksharp/index.json";

        var stream = await HttpClient.GetStringAsync(url);
        using var doc = JsonDocument.Parse(stream);

        LatestVersion = null;

        var root = doc.RootElement;

        if (!root.TryGetProperty("items", out var pages))
        {
            return null;
        }

        foreach (var page in pages.EnumerateArray())
        {
            if (!page.TryGetProperty("items", out var items))
            {
                continue;
            }

            foreach (var item in items.EnumerateArray())
            {
                if (!item.TryGetProperty("catalogEntry", out var entry))
                {
                    continue;
                }

                if (!entry.TryGetProperty("version", out var versionElement))
                {
                    continue;
                }

                var versionText = versionElement.GetString();
                if (string.IsNullOrWhiteSpace(versionText))
                {
                    continue;
                }

                // Skip prerelease versions
                if (versionText.Contains('-', StringComparison.Ordinal))
                {
                    continue;
                }

                if (!Version.TryParse(versionText, out var version))
                {
                    continue;
                }

                if (LatestVersion == null || version > LatestVersion)
                {
                    LatestVersion = version;
                }
            }
        }

        Called = true;
        return LatestVersion;
    }
}