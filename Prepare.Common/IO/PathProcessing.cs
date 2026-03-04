namespace Prepare.Common.IO
{
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
            string[] specialRoots,
            string[] specialRootsIncludingRoot)
        {
            var fileName = Path.GetFileName(fullPath);

            var namespacePath = namespaceName.Replace('.', Path.DirectorySeparatorChar);

            var destinationFolder = Path.Combine(projectDirectory, namespacePath);

            var normalizedFullPath = fullPath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            var enumerable = new List<string>(specialRoots);
            enumerable.AddRange(specialRootsIncludingRoot);

            foreach (var root in enumerable)
            {
                var index = normalizedFullPath
                    .IndexOf(root, StringComparison.OrdinalIgnoreCase);

                if (index >= 0)
                {
                    var includeRoot = specialRootsIncludingRoot
                        .Any(i => root.Equals(i, StringComparison.OrdinalIgnoreCase));

                    string relevantPath;

                    if (includeRoot)
                    {
                        relevantPath = normalizedFullPath.Substring(index);
                    }
                    else
                    {
                        relevantPath = normalizedFullPath
                            .Substring(index + root.Length)
                            .TrimStart(Path.DirectorySeparatorChar);
                    }

                    var directoryPart = Path.GetDirectoryName(relevantPath);

                    if (!string.IsNullOrWhiteSpace(directoryPart))
                    {
                        var parts = directoryPart
                            .Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(CapitalizeFirstLetter)
                            .ToArray();

                        var extraPath = Path.Combine(parts);

                        destinationFolder = Path.Combine(destinationFolder, extraPath);
                    }

                    break;
                }
            }

            destinationFolder = RemoveRepeatedTrailingSegments(destinationFolder);

            Directory.CreateDirectory(destinationFolder);

            var destinationFullPath = Path.Combine(destinationFolder, fileName);

            if (!string.Equals(fullPath, destinationFullPath, StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(destinationFullPath))
                {
                    throw new InvalidOperationException(
                        "The destination file '" + destinationFullPath + "' already exists");
                }

                File.Move(fullPath, destinationFullPath);
            }

            return RelativePath.Get(Path.GetFullPath(projectDirectory), Path.GetFullPath(destinationFullPath));
        }

        private static string RemoveRepeatedTrailingSegments(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return fullPath;

            fullPath = fullPath.TrimEnd(Path.DirectorySeparatorChar);

            var parts = fullPath
                .Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (parts.Count < 2)
                return fullPath;

            var removed = false;

            for (var len = 1; len <= parts.Count / 2; len++)
            {
                var lastSegment = parts.Skip(parts.Count - len).Take(len).ToList();
                var previousSegment = parts.Skip(parts.Count - (2 * len)).Take(len).ToList();

                if (SegmentsEqual(previousSegment, lastSegment))
                {
                    parts.RemoveRange(parts.Count - len, len);
                    removed = true;
                    break;
                }
            }

            if (!removed)
            {
                var last = parts[parts.Count - 1];

                if (parts.Take(parts.Count - 1)
                    .Any(p => string.Equals(p, last, StringComparison.Ordinal)))
                {
                    parts.RemoveAt(parts.Count - 1);
                }
            }

            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        private static bool SegmentsEqual(List<string> a, List<string> b)
        {
            if (a.Count != b.Count)
                return false;

            for (var i = 0; i < a.Count; i++)
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
                .Select(file => RelativePath.Get(Path.GetFullPath(projectDirectory), Path.GetFullPath(file)))
                .ToDictionary(
                    relativePath => relativePath,
                    relativePath => relativePath);
        }
    }
}
