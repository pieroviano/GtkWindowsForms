using System.Collections.Concurrent;

namespace Prepare.Common.Alignment;

/// <summary>
/// Maps a conflicted repo-relative path (a path in <c>OURS</c>, the fork's layout) to the corresponding
/// file inside a transformed tree (<c>build/base</c> or <c>build/theirs</c>).
///
/// The two layouts are not identical: the fork's patches move and rename files, so where git
/// rename-detects during the cherry-pick it reports the conflict under the <b>fork's</b> path while the
/// tree still holds the transform's path (e.g. fork <c>System/Drawing/Bitmap.cs</c> vs tree
/// <c>System/Drawing/Drawing2D/Bitmap.cs</c>). Joining the conflicted path onto the tree root therefore
/// yields a non-existent file — which both blanks the operator's diff references and makes
/// <see cref="DeltaMerge.IncorporateUpstreamInto"/> bail out, misreporting an ordinary content conflict
/// as "a patch moved/deleted this path".
///
/// Resolution order: exact relative path, then a unique same-filename match inside the same top-level
/// project, then a unique same-filename match anywhere in the tree. Ambiguous (several candidates) and
/// absent files return null — the caller then falls back to the by-hand branch rather than guessing.
/// </summary>
public static class TreePathResolver
{
    /// <summary>Per-tree filename → relative paths index, built once per tree root (625 lookups per run).</summary>
    private static readonly ConcurrentDictionary<string, Dictionary<string, List<string>>> Indexes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the full path of <paramref name="relativePath"/> inside <paramref name="treeRoot"/>,
    /// following renames, or null when it cannot be resolved unambiguously.
    /// </summary>
    public static string? Resolve(string treeRoot, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(treeRoot) || !Directory.Exists(treeRoot))
        {
            return null;
        }

        var rel = relativePath.Replace('\\', '/');
        var direct = Path.Combine(treeRoot, rel.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(direct))
        {
            return Path.GetFullPath(direct);
        }

        var index = Indexes.GetOrAdd(Path.GetFullPath(treeRoot), BuildIndex);
        if (!index.TryGetValue(Path.GetFileName(rel), out var candidates))
        {
            return null;
        }

        // Prefer a match inside the same generated project before considering the whole tree: a name
        // like ResourceManager.cs can legitimately exist in both projects.
        var project = rel.Split('/')[0];
        var sameProject = candidates
            .Where(c => c.StartsWith(project + "/", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var chosen = sameProject.Count > 0 ? sameProject : candidates;
        return chosen.Count == 1
            ? Path.GetFullPath(Path.Combine(treeRoot, chosen[0].Replace('/', Path.DirectorySeparatorChar)))
            : null;
    }

    /// <summary>Clears the cached indexes (tests, or trees regenerated within one process).</summary>
    public static void ClearCache() => Indexes.Clear();

    private static Dictionary<string, List<string>> BuildIndex(string treeRoot)
    {
        var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(treeRoot, "*", SearchOption.AllDirectories))
        {
            var rel = file.Substring(treeRoot.Length).TrimStart('\\', '/').Replace('\\', '/');

            // Captured provenance manifests are metadata, not part of either layout.
            if (rel.StartsWith("_manifests/", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var name = Path.GetFileName(rel);
            if (!index.TryGetValue(name, out var list))
            {
                index[name] = list = new List<string>();
            }

            list.Add(rel);
        }

        return index;
    }
}
