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
            throw new InvalidOperationException("PostProcessing.xml has no root element.");

        foreach (var replacement in replacements)
        {
            var itemElement = new XElement("Item",
                new XElement("FileName", fileItem.Source),
                new XElement("SourceString", replacement.OldText),
                new XElement("TargetString", replacement.NewText)
            );

            root.Add(itemElement);
        }

        document.Save(_postProcessingPath);
    }
}