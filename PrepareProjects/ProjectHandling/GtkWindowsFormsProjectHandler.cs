using Prepare.Common.IO;
using Prepare.Common.Languages;
using PrepareProjects.Preparation.Directories;
using PrepareProjects.Preparation.Source;

namespace PrepareProjects.ProjectHandling;

public class GtkWindowsFormsProjectHandler
{
    public static async Task<(string targetProjectDirectory, string sourceProjectPath, string csprojPath)> PrepareAsync(string? sourceProjectName, string? targetProjectName)
    {
        var (targetProjectDirectory, sourceProjectPath) = await ProjectPreparation.PrepareTargetProjectFiles(sourceProjectName, targetProjectName);

        var csprojPath = ProjectPatching.PatchProject(targetProjectDirectory, sourceProjectPath, CopyResources);

        ProjectTranslation.TranslateFiles(sourceProjectName, targetProjectName, targetProjectDirectory,
            ProjectPatching.PatchProjectNamespace, "Resources", "translations");

        ConsoleColor.White.Write("Preparation completed successfully.", true);
        Directory.Delete(Path.Combine(targetProjectDirectory, "Properties\\PublishProfiles\\"), true);
        return (targetProjectDirectory, sourceProjectPath, csprojPath);
    }

    private static void CopyResources(string targetProjectDirectory)
    {
        ProjectUtility.CopyLocalizationResourcesFiles(Path.Combine(targetProjectDirectory, "Properties"),
            "Resources.Designer.cs", "Resources.xml", "Resources.zh-Hans.xml",
            "Resources.it.xml", "Resources.fr.xml", "Resources.de.xml", "Resources.es.xml");
        ProjectUtility.CopyLocalizationResourcesFiles(Path.Combine(targetProjectDirectory, "Properties"),
            "MessageBoxResources.Designer.cs", "MessageBoxResources.xml", "MessageBoxResources.zh-Hans.xml",
            "MessageBoxResources.it.xml", "MessageBoxResources.fr.xml", "MessageBoxResources.de.xml", "MessageBoxResources.es.xml");
    }
}
