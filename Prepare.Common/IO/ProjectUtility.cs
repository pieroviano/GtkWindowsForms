using Prepare.Common.Languages;
using Prepare.Common.Logging;
using Prepare.Common.Projects;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Common.IO;

public static class ProjectUtility
{
    public static void CopyDirectory(string sourceDir, string targetDir, string? prefixOfFilesToDelete = "GTKWinFormsApp_Win")
    {
        var files = Directory.GetFiles(sourceDir);
        foreach (var file in files)
        {
            var destFile = Path.Combine(targetDir, Path.GetFileName(file));
            LogUtility.FileCopyAndLog(file, destFile, true);
        }

        var directories = Directory.GetDirectories(sourceDir).Where(d => !d.EndsWith("\\bin") && !d.EndsWith("\\obj"));
        foreach (var directory in directories)
        {
            var destDir = Path.Combine(targetDir, Path.GetFileName(directory));
            Directory.CreateDirectory(destDir);
            CopyDirectory(directory, destDir);
        }

        // Delete GTKWinFormsApp_Win*.* files in targetDir
        if (!string.IsNullOrEmpty(prefixOfFilesToDelete))
        {
            foreach (var file in Directory.GetFiles(targetDir, $"{prefixOfFilesToDelete}*.*"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    // Ignore delete failures (locked, readonly, etc.)
                    YellowConsole.Write($"Unexpected exception: {ex}");
                }
            }
        }
    }

    public static string CopyProjectAndTranslateIt(object? sourceProjectName, object? targetProjectName,
        Func<string, string?, string, string> patchProjectNamespace)
    {
        var sourceDir = Path.GetFullPath(
            @$"..\..\..\..\gtksystem-windows-forms\Source\{sourceProjectName}");

        var targetDir = Path.GetFullPath(@$"..\..\..\..\{targetProjectName}");

        PrepareEmptyDirectory(targetDir);
        CopyDirectory(sourceDir, targetDir);

        var fileToTranslate = Path.GetFullPath(@$"{@$"..\..\..\..\{targetProjectName}"}\GTKSystem.Windows.Forms.csproj");
        Translator.ProcessFileTranslation(fileToTranslate, patchProjectNamespace, "Translations");
        return targetDir;
    }

    public static void CopyLocalizationResourcesFiles(string targetDir, params string[] sourceFileNames)
    {
        var resourcesDir = Path.GetFullPath(@"..\..\..\..\Resources");
        Directory.CreateDirectory(targetDir);

        foreach (var sourceFile in sourceFileNames)
        {
            CopyAndRenameFile(resourcesDir, targetDir, sourceFile);
        }
    }

    public static async Task EnsureLatestGtkSharpAsync(string csprojPath)
    {
        var latestVersion = await NuGetPackageVersionProvider.GetLatestStableGtkSharpVersionAsync();

        if (latestVersion == null)
        {
            return;
        }

        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var gtkSharpReference = doc
            .Descendants(ns + "PackageReference")
            .FirstOrDefault(pr =>
                string.Equals(
                    (string?)pr.Attribute("Include"),
                    "GtkSharp",
                    StringComparison.OrdinalIgnoreCase));

        if (gtkSharpReference == null)
        {
            return;
        }

        var versionAttribute = gtkSharpReference.Attribute("Version");

        if (versionAttribute == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(versionAttribute.Value))
        {
            return;
        }

        versionAttribute.Value = latestVersion.ToString();

        doc.Save(csprojPath);
    }

    public static void PrepareEmptyDirectory(string targetDir)
    {
        if (Directory.Exists(targetDir))
        {
            Directory.Delete(targetDir, true);
        }

        Directory.CreateDirectory(targetDir);
    }

    private static void CopyAndRenameFile(string sourceDir, string targetDir, string sourceFileName)
    {
        var sourcePath = Path.Combine(sourceDir, sourceFileName);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                "Required resource file not found.",
                sourcePath);
        }

        string targetFileName;
        var extension = Path.GetExtension(sourceFileName);

        if (extension == ".json" || extension == ".cs")
        {
            targetFileName = sourceFileName;
        }
        else
        {
            targetFileName = Path.ChangeExtension(sourceFileName, ".resx");
        }

        var targetPath = Path.Combine(targetDir, targetFileName);

        // ------------------------------------------------------------
        // Special handling for .resx
        // ------------------------------------------------------------
        if (targetFileName.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
        {
            NormalizeResourcesNamesAndSave(targetDir, sourcePath, targetPath);
        }
        else
        {
            LogUtility.FileCopyAndLog(sourcePath, targetPath, true);
        }
    }

    private static void NormalizeResourcesNamesAndSave(string targetDir, string sourcePath, string targetPath)
    {
        var doc = XDocument.Load(sourcePath);

        foreach (var data in doc.Descendants("data"))
        {
            var nameAttr = data.Attribute("name");
            if (nameAttr == null)
                continue;

            var originalName = nameAttr.Value;
            var normalizedName = NormalizeResourceName(originalName);

            if (!string.Equals(originalName, normalizedName, StringComparison.Ordinal))
            {
                nameAttr.Value = normalizedName;
            }
        }

        Directory.CreateDirectory(targetDir);
        doc.Save(targetPath);
    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private static string NormalizeResourceName(string name)
    {
        // Replace non-alphanumeric characters with '_'
        var result = Regex.Replace(name, @"[^a-zA-Z0-9]", "_");

        // Collapse multiple underscores
        result = Regex.Replace(result, @"_+", "_");

        // Trim underscores
        result = result.Trim('_');

        if (result.StartsWith("Gtk"))
        {
            return result;
        }
        // Prepend prefix
        return $"Gtk_{result}";
    }

    public static Dictionary<string, string> GetProjectFiles(string csprojPath, out string projectDirectory)
    {
        projectDirectory = Path.GetDirectoryName(csprojPath)!;

        var files = Directory.GetFiles(projectDirectory, "*.*", SearchOption.AllDirectories);

        var projectFiles = PathProcessing.GetProjectFiles(files, projectDirectory);
        return projectFiles;
    }

    public static void PersistProjectFiles(
        string csprojPath,
        Dictionary<string, string> projectFiles,
        string projectDirectory, string backupName)
    {
        var root = new XElement("Files");
        BackupUtility.CleanBackup(projectDirectory, backupName);
        foreach (var kvp in projectFiles)
        {
            var itemElement = new XElement("Item",
                new XAttribute("Source", kvp.Key),
                new XAttribute("Destination", kvp.Value)
            );

            ReorderingPersister.AddInnerTextFromFileContent(projectDirectory, kvp, itemElement, backupName);

            root.Add(itemElement);
        }

        var outputPath = Path.ChangeExtension(csprojPath, $".{backupName.Replace(BackupUtility.Backup, BackupUtility.Files)}.xml");
        outputPath = PathService.ChangeLocationToParentFolder(outputPath);

        var doc = new XDocument(root);
        doc.Save(outputPath);
    }

    public static string? GetFilesXmlPath(string csprojPath, string backupName)
    {
        var filesXmlPath = Path.ChangeExtension(csprojPath,
            $".{backupName.Replace(BackupUtility.Backup, BackupUtility.Files)}.xml");
        filesXmlPath = PathService.ChangeLocationToParentFolder(filesXmlPath);

        if (!File.Exists(filesXmlPath))
        {
            Console.Error.WriteLine($"Associated .{backupName.Replace(BackupUtility.Backup, BackupUtility.Files)}.xml not found: {filesXmlPath}");
            return null;
        }

        return filesXmlPath;
    }
}
