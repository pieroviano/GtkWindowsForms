using Prepare.Common.IO;
using UpdatePostProcessing.Extensions;
using UpdatePostProcessing.Model;
using UpdatePostProcessing.Services;

namespace UpdatePostProcessing;

public class ProcessFileUpdatesProcessor
{
    public static int ProcessFilesUpdates(string csprojPath, string backupName)
    {
        Console.WriteLine("Processing modified files");

        if (!File.Exists(csprojPath))
        {
            Console.Error.WriteLine($"File not found: {csprojPath}");
            return 1;
        }

        var filesXmlPath = ProjectUtility.GetFilesXmlPath(csprojPath, backupName);
        if (filesXmlPath == null)
        {
            return 1;
        }

        var items = FileItemsService.GetFileItems(filesXmlPath);

        var csprojDirectory = Path.GetDirectoryName(csprojPath);
#pragma warning disable CS8604 // Possible null reference argument.
        var files = Directory.GetFiles(csprojDirectory, "*.cs",
            SearchOption.AllDirectories);
#pragma warning restore CS8604 // Possible null reference argument.
        items.CompareFiles(csprojDirectory, files);

        return 0;
    }
}