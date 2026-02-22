namespace Prepare.Common.IO;

public static class PathProcessing
{
    public static bool IsExcluded(string relativePath, string[] excludedPrefixes)
    {
        foreach (var prefix in excludedPrefixes)
        {
            if (relativePath.StartsWith(prefix + "\\", StringComparison.OrdinalIgnoreCase) ||
                relativePath.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase) ||
                relativePath.Equals(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static string MoveFileToRightFolder(
        string projectDirectory,
        string fullPath,
        string namespaceName,
        string[] specialRoots, string[] specialRootsIncludingRoot)
    {
        string fileName = Path.GetFileName(fullPath);

        string namespacePath = namespaceName.Replace('.', Path.DirectorySeparatorChar);

        string destinationFolder = Path.Combine(projectDirectory, namespacePath);

        string normalizedFullPath = fullPath.Replace('/', Path.DirectorySeparatorChar)
                                            .Replace('\\', Path.DirectorySeparatorChar);

        var enumerable = specialRoots.ToList();
        enumerable.AddRange(specialRootsIncludingRoot);
        foreach (var root in enumerable)
        {
            int index = normalizedFullPath.IndexOf(root, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {

                bool includeRoot =
                    specialRootsIncludingRoot.Any(i => root.Equals(i, StringComparison.OrdinalIgnoreCase));

                string relevantPath;

                if (includeRoot)
                {
                    // Include the root folder itself
                    relevantPath = normalizedFullPath.Substring(index);
                }
                else
                {
                    // Exclude the root folder (GTKControls / GTKSystem behavior)
                    relevantPath = normalizedFullPath
                        .Substring(index + root.Length)
                        .TrimStart(Path.DirectorySeparatorChar);
                }

                string? directoryPart = Path.GetDirectoryName(relevantPath);

                if (!string.IsNullOrWhiteSpace(directoryPart))
                {
                    var parts = directoryPart
                        .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                        .Select(CapitalizeFirstLetter);

                    string extraPath = Path.Combine(parts.ToArray());

                    destinationFolder = Path.Combine(destinationFolder, extraPath);
                }

                break;
            }
        }

        // Remove repeated trailing folder sequences
        destinationFolder = RemoveRepeatedTrailingSegments(destinationFolder);

        Directory.CreateDirectory(destinationFolder);

        string destinationFullPath = Path.Combine(destinationFolder, fileName);

        if (!string.Equals(fullPath, destinationFullPath, StringComparison.OrdinalIgnoreCase))
        {
            if (File.Exists(destinationFullPath))
            {
                throw new InvalidOperationException(
                    $"The destination file '{destinationFullPath}' already exists");
            }

            File.Move(fullPath, destinationFullPath);
        }

        return Path.GetRelativePath(projectDirectory, destinationFullPath);
    }

    private static string RemoveRepeatedTrailingSegments(string fullPath)
    {
        if (string.IsNullOrWhiteSpace(fullPath))
            return fullPath;

        fullPath = fullPath.TrimEnd(Path.DirectorySeparatorChar);

        var parts = fullPath
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (parts.Count < 2)
            return fullPath;

        bool removed = false;

        for (int len = 1; len <= parts.Count / 2; len++)
        {
            var lastSegment = parts.Skip(parts.Count - len).Take(len).ToList();
            var previousSegment = parts.Skip(parts.Count - (2 * len)).Take(len).ToList();

            if (SegmentsEqual(previousSegment, lastSegment))
            {
                parts.RemoveRange(parts.Count - len, len);
                removed = true;
                break; // remove only one repetition
            }
        }

        if (!removed)
        {
            var last = parts[^1];

            if (parts.Take(parts.Count - 1)
                .Any(p => string.Equals(p, last, StringComparison.Ordinal)))
            {
                parts.RemoveAt(parts.Count - 1);
            }
        }

        return string.Join(Path.DirectorySeparatorChar, parts);
    }
    private static bool SegmentsEqual(List<string> a, List<string> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (!a[i].Equals(b[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }


    private static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (input.Length == 1)
            return input.ToUpperInvariant();

        return char.ToUpperInvariant(input[0]) + input.Substring(1);
    }

    public static Dictionary<string, string> GetProjectFiles(
        string[] files,
        string projectDirectory)
    {
        return files
            .Select(file => Path.GetRelativePath(projectDirectory, file))
            .ToDictionary(relativePath => relativePath,
                relativePath => relativePath);
    }
}