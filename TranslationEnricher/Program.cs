using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TranslationEnricher;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory path.");
            return;
        }

        string searchDirectory = args[0];

        if (!Directory.Exists(searchDirectory))
        {
            Console.WriteLine("Directory does not exist.");
            return;
        }

        string basePath = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\..\PrepareProjects"));

        string translationsPath = Path.Combine(basePath, "Translations.xml");
        string resourcesPath = Path.Combine(basePath, @"Resources\Resources.xml");
        string zhResourcesPath = Path.Combine(basePath, @"Resources\Resources.zh-Hans.xml");

        XDocument translationsDoc = XDocument.Load(translationsPath);
        XDocument resourcesDoc = XDocument.Load(resourcesPath);
        XDocument zhResourcesDoc = XDocument.Load(zhResourcesPath);

        var entries = translationsDoc.Root.Elements("Entry");

        foreach (var entry in entries)
        {
            string chinese = entry.Element("Chinese")?.Value?.Trim();
            string english = entry.Element("English")?.Value?.Trim();

            if (string.IsNullOrEmpty(chinese) || string.IsNullOrEmpty(english))
                continue;

            // 1️⃣ Check if Chinese text exists in .cs files
            if (!ChineseStringExistsInDirectory(searchDirectory, chinese))
                continue;

            // 2️⃣ Check if Chinese value already exists in zh resource file
            if (ChineseValueExists(zhResourcesDoc, chinese))
                continue;

            // 3️⃣ Build key
            string resourceKey = BuildResourceKey(english);

            // 4️⃣ Add to both resource files
            AddResource(resourcesDoc, resourceKey, english);
            AddResource(zhResourcesDoc, resourceKey, chinese);

            Console.WriteLine($"Added resource: {resourceKey}");
        }

        resourcesDoc.Save(resourcesPath);
        zhResourcesDoc.Save(zhResourcesPath);

        Console.WriteLine("Completed.");
    }

    static bool ChineseStringExistsInDirectory(string directory, string chineseText)
    {
        string pattern = $"\"{Regex.Escape(chineseText)}\"";

        foreach (var file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
        {
            string content = File.ReadAllText(file);
            if (content.Contains(pattern))
                return true;
        }

        return false;
    }

    static bool ChineseValueExists(XDocument doc, string chineseText)
    {
        return doc.Root.Elements("data")
            .Any(e => e.Element("value")?.Value.Trim() == chineseText);
    }

    static string BuildResourceKey(string input)
    {
        string result = Regex.Replace(input, @"[^a-zA-Z0-9]+", "_");
        result = result.Trim('_');
        return result;
    }

    static void AddResource(XDocument doc, string key, string value)
    {
        XElement dataElement = new XElement("data",
            new XAttribute("name", key),
            new XAttribute(XNamespace.Xml + "space", "preserve"),
            new XElement("value", value));

        doc.Root.Add(dataElement);
    }
}