using System.Xml.Linq;
using Prepare.Common.IO;

namespace Prepare.Common.Projects
{
    internal static class ReorderingPersister
    {
        internal static void AddInnerTextFromFileContent(
            string projectDirectory,
            KeyValuePair<string, string> kvp,
            XElement itemElement, string backupName)
        {
            if (kvp.Key.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                string fullPath = Path.Combine(projectDirectory, kvp.Value);

                if (File.Exists(fullPath))
                {
                    var destFileName = BackupUtility.GetDestFileName(fullPath, backupName);
                    itemElement.Value = destFileName;
                }
            }
        }
    }
}
