using System.Xml.Linq;
using Prepare.Common.Languages;

namespace Prepare.Tests
{
    public class ResxLocaleEnricherTests
    {
        private static string WriteResx(string dir, string name, params (string key, string value)[] data)
        {
            var root = new XElement("root");
            foreach (var (key, value) in data)
            {
                root.Add(new XElement("data",
                    new XAttribute("name", key),
                    new XElement("value", value)));
            }

            var path = Path.Combine(dir, name);
            new XDocument(new XDeclaration("1.0", "utf-8", null), root).Save(path);
            return path;
        }

        private static Dictionary<string, string> ReadData(string path)
        {
            return XDocument.Load(path).Root!
                .Elements("data")
                .ToDictionary(
                    d => (string)d.Attribute("name")!,
                    d => d.Element("value")?.Value ?? string.Empty);
        }

        [Fact]
        public void EnsureBlankEntriesForSourceKeys_AddsMissingKeysAsBlank_PreservesExisting()
        {
            var dir = Path.Combine(Path.GetTempPath(), "resx-enrich-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var zhHans = WriteResx(dir, "R.zh-Hans.xml", ("Greeting", "你好"), ("Bye", "再见"));
                var en = WriteResx(dir, "R.xml", ("Greeting", "Hello")); // missing "Bye"

                var added = ResxLocaleEnricher.EnsureBlankEntriesForSourceKeys(zhHans, new[] { en });

                Assert.Equal(1, added);
                var result = ReadData(en);
                Assert.Equal("Hello", result["Greeting"]); // existing preserved
                Assert.True(result.ContainsKey("Bye"));     // missing added
                Assert.Equal(string.Empty, result["Bye"]);  // as blank
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void EnsureBlankEntriesForSourceKeys_WhenLocaleComplete_IsNoOp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "resx-enrich-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var zhHans = WriteResx(dir, "R.zh-Hans.xml", ("Greeting", "你好"));
                var en = WriteResx(dir, "R.xml", ("Greeting", "Hello"));

                var added = ResxLocaleEnricher.EnsureBlankEntriesForSourceKeys(zhHans, new[] { en });

                Assert.Equal(0, added);
                Assert.Equal("Hello", ReadData(en)["Greeting"]);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
