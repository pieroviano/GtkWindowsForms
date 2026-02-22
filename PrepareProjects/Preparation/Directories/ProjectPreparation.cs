using Prepare.Common.Content;
using Prepare.Common.IO;
using PrepareProjects.Preparation.Source;

namespace PrepareProjects.Preparation.Directories;

public static class ProjectPreparation
{
    public static async Task<(string targetProjectDirectory, string csprojPath)> PrepareTargetProjectFiles(string? sourceProjectName, 
        string? targetProjectName)
    {
        var targetProjectDirectory = ProjectUtility.CopyProjectAndTranslateIt(sourceProjectName, targetProjectName, 
            ProjectPatching.PatchProjectNamespace);

        Normalizer.NormalizeNetstandardSymbolRemovingVersion(targetProjectDirectory);

        var csprojPath = RenameProjectFiles(targetProjectDirectory);
        await ProjectUtility.EnsureLatestGtkSharpAsync(csprojPath);
        return (targetProjectDirectory, csprojPath);
    }

    private static void RenameIfExists(string sourcePath, string targetPath)
    {
        if (File.Exists(sourcePath))
        {
            File.Move(sourcePath, targetPath, true);
        }
    }

    private static string RenameProjectFiles(string targetDir)
    {
        var oldCsproj = Path.Combine(targetDir, "GTKSystem.Windows.Forms.csproj");
        var newCsproj = Path.Combine(targetDir, "Net4x.Gtk.Windows.Forms.csproj");

        RenameIfExists(oldCsproj, newCsproj);

        RenameIfExists(
            Path.Combine(targetDir, "GTKSystem.Windows.Forms.csproj.user"),
            Path.Combine(targetDir, "Net4x.Gtk.Windows.Forms.csproj.user"));

        return newCsproj;
    }
}