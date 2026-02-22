using System.Xml.Linq;
using Prepare.Common.IO;

namespace PrepareProjects.Preparation.Directories;

public static class FilesXmlService
{
    private static void UpdateFilesXmlAfterRename(
        string csprojPath,
        string oldRelativePath,
        string newRelativePath)
    {
        var filesXmlPath = Path.ChangeExtension(csprojPath, ".Files.xml");

        if (!File.Exists(filesXmlPath))
        {
            return;
        }

        var doc = XDocument.Load(filesXmlPath);

        var item = doc.Root?
            .Elements("Item")
            .FirstOrDefault(e =>
                string.Equals(
                    e.Attribute("Destination")?.Value,
                    oldRelativePath.Replace('\\', '/'),
                    StringComparison.OrdinalIgnoreCase) || string.Equals(
                    e.Attribute("Destination")?.Value,
                    oldRelativePath.Replace('/', '\\'),
                    StringComparison.OrdinalIgnoreCase));

        if (item == null)
        {
            return;
        }

        // Update Destination attribute
        item.SetAttributeValue("Destination", newRelativePath.Replace('\\', '/'));

        // Update inner text from disk
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        var fullPath = Path.Combine(projectDirectory, newRelativePath);

        if (File.Exists(fullPath))
        {
            if (File.Exists(item.Value))
            {
                File.Delete(item.Value);
            }

            var destFileName = BackupUtility.GetDestFileName(fullPath, BackupUtility.Backup);
            item.Value = destFileName;
        }

        doc.Save(filesXmlPath);
    }

    public static void UpdateFilesXmlEntries(string projectDirectory, string oldName, string newName,
        string csprojPath,
        string newPath)
    {
        // 🔽 NEW PART — update Files.xml entries
        var renamedFiles = Directory.GetFiles(newPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in renamedFiles)
        {
            var oldRelative = Path.GetRelativePath(projectDirectory,
                file.Replace(newName, oldName));

            var newRelative = Path.GetRelativePath(projectDirectory, file);

            UpdateFilesXmlAfterRename(csprojPath, oldRelative, newRelative);
        }
    }
}