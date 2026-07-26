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
        var newCsproj = Path.Combine(targetDir, "Gtk.Windows.Forms.csproj");

        RenameIfExists(oldCsproj, newCsproj);

        RenameIfExists(
            Path.Combine(targetDir, "GTKSystem.Windows.Forms.csproj.user"),
            Path.Combine(targetDir, "Gtk.Windows.Forms.csproj.user"));

        if (!File.Exists(newCsproj))
        {
            // Neither the renamed nor the original project file is present: the upstream project
            // may have been renamed. Fail loud so downstream csproj-path assumptions don't break obscurely.
            PathGuard.RequireFile(oldCsproj,
                "rename GTKSystem.Windows.Forms.csproj -> Gtk.Windows.Forms.csproj");
        }

        return newCsproj;
    }
}