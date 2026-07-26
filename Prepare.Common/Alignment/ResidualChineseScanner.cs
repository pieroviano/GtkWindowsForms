using System.Text.RegularExpressions;

namespace Prepare.Common.Alignment;

/// <summary>
/// Post-merge check (§3.7): scans the aligned <c>Gtk.*</c> for residual Chinese (`[一-鿿]`) — a
/// translation gap left by an unfilled blank entry (protected by the apply guard, so the Chinese
/// stays visible rather than being erased). Returns each offending file and its match count.
/// </summary>
public static class ResidualChineseScanner
{
    private static readonly Regex Chinese = new(@"[一-鿿]", RegexOptions.Compiled);

    public static IReadOnlyList<KeyValuePair<string, int>> Scan(params string[] dirs)
    {
        var results = new List<KeyValuePair<string, int>>();
        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
            {
                var normalized = file.Replace('/', '\\');
                if (normalized.Contains("\\bin\\") || normalized.Contains("\\obj\\"))
                {
                    continue;
                }

                var count = Chinese.Matches(File.ReadAllText(file)).Count;
                if (count > 0)
                {
                    results.Add(new KeyValuePair<string, int>(file, count));
                }
            }
        }

        return results;
    }
}
