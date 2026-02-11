using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Prepare.Common;

namespace PrepareProjects;

public class GtkWindowsForms
{
    public static async Task<(string targetProjectDirectory, string sourceProjectPath)> PrepareAsync(string? sourceProjectName, string? targetProjectName)
    {
        var (targetProjectDirectory, sourceProjectPath) = await ProjectPreparation.PrepareTargetProjectFiles(sourceProjectName, targetProjectName);

        ProjectPatching.PatchProject(targetProjectDirectory, sourceProjectPath, CopyResources);

        ProjectTranslation.TranslateFiles(sourceProjectName, targetProjectName, targetProjectDirectory,
            ProjectPatching.PatchProjectNamespace, "Resources", "translations");

        Console.WriteLine("Preparation completed successfully.");
        return (targetProjectDirectory, sourceProjectPath);
    }

    private static void CopyResources(string targetProjectDirectory)
    {
        ProjectUtility.CopyLocalizationResourcesFiles(Path.Combine(targetProjectDirectory, "Properties"),
            "Resources.xml", "Resources.zh-Hans.xml",
            "Resources.it.xml", "Resources.fr.xml", "Resources.de.xml", "Resources.es.xml");
    }
}