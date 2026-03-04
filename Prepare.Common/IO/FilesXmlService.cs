using System.Xml.Linq;

namespace Prepare.Common.IO;

public static class FilesXmlService
{
    public static void UpdateFilesXmlEntries(string projectDirectory, string oldName, string newName,
        string csprojPath,
        string newPath)
    {
        var renamedFiles = Directory.GetFiles(newPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in renamedFiles)
        {
            var oldRelative = RelativePath.Get(Path.GetFullPath(projectDirectory),
                Path.GetFullPath(file.Replace(newName, oldName)));

            var newRelative = RelativePath.Get(Path.GetFullPath(projectDirectory), Path.GetFullPath(file));

            UpdateFilesXmlAfterRename(csprojPath, oldRelative, newRelative);
        }
    }

    private static void UpdateFilesXmlAfterRename(
        string csprojPath,
        string oldRelativePath,
        string newRelativePath)
    {
        var filesXmlPath = Path.ChangeExtension(csprojPath, ".Files.xml");
        filesXmlPath = PathService.ChangeLocationToParentFolder(filesXmlPath);

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

            var projectName = Path.GetFileNameWithoutExtension(csprojPath);
            var destFileName = BackupUtility.GetDestFileName(fullPath, BackupUtility.Backup, projectName);
            item.Value = destFileName;
        }

        doc.Save(filesXmlPath);
    }
}
