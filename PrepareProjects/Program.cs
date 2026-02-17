using Prepare.Common;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Projects;
using PrepareProjects.Preparation;
using PrepareProjects.ProjectHandling;
using System.Text;
using System.Xml.Linq;

namespace PrepareProjects;

public class Program
{
    public static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if REFERENCE
        var fullPath = Path.GetFullPath(Path.Combine(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.csproj"));
        if (File.Exists(fullPath))
        {
            ProcessFileUpdatesProcessor.ProcessFilesUpdates(fullPath);
        }
#endif
        var (targetDir, sourceProjectPath, csprojPath) =
            await GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", "Net4x.Gtk.Windows.Forms");
        SystemResourcesExtensionsProjectHandler.Prepare();
        var version = VersionPrepare.PrepareSolutionVersion();

        var projectName = await GtkSampleAppProjectsHandler.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName =
            await GtkSampleAppProjectsHandler.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        var currentDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.Combine(targetDir, "Properties");
        var combine = Path.Combine(targetDir, "Net4x.Gtk.Windows.Forms.csproj");
        RunResGen(combine, "Resources.resx", "System.Properties");
        RunResGen(projectName, "GtkWinFormsAppResources.resx",
            $"{Path.GetFileNameWithoutExtension(projectName)}.Properties");
        RunResGen(otherProjectName, "GtkWinFormsAppResources.resx",
            $"{Path.GetFileNameWithoutExtension(otherProjectName)}.Properties");
        Environment.CurrentDirectory = currentDirectory;

        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" });

        if (File.Exists(@"..\..\..\Resources\ControlExtension.cs"))
        {
            File.Copy(@"..\..\..\Resources\ControlExtension.cs",
            Path.Combine(targetDir, @"GTKControls\ControlExtension.cs"));
        }
        File.Copy(@"..\..\..\Resources\GridCell.cs", Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
        if (File.Exists(@"..\..\..\Resources\DeserializingResourceReader.cs"))
        {
            var destFileName = Path.Combine(targetDir, @"GTKSystem\Resources\Extensions\DeserializingResourceReader.cs");
            File.Delete(destFileName);
            File.Copy(@"..\..\..\Resources\DeserializingResourceReader.cs",
                destFileName);
        }
        if (File.Exists(@"..\..\..\Resources\TreeView.cs"))
        {
            File.Delete(Path.Combine(targetDir, @"GTKControls\TreeView.cs"));
            File.Copy(@"..\..\..\Resources\TreeView.cs", Path.Combine(targetDir, @"GTKControls\TreeView.cs"));
        }
        SummaryCommentTransformer.TransformFolder(targetDir);
        MemberSpacingFormatter.TransformFolder(targetDir);
        ProjectCompilation.RestoreProject(sourceProjectPath);

#pragma warning disable CS8604 // Possible null reference argument.
        var path = Path.Combine(Path.GetDirectoryName(csprojPath), "GTKSystem.Windows.Forms.xml");
#pragma warning restore CS8604 // Possible null reference argument.
        if (!File.Exists(path))
        {
            File.Delete(path);
        }
        new FolderFixer().ReorganizeFiles(csprojPath, new[]
        {
            "Properties",
            "Content",
            "Resources"
        }, ["GTKControls", "GTKSystem"], ["Application"]);
        NormalizeGtkFolders(csprojPath);
        FixTableLayoutPanelReference(csprojPath);

        FixClass(csprojPath, Path.Combine(
            "GtkSystem",
            "Windows",
            "Forms",
            "GTKControls",
            "ControlBase",
            "TableLayoutPanelBase.cs"));

        FixClass(csprojPath, Path.Combine(
            "System",
            "Windows",
            "Forms",
            "TreeView.cs"));

        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        RenameFolder(Path.Combine(projectDirectory, "GtkSystem", "Windows", "Forms"),
            "GTKControls", "GtkControls", csprojPath);
    }

    private static void FixClass(string csprojPath, string relativePath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        var fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            return;
        }

        var content = File.ReadAllText(fullPath);

        if (!content.Contains("GTKControls"))
        {
            return;
        }

        content = content.Replace("GTKControls", "GtkControls");

        File.WriteAllText(fullPath, content);
    }

    private static void FixTableLayoutPanelReference(string csprojPath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;

        var relativePath = Path.Combine("System", "Windows", "Forms", "TableLayoutPanel.cs");
        var fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            return;
        }

        var content = File.ReadAllText(fullPath);

        var modified = false;

        // 1️⃣ Replace GTKControls -> GtkControls
        if (content.Contains("GTKControls"))
        {
            content = content.Replace("GTKControls", "GtkControls");
            modified = true;
        }

        if (modified)
        {
            File.WriteAllText(fullPath, content);
            return;
        }
    }

    private static void NormalizeGtkFolders(string csprojPath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;

        RenameFolder(projectDirectory, "GTKSystem", "GtkSystem", csprojPath);
        RenameFolder(projectDirectory, "GTKControls", "GtkControls", csprojPath);

        // After renaming, update .cs files inside GtkControls
        var gtkControlsPath = Path.Combine(projectDirectory, "GtkControls");

        if (Directory.Exists(gtkControlsPath))
        {
            var csFiles = Directory.GetFiles(
                gtkControlsPath,
                "*.cs",
                SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var content = File.ReadAllText(file);

                if (content.Contains("GTKControls"))
                {
                    content = content.Replace("GTKControls", "GtkControls");
                    File.WriteAllText(file, content);
                }
            }
        }
    }

    private static void RenameFolder(
        string projectDirectory,
        string oldName,
        string newName, string csprojPath)
    {
        var oldPath = Path.Combine(projectDirectory, oldName);
        var newPath = Path.Combine(projectDirectory, newName);

        if (!Directory.Exists(oldPath))
        {
            return;
        }

        if (Directory.Exists(newPath))
        {
            var path = oldPath + Guid.NewGuid();
            Directory.Move(oldPath, path);
            oldPath = path;
        }

        if (Directory.Exists(newPath))
        {
            throw new InvalidOperationException(
                $"Cannot rename '{oldName}' because '{newName}' already exists.");
        }

        // Windows is case-insensitive, so do a two-step rename if only casing changes
        var tempPath = Path.Combine(projectDirectory, Guid.NewGuid().ToString());

        Directory.Move(oldPath, tempPath);
        Directory.Move(tempPath, newPath);
        UpdateFilesXmlEntries(projectDirectory, oldName, newName, csprojPath, newPath);
    }

    private static void UpdateFilesXmlEntries(string projectDirectory, string oldName, string newName, string csprojPath,
        string newPath)
    {
        // 🔽 NEW PART — update Files.xml entries
        var renamedFiles = Directory.GetFiles(newPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in renamedFiles)
        {
            string oldRelative = Path.GetRelativePath(projectDirectory,
                file.Replace(newName, oldName));

            string newRelative = Path.GetRelativePath(projectDirectory, file);

            UpdateFilesXmlAfterRename(csprojPath, oldRelative, newRelative);
        }
    }

    private static void RunResGen(string projectName, string resourceName, string targetNamespace)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            Console.WriteLine("No project name specified for resgen.");
            return;
        }

        // Assume project path is the projectName directory
        var projectDir = Path.GetDirectoryName(Path.GetFullPath(projectName));
        var resxPath = Path.Combine(projectDir, "Properties", resourceName);

        ResxTypedClassGenerator.GenerateDesigner(resxPath, targetNamespace);
    }

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
            item.Value = File.ReadAllText(fullPath);
        }

        doc.Save(filesXmlPath);
    }
}