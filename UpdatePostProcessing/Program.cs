using System.Diagnostics;
using System.Xml.Linq;
using UpdatePostProcessing.Extensions;
using UpdatePostProcessing.Model;
using UpdatePostProcessing.Services;

namespace UpdatePostProcessing;

internal class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Missing csproj path argument.");
            return 1;
        }

        var csprojPath = args[0];

        if (!File.Exists(csprojPath))
        {
            Console.Error.WriteLine($"File not found: {csprojPath}");
            return 1;
        }

        var filesXmlPath = Path.ChangeExtension(csprojPath, ".Files.xml");

        if (!File.Exists(filesXmlPath))
        {
            Console.Error.WriteLine($"Associated .Files.xml not found: {filesXmlPath}");
            return 1;
        }

        var items = FileItemsService.GetFileItems(filesXmlPath);

        var resourceFiles = GetCsResourceFiles(csprojPath, out var csprojDirectory);

        foreach (var resourceFile in resourceFiles)
        {
            var fileItem = items.GetItem(resourceFile, out var fileName);
            if (fileItem == null)
            {
                continue;
            }

            var contents = fileItem.GetContents(resourceFile, true);
            var replacements = fileItem.ProcessItem(csprojDirectory, fileName, contents);
            if (replacements != null)
            {
                var persister = new PostProcessingPersister();
                persister.PersistReplacements(replacements, fileItem);
            }
        }

        return 0;
    }

    private static string[] GetCsResourceFiles(string csprojPath, out string csprojDirectory)
    {
        csprojDirectory = Path.GetDirectoryName(csprojPath) ?? string.Empty;

        var resourcesPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PrepareProjects\Resources"));

        var resourceFiles = Directory
            .GetFiles(resourcesPath, "*.cs", SearchOption.TopDirectoryOnly).Where(
                f=>!f.EndsWith("GtkWinFormsAppResources.Designer.cs") &&
                   !f.EndsWith("ControlExtension.cs") &&
                   !f.EndsWith("GridCell.cs") &&
                   !f.EndsWith("System.ComponentModel.cs") &&
                   !f.EndsWith("System.Resources.ResourceManager.cs")).ToArray();
        return resourceFiles;
    }
}