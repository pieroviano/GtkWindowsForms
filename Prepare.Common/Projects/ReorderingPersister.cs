using System.Xml.Linq;
using Prepare.Common.IO;

namespace Prepare.Common.Projects;

internal static class ReorderingPersister
{
    internal static void AddBackupFilePathAsInnerText(
        string projectDirectory,
        KeyValuePair<string, string> keyValuePair,
        XElement itemElement, string backupName, string projectName)
    {
        if (keyValuePair.Key.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
            var fullPath = Path.Combine(projectDirectory, keyValuePair.Value);

            if (File.Exists(fullPath))
            {
                var destFileName = BackupUtility.GetDestFileName(fullPath, backupName, projectName);
                itemElement.Value = destFileName;
            }
        }
    }
}
