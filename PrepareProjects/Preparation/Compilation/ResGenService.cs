using Prepare.Common.Logging;

namespace PrepareProjects.Preparation.Compilation;

public static class ResGenService
{
    public static void RegenerateSrDesigner(string projectDirectory, string csprojPath)
    {
        var currentDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.Combine(projectDirectory, "GtkSystem\\Resources\\Extensions");
        RunResGen(csprojPath, "SR.resx",
            "GtkSystem", "GtkSystem\\Resources\\Extensions");
        Environment.CurrentDirectory = currentDirectory;
    }

    public static void RegenerateDesignersFromResources(string targetDir, string projectName, string otherProjectName)
    {
        var currentDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = Path.Combine(targetDir, "Properties");
        var combine = Path.Combine(targetDir, "Net4x.Gtk.Windows.Forms.csproj");
        RunResGen(combine, "Resources.resx", "System.Properties", "Properties");
        RunResGen(projectName, "GtkWinFormsAppResources.resx",
            $"{Path.GetFileNameWithoutExtension(projectName)}.Properties", "Properties");
        RunResGen(otherProjectName, "GtkWinFormsAppResources.resx",
            $"{Path.GetFileNameWithoutExtension(otherProjectName)}.Properties", "Properties");
        Environment.CurrentDirectory = currentDirectory;
    }

    private static void RunResGen(string projectName, string resourceName, string targetNamespace, string partToCombine)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            RedConsole.Write("No project name specified for resgen.");
            return;
        }

        // Assume project path is the projectName directory
        var projectDir = Path.GetDirectoryName(Path.GetFullPath(projectName));
#pragma warning disable CS8604 // Possible null reference argument.
        var resxPath = Path.Combine(projectDir, partToCombine, resourceName);
#pragma warning restore CS8604 // Possible null reference argument.

        ResxTypedClassGenerator.GenerateDesigner(resxPath, targetNamespace);
    }
}
