using Prepare.Common.Logging;
using XmlDocSync.Roslyn;
using System.Text.RegularExpressions;

namespace XmlDocSync.Services;

public static class TopLevelTypeSplitService
{
    public static void ProcessDirectory(string directoryPath, string[] toExclude)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

        var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !toExclude.Any(i => Path.GetFullPath(f).EndsWith(i, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        // Group files by directory
        var filesByDirectory = csFiles.GroupBy(Path.GetDirectoryName);

        ProcessFilesInDirectory(filesByDirectory, ProcessFiles);
    }

    internal static void ProcessFilesInDirectory(IEnumerable<IGrouping<string?, string>> filesByDirectory, Action<List<string>> processFiles)
    {
        foreach (var dirGroup in filesByDirectory)
        {
            var filesInDir = dirGroup.ToList();

            var filesToProcess = new List<string>();

            // First, identify all base files (files that have child files)
            var baseFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var childFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in filesInDir)
            {
                var baseName = Path.GetFileNameWithoutExtension(file);

                // Check if this file has any children (files matching BaseName.*.cs)
                var hasChildren = filesInDir.Any(f =>
                {
                    if (string.Equals(f, file, StringComparison.OrdinalIgnoreCase))
                        return false;
                    var otherFileName = Path.GetFileName(f);
                    return Regex.IsMatch(otherFileName, $"^{Regex.Escape(baseName)}\\..+\\.cs$", RegexOptions.IgnoreCase);
                });

                if (hasChildren)
                {
                    baseFiles.Add(file);
                }

                // Check if this file is a child of another file
                var fileName = Path.GetFileName(file);
                if (fileName.Count(c => c == '.') > 1) // Has pattern like "Base.Child.cs"
                {
                    var potentialBaseName = fileName.Substring(0, fileName.IndexOf('.'));
                    var potentialBaseFile = filesInDir.FirstOrDefault(f =>
                    {
                        var fName = Path.GetFileNameWithoutExtension(f);
                        return string.Equals(fName, potentialBaseName, StringComparison.OrdinalIgnoreCase);
                    });

                    if (potentialBaseFile != null)
                    {
                        childFiles.Add(file);
                    }
                }
            }

            // Process only files that are neither base files nor child files
            foreach (var file in filesInDir)
            {
                if (!baseFiles.Contains(file) && !childFiles.Contains(file))
                {
                    filesToProcess.Add(file);
                }
            }

            processFiles?.Invoke(filesToProcess);
        }
    }

    private static void ProcessFiles(List<string> filesToProcess)
    {
        foreach (var file in filesToProcess)
        {
            try
            {
                ProcessFile(file);
            }
            catch (Exception ex)
            {
                RedConsole.Write($"Error processing file '{file}': {ex.Message}");
            }
        }
    }

    public static void ProcessFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var sourceCode = File.ReadAllText(filePath);

        var splitResults = TopLevelTypeSplitter.Split(sourceCode);

        // If nothing to split (or only one type with same filename), do nothing
        if (splitResults.Count == 0)
            return;

        var directory = Path.GetDirectoryName(filePath)!;
        var originalFileName = Path.GetFileName(filePath);

        var generatedFileNames = new List<string>();

        // Write all generated files
        foreach (var kvp in splitResults)
        {
            var newFileName = kvp.Key;
            var newFilePath = Path.Combine(directory, newFileName);

            var contents = kvp.Value;
            while (contents.Contains("\r\n\r\n\r\n"))
            {
                contents = contents.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            }
            File.WriteAllText(newFilePath, contents);
            generatedFileNames.Add(newFileName);
        }

        // Delete original file ONLY if none of the generated files match its name
        var shouldDeleteOriginal = generatedFileNames
            .All(name => !string.Equals(name, originalFileName, StringComparison.OrdinalIgnoreCase));

        if (shouldDeleteOriginal)
        {
            File.Delete(filePath);
        }
    }
}
