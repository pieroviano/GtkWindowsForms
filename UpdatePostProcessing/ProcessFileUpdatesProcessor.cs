using UpdatePostProcessing.Extensions;
using UpdatePostProcessing.Model;
using UpdatePostProcessing.Services;

namespace UpdatePostProcessing;

public class ProcessFileUpdatesProcessor
{

    public static int ProcessFilesUpdates(string csprojPath)
    {
        Console.WriteLine("Processing modified files");

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

        CompareFiles(csprojDirectory, items, resourceFiles, true);

        var files = Directory.GetFiles(csprojDirectory, "*.cs", SearchOption.AllDirectories);
        CompareFiles(csprojDirectory, items, files, false);

        return 0;
    }

    private static void CompareFiles(string csprojDirectory, List<FileItem> items, string[] files, bool useResourceFile)
    {
        for (var index = 0; index < files.Length; index++)
        {
            if (index % 100 == 0)
            {
                Console.WriteLine($"[{index + (index == 0 ? 1 : 0)}/{files.Length}]");
            }

            var file = files[index];
            var fileItem = items.GetItem(file, out var fileName);
            if (fileItem == null)
            {
                continue;
            }

            var contents = fileItem.GetContents(file, useResourceFile);
            var replacements = fileItem.ProcessItem(csprojDirectory, fileName, contents);
            if (replacements != null)
            {
                replacements = replacements.Where(r =>
                    !string.IsNullOrWhiteSpace(r.OldText) &&
                    !string.IsNullOrWhiteSpace(r.NewText)
                    && r.NewText != r.OldText).ToList();
                var persister = new PostProcessingPersister();
                persister.PersistReplacements(replacements, fileItem);
            }
        }
    }

    private static string[] GetCsResourceFiles(string csprojPath, out string csprojDirectory)
    {
        csprojDirectory = Path.GetDirectoryName(csprojPath) ?? string.Empty;

        var resourcesPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PrepareProjects\Resources"));

        var resourceFiles = Directory
            .GetFiles(resourcesPath, "*.cs", SearchOption.TopDirectoryOnly).Where(f =>
                !f.EndsWith("GtkWinFormsAppResources.Designer.cs") &&
                !f.EndsWith("ControlExtension.cs") &&
                !f.EndsWith("GridCell.cs") &&
                !f.EndsWith("System.ComponentModel.cs") &&
                !f.EndsWith("System.Resources.ResourceManager.cs")).ToArray();
        return resourceFiles;
    }
}