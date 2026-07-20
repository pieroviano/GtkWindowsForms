namespace Prepare.Common.Alignment;

/// <summary>Content-aware comparison of two directory trees.</summary>
public sealed class TreeDiffResult
{
    public TreeDiffResult(IReadOnlyList<string> onlyInLeft, IReadOnlyList<string> onlyInRight,
        IReadOnlyList<string> contentDiffers)
    {
        OnlyInLeft = onlyInLeft;
        OnlyInRight = onlyInRight;
        ContentDiffers = contentDiffers;
    }

    public IReadOnlyList<string> OnlyInLeft { get; }
    public IReadOnlyList<string> OnlyInRight { get; }
    public IReadOnlyList<string> ContentDiffers { get; }

    /// <summary>True when both trees hold the same paths with byte-identical content.</summary>
    public bool Identical => OnlyInLeft.Count == 0 && OnlyInRight.Count == 0 && ContentDiffers.Count == 0;
}

/// <summary>
/// Byte-level directory-tree comparison. Backs the §3.1 determinism self-check: run the transform
/// twice on the same commit and assert <see cref="TreeDiffResult.Identical"/> — any residual
/// non-determinism fails loud rather than polluting the alignment delta.
/// </summary>
public static class TreeDiff
{
    public static TreeDiffResult Compare(string leftDir, string rightDir)
    {
        var left = Files(leftDir);
        var right = Files(rightDir);

        var onlyInLeft = left.Keys.Where(k => !right.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
        var onlyInRight = right.Keys.Where(k => !left.ContainsKey(k))
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();

        var differs = new List<string>();
        foreach (var key in left.Keys.Where(right.ContainsKey).OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
        {
            if (!FilesEqual(left[key], right[key]))
            {
                differs.Add(key);
            }
        }

        return new TreeDiffResult(onlyInLeft, onlyInRight, differs);
    }

    private static Dictionary<string, string> Files(string root)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var full = Path.GetFullPath(root);
        if (!Directory.Exists(full))
        {
            return map;
        }

        var prefix = full.Length + (full.EndsWith(Path.DirectorySeparatorChar.ToString()) ? 0 : 1);
        foreach (var file in Directory.EnumerateFiles(full, "*", SearchOption.AllDirectories))
        {
            map[file.Substring(prefix).Replace('\\', '/')] = file;
        }

        return map;
    }

    private static bool FilesEqual(string a, string b)
    {
        var infoA = new FileInfo(a);
        var infoB = new FileInfo(b);
        if (infoA.Length != infoB.Length)
        {
            return false;
        }

        using var streamA = File.OpenRead(a);
        using var streamB = File.OpenRead(b);
        var bufferA = new byte[8192];
        var bufferB = new byte[8192];

        int read;
        while ((read = streamA.Read(bufferA, 0, bufferA.Length)) > 0)
        {
            if (ReadFully(streamB, bufferB, read) != read)
            {
                return false;
            }

            for (var i = 0; i < read; i++)
            {
                if (bufferA[i] != bufferB[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static int ReadFully(Stream stream, byte[] buffer, int count)
    {
        var offset = 0;
        int read;
        while (offset < count && (read = stream.Read(buffer, offset, count - offset)) > 0)
        {
            offset += read;
        }

        return offset;
    }
}
