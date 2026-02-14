using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace PrepareProjects;

public class Program
{
    static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var (targetDir, sourceProjectPath, csprojPath) = await GtkWindowsForms.PrepareAsync("GTKSystem.Windows.Forms", "Net4x.Gtk.Windows.Forms");
        SystemResourcesExtensions.Prepare();
        var version = VersionPrepare.PrepareSolutionVersion();

        var projectName = await GtkSampleApp.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName = await GtkSampleApp.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        var currentDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.Combine(targetDir, "Properties");
        var combine = Path.Combine(targetDir, "Net4x.Gtk.Windows.Forms.csproj");
        RunResGen(combine, "Resources.resx", "System.Properties");
        RunResGen(projectName, "GtkWinFormsAppResources.resx", $"{Path.GetFileNameWithoutExtension(projectName)}.Properties");
        RunResGen(otherProjectName, "GtkWinFormsAppResources.resx", $"{Path.GetFileNameWithoutExtension(otherProjectName)}.Properties");
        Environment.CurrentDirectory = currentDirectory;

        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" });

        File.Copy(@"..\..\..\Resources\ControlExtension.cs", Path.Combine(targetDir, @"GTKControls\ControlExtension.cs"));
        File.Copy(@"..\..\..\Resources\GridCell.cs", Path.Combine(targetDir, @"GTKControls\GridCell.cs"));
        File.Delete(Path.Combine(targetDir, @"GTKSystem\Resources\Extensions\DeserializingResourceReader.cs"));
        File.Copy(@"..\..\..\Resources\DeserializingResourceReader.cs", Path.Combine(targetDir, @"GTKSystem\Resources\Extensions\DeserializingResourceReader.cs"));
        File.Delete(Path.Combine(targetDir, @"GTKControls\TreeView.cs"));
        File.Copy(@"..\..\..\Resources\TreeView.cs", Path.Combine(targetDir, @"GTKControls\TreeView.cs"));

        ProjectCompilation.RestoreProject(sourceProjectPath);

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
        string projectDirectory = Path.GetDirectoryName(csprojPath)!;
        RenameFolder(Path.Combine(projectDirectory, "GtkSystem", "Windows", "Forms"), 
            "GTKControls", "GtkControls");
    }

    private static void NormalizeGtkFolders(string csprojPath)
    {
        string projectDirectory = Path.GetDirectoryName(csprojPath)!;

        RenameFolder(projectDirectory, "GTKSystem", "GtkSystem");
        RenameFolder(projectDirectory, "GTKControls", "GtkControls");

        // After renaming, update .cs files inside GtkControls
        string gtkControlsPath = Path.Combine(projectDirectory, "GtkControls");

        if (Directory.Exists(gtkControlsPath))
        {
            var csFiles = Directory.GetFiles(
                gtkControlsPath,
                "*.cs",
                SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                string content = File.ReadAllText(file);

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
        string newName)
    {
        string oldPath = Path.Combine(projectDirectory, oldName);
        string newPath = Path.Combine(projectDirectory, newName);

        if (!Directory.Exists(oldPath))
            return;

        if (Directory.Exists(newPath))
        {
            var path = oldPath + Guid.NewGuid().ToString();
            Directory.Move(oldPath, path);
            oldPath = path;
        }
        if (Directory.Exists(newPath))
            throw new InvalidOperationException(
                $"Cannot rename '{oldName}' because '{newName}' already exists.");

        // Windows is case-insensitive, so do a two-step rename if only casing changes
        string tempPath = Path.Combine(projectDirectory, Guid.NewGuid().ToString());

        Directory.Move(oldPath, tempPath);
        Directory.Move(tempPath, newPath);
    }

    private static void FixClass(string csprojPath, string relativePath)
    {
        string projectDirectory = Path.GetDirectoryName(csprojPath)!;
        string fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
            return;

        string content = File.ReadAllText(fullPath);

        if (!content.Contains("GTKControls"))
            return;

        content = content.Replace("GTKControls", "GtkControls");

        File.WriteAllText(fullPath, content);
    }

    private static void FixTableLayoutPanelReference(string csprojPath)
    {
        string projectDirectory = Path.GetDirectoryName(csprojPath)!;

        string relativePath = Path.Combine("System", "Windows", "Forms", "TableLayoutPanel.cs");
        string fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
            return;

        string content = File.ReadAllText(fullPath);

        bool modified = false;

        // 1️⃣ Replace GTKControls -> GtkControls
        if (content.Contains("GTKControls"))
        {
            content = content.Replace("GTKControls", "GtkControls");
            modified = true;
        }

        if (modified)
        {
            File.WriteAllText(fullPath, content);
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
        string projectDir = Path.GetDirectoryName(Path.GetFullPath(projectName));
        string resxPath = Path.Combine(projectDir, "Properties", resourceName);

        ResxTypedClassGenerator.GenerateDesigner(resxPath, targetNamespace);
    }
}
