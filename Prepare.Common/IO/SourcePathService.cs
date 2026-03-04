using Prepare.Common.Logging;
using Prepare.Common.Models;

namespace Prepare.Common.IO;

internal class SourcePathService
{
    public static bool GetCurrentNewFullPath(string csprojDirectory, FileItem item, out string currentNew)
    {
        // Build currentNew
        currentNew = Path.Combine(csprojDirectory, item.Destination);

        if (!File.Exists(currentNew))
        {
            YellowConsole.Write($"Skipping (not found): {currentNew}");
            return false;
        }

        return true;
    }

    public static string GetCurrentOldFullPath(string fileName, string contents)
    {
        // Build currentOld in %TEMP%
        var tempPath = Path.GetTempPath();
        var currentOld = Path.Combine(tempPath, fileName);

        if (File.Exists(currentOld))
        {
            File.Delete(currentOld);
        }

        File.WriteAllText(currentOld, contents);
        return currentOld;
    }
}
