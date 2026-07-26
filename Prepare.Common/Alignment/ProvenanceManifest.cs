using System.Text;

namespace Prepare.Common.Alignment;

/// <summary>
/// Maps each generated <c>Gtk.*</c> file (repo-relative, forward slashes) to the raw
/// <c>gtksystem-windows-forms</c> source file it came from — recorded by the transform as it
/// moves/renames/splits files (AlignProjects-Plan.md §3.1), and read by the conflict pause (§3.6) to
/// point the operator at the exact upstream source. Stored as tab-separated lines
/// (<c>generated&lt;TAB&gt;source</c>), UTF-8, no BOM.
/// </summary>
public sealed class ProvenanceManifest
{
    private readonly Dictionary<string, string> _map = new(StringComparer.Ordinal);

    /// <summary>The recorded mappings (generated relative path → source full path).</summary>
    public IReadOnlyDictionary<string, string> Entries => _map;

    /// <summary>Records (or overwrites) the source a generated file came from.</summary>
    public void Add(string generatedRelativePath, string sourceFullPath) =>
        _map[Normalize(generatedRelativePath)] = sourceFullPath;

    /// <summary>
    /// Drops a mapping - for a generated file the transform later deletes or renames away, so the
    /// manifest never points the operator at a path that no longer exists.
    /// </summary>
    public void Remove(string generatedRelativePath) => _map.Remove(Normalize(generatedRelativePath));

    /// <summary>
    /// Writes the manifest to <paramref name="path"/> (creating the directory). Entries are ordinal-sorted:
    /// the manifest travels with the transformed tree, and the determinism self-check compares those trees
    /// byte for byte - insertion order would make an otherwise reproducible run look non-deterministic.
    /// </summary>
    public void Save(string path)
    {
        var sb = new StringBuilder();
        foreach (var kv in _map.OrderBy(e => e.Key, StringComparer.Ordinal))
        {
            sb.Append(kv.Key).Append('\t').Append(kv.Value).Append('\n');
        }

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
    }

    /// <summary>Loads a manifest; returns an empty map when the path is null/missing.</summary>
    public static IReadOnlyDictionary<string, string> Load(string? path)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return map;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            var idx = line.IndexOf('\t');
            if (idx <= 0)
            {
                continue;
            }

            var generated = Normalize(line.Substring(0, idx));
            if (generated.Length > 0)
            {
                map[generated] = line.Substring(idx + 1);
            }
        }

        return map;
    }

    private static string Normalize(string path) => path.Replace('\\', '/').Trim();
}
