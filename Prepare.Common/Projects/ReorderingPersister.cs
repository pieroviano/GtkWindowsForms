using System.Xml.Linq;
using Prepare.Common.IO;

namespace Prepare.Common.Projects
{
    internal static class ReorderingPersister
    {
        public static void PersistProjectFiles(
            string csprojPath,
            Dictionary<string, string> projectFiles,
            string projectDirectory)
        {
            XElement root = new XElement("Files");
            BackupUtility.CleanBackup(projectDirectory);
            foreach (var kvp in projectFiles)
            {
                var itemElement = new XElement("Item",
                    new XAttribute("Source", kvp.Key),
                    new XAttribute("Destination", kvp.Value)
                );

                AddInnerTextFromFileContent(projectDirectory, kvp, itemElement);

                root.Add(itemElement);
            }

            string outputPath = Path.ChangeExtension(csprojPath, ".Files.xml");

            XDocument doc = new XDocument(root);
            doc.Save(outputPath);
        }

        private static void AddInnerTextFromFileContent(
            string projectDirectory,
            KeyValuePair<string, string> kvp,
            XElement itemElement)
        {
            if (kvp.Key.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                string fullPath = Path.Combine(projectDirectory, kvp.Value);

                if (File.Exists(fullPath))
                {
                    var destFileName = BackupUtility.GetDestFileName(fullPath);
                    itemElement.Value = destFileName;
                }
            }
        }
    }
}
