using Prepare.Common.Extensions;
using Prepare.Common.IO;
using Prepare.Common.Logging;

namespace Prepare.Common.Services;

public class FileUpdatesService
{
    public static int Process(string csprojPath, string backupName)
    {
        ConsoleColor.White.Write("Processing modified files", true);

        if (!File.Exists(csprojPath))
        {
            RedConsole.Write($"File not found: {csprojPath}");
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
