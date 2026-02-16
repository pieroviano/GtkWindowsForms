using System.Xml.Linq;
using UpdatePostProcessing.Model;

namespace UpdatePostProcessing.Services;

internal class PostProcessingPersister
{
    private readonly string _postProcessingPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\PrepareProjects\PostProcessing.xml"));

    public void PersistReplacements(IEnumerable<TextReplacement> replacements, FileItem fileItem)
    {
        if (!File.Exists(_postProcessingPath))
        {
            // Create new document if not exists
            var newDoc = new XDocument(new XElement("Items"));
            newDoc.Save(_postProcessingPath);
        }

        var document = XDocument.Load(_postProcessingPath);

        var root = document.Root;

        if (root == null)
        {
            throw new InvalidOperationException("PostProcessing.xml has no root element.");
        }

        var toTersist = replacements.Select(replacement =>
            new ToPersist(fileItem.Source, fileItem.Destination, replacement.OldText,
                replacement.NewText)).GetMinimalDiffs();
        var modified = false;

        foreach (var replacement in toTersist)
        {
            if (replacement.SourceString == "TK" || replacement.TargetString == "tk")
            {
                continue;
            }

            var itemElement = new XElement("Item",
                new XElement("FileName", replacement.SourceFileName),
                new XElement("SourceString", replacement.SourceString),
                new XElement("TargetString", replacement.TargetString)
            );

            if (!ItemExists(
                    root,
                    replacement.SourceFileName,
                    replacement.SourceString,
                    replacement.TargetString))
            {
                Console.WriteLine($"Differences found for {replacement.SourceFileName}: {itemElement}");
                root.Add(itemElement);
                modified = true;
            }
        }

        if (modified)
        {
            document.Save(_postProcessingPath);
        }
    }

    private static bool ItemExists(
        XElement root,
        string fileName,
        string sourceString,
        string targetString)
    {
        return root.Elements("Item").Any(item =>
            (string?)item.Element("FileName") == fileName &&
            ((string?)item.Element("SourceString"))?.Replace("\r\n","\n") == sourceString.Replace("\r\n", "\n") &&
            ((string?)item.Element("TargetString"))?.Replace("\r\n", "\n") == targetString.Replace("\r\n", "\n"));
    }
}