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

        var targetDir = await GtkWindowsForms.PrepareAsync("GTKSystem.Windows.Forms", "Net4x.Gtk.Windows.Forms");
        SystemResourcesExtensions.Prepare();
        var version = VersionPrepare.PrepareSolutionVersion();

        var projectName = await GtkSampleApp.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName = await GtkSampleApp.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        PostProcessing.ApplyPostProcessing(targetDir);

        // --- Run resgen for each project ---
        RunResGen(projectName);
        RunResGen(otherProjectName);
    }

    private static void RunResGen(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            Console.WriteLine("No project name specified for resgen.");
            return;
        }

        // Assume project path is the projectName directory
        string projectDir = Path.GetDirectoryName(Path.GetFullPath(projectName));
        string resxPath = Path.Combine(projectDir, "Properties", "GtkWinFormsAppResources.resx");

        ResxTypedClassGenerator.GenerateDesigner(resxPath, $"{Path.GetFileNameWithoutExtension(projectName)}.Properties");
    }
}
