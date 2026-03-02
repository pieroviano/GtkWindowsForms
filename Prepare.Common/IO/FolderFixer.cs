using System.Xml.Linq;

namespace Prepare.Common.IO;

public static class FolderFixer
{
    public static void ReorganizeFiles(
        string csprojPath,
        string[] excludedPrefixes,
        string[] specialRoots,
        string[] specialRootsIncludingRoot)
    {
        if (string.IsNullOrWhiteSpace(csprojPath))
        {
            throw new ArgumentException("csprojPath cannot be null or empty.");
        }

        if (!File.Exists(csprojPath))
        {
            throw new FileNotFoundException("The specified csproj file does not exist.", csprojPath);
        }

        var projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out var projectDirectory);

        FixFolders(projectFiles, csprojPath, excludedPrefixes, specialRoots, specialRootsIncludingRoot);

        UpdateCsprojCompileItems(csprojPath, projectFiles);

        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, projectDirectory, BackupUtility.Backup);

        DeleteEmptyDirectories(projectDirectory);
    }

    private static void DeleteEmptyDirectories(string rootDirectory)
    {
        var directories = Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories)
            .OrderByDescending(d => d.Length)
            .ToList();

        foreach (var dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                continue;
            }

            var hasFiles = Directory.GetFiles(dir).Any();
            var hasSubDirs = Directory.GetDirectories(dir).Any();

            if (!hasFiles && !hasSubDirs)
            {
                Directory.Delete(dir);
            }
        }
    }

    private static void FixFolders(Dictionary<string, string> projectFiles, string csprojPath,
        string[] excludedPrefixes, string[] specialRoots, string[] specialRootsIncludingRoot)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath);

        foreach (var key in projectFiles.Keys.ToList())
        {
            if (!key.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (PathProcessing.IsExcluded(key, excludedPrefixes))
            {
                continue;
            }

#pragma warning disable CS8604 // Possible null reference argument.
            var fullPath = Path.Combine(projectDirectory, key);
#pragma warning restore CS8604 // Possible null reference argument.

            if (!File.Exists(fullPath))
            {
                continue;
            }

            var fileContent = File.ReadAllText(fullPath);

            var namespaceName = CsContentProcessor.ExtractNamespace(fileContent);

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                continue;
            }

            var newRelativePath = PathProcessing.MoveFileToRightFolder(
                projectDirectory,
                fullPath,
                namespaceName,
                specialRoots, specialRootsIncludingRoot);

            projectFiles[key] = newRelativePath;
        }
    }

    private static void UpdateCsprojCompileItems(
        string csprojPath,
        Dictionary<string, string> projectFiles)
    {
        var doc = XDocument.Load(csprojPath);

        var compileElements = doc.Descendants()
            .Where(e => e.Name.LocalName == "Compile");

        foreach (var element in compileElements)
        {
            foreach (var attrName in new[] { "Include", "Update", "Remove" })
            {
                var attr = element.Attribute(attrName);
                if (attr == null)
                {
                    continue;
                }

                var originalValue = attr.Value.Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar);

                if (projectFiles.TryGetValue(originalValue, out var newValue))
                {
                    attr.Value = newValue.Replace('\\', '/'); // keep project-style separators
                }
            }
        }

        doc.Save(csprojPath);
    }
}
