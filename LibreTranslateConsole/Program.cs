using System.Net.Http.Json;
using System.Xml.Linq;

namespace LibreTranslateConsole;

class Program
{
    private const string LibreTranslateUrl = "http://localhost:5000/translate";
    private const int SaveChunkSize = 10;

    static async Task Main()
    {
        string filePath = "..\\..\\..\\..\\ChineseExtractor\\JsonTranslations.xml";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("JsonTranslations.xml not found.");
            return;
        }

        XDocument doc = XDocument.Load(filePath);
        var entries = doc.Root!.Elements("Entry").ToList();

        using HttpClient http = new HttpClient();

        int translatedCount = 0;

        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];
            string chinese = entry.Element("Chinese")?.Value?.Trim();
            XElement englishElement = entry.Element("English");

            if (string.IsNullOrWhiteSpace(chinese))
                continue;

            if (!string.IsNullOrWhiteSpace(englishElement?.Value))
                continue; // already translated

            string translation = await TranslateAsync(http, chinese);

            englishElement.Value = translation;
            translatedCount++;

            Console.WriteLine($"Translated: {chinese} -> {translation}");

            if (translatedCount % SaveChunkSize == 0)
            {
                doc.Save(filePath);
                Console.WriteLine($"💾 Saved after {translatedCount} translations");
            }
        }

        // Final save
        doc.Save(filePath);
        Console.WriteLine("✅ Translation completed.");
    }

    private static async Task<string> TranslateAsync(HttpClient http, string text)
    {
        var request = new
        {
            q = text,
            source = "zh",
            target = "en",
            format = "text"
        };

        HttpResponseMessage response = await http.PostAsJsonAsync(LibreTranslateUrl, request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();
        return json?.translatedText ?? string.Empty;
    }

    private class LibreTranslateResponse
    {
        public string translatedText { get; set; }
    }
}