using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ExtractStringsToTranslate;

public class Program
{
    static int Main(string[] args)
    {
        var rootDir =
            args.Length > 0
                ? Path.GetFullPath(args[0])
                : Directory.GetCurrentDirectory();

        if (!Directory.Exists(rootDir))
        {
            Console.WriteLine("Directory does not exist.");
            return 1;
        }

        string destination = "translations";
        if (rootDir.Contains("App"))
        {
            destination = "samplesTranslations";
        }
        string translationsPath = @$"..\..\..\{destination}.xml";

        var translationsDoc = XDocument.Load(translationsPath);
        var translationsRoot = translationsDoc.Root!;

        // ------------------------------------------------------------
        // Ensure <Name> exists for every Entry
        // ------------------------------------------------------------
        int namesAdded = 0;

        foreach (var entry in translationsRoot.Elements("Entry"))
        {
            if (entry.Element("Name") != null)
                continue;

            var english = entry.Element("English")?.Value;
            if (string.IsNullOrWhiteSpace(english))
                continue;

            var name = GenerateName(english);
            entry.AddFirst(new XElement("Name", name));
            namesAdded++;
        }

        translationsDoc.Save(translationsPath);

        // ------------------------------------------------------------
        // Update RESX files
        // ------------------------------------------------------------
        string resxEnPath;
        string resxZhPath;
        if (rootDir.Contains("App"))
        {
            resxEnPath = @"..\..\..\..\PrepareProjects\Resources\GtkWinFormsAppResources.xml";
            resxZhPath = @"..\..\..\..\PrepareProjects\Resources\GtkWinFormsAppResources.zh-Hans.xml";
        }
        else
        {
            resxEnPath = @"..\..\..\..\PrepareProjects\Resources\Resources.xml";
            resxZhPath = @"..\..\..\..\PrepareProjects\Resources\Resources.zh-Hans.xml";
        }

        int enAdded = UpdateResx(
            resxEnPath,
            translationsRoot,
            valueSelector: e => e.Element("English")?.Value
        );

        int zhAdded = UpdateResx(
            resxZhPath,
            translationsRoot,
            valueSelector: e => e.Element("Chinese")?.Value
        );

        Console.WriteLine($"Added {namesAdded} Name entries.");
        Console.WriteLine($"Added {enAdded} EN resources.");
        Console.WriteLine($"Added {zhAdded} ZH resources.");

        return 0;
    }

    // ------------------------------------------------------------
    // RESX helper
    // ------------------------------------------------------------

    static int UpdateResx(
        string resxPath,
        XElement translationsRoot,
        Func<XElement, string?> valueSelector)
    {
        var doc = XDocument.Load(resxPath);
        var root = doc.Root!;

        var existingNames = root
            .Elements("data")
            .Select(d => d.Attribute("name")?.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToHashSet();

        int added = 0;

        foreach (var entry in translationsRoot.Elements("Entry"))
        {
            var name = entry.Element("Name")?.Value;
            var value = valueSelector(entry);

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(value) ||
                existingNames.Contains(name))
                continue;

            root.Add(
                new XElement("data",
                    new XAttribute("name", name),
                    new XAttribute(XNamespace.Xml + "space", "preserve"),
                    new XElement("value", value)
                )
            );

            existingNames.Add(name);
            added++;
        }

        doc.Save(resxPath);
        return added;
    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    static string GenerateName(string english)
    {
        var name = Regex.Replace(english, @"[^a-zA-Z0-9]", "_");
        name = Regex.Replace(name, @"_+", "_");
        name = name.Trim('_');
        return $"Res_{name}";
    }
}
