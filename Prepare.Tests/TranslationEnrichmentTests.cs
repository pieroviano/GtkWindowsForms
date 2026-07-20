using System.Xml.Linq;
using Prepare.Common.Languages;

namespace Prepare.Tests
{
    public class TranslationEnrichmentTests
    {
        private static (string dict, string srcDir, string root) Setup(string existingChinese, string existingEnglish)
        {
            var root = Path.Combine(Path.GetTempPath(), "tr-enrich-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            var srcDir = Path.Combine(root, "src");
            Directory.CreateDirectory(srcDir);

            var dict = Path.Combine(root, "Translations.xml");
            File.WriteAllText(dict,
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>\r\n" +
                "<Translations>\r\n" +
                "\t<Entry>\r\n" +
                $"\t\t<Chinese>{existingChinese}</Chinese>\r\n" +
                $"\t\t<English>{existingEnglish}</English>\r\n" +
                "\t</Entry>\r\n" +
                "</Translations>\r\n");
            return (dict, srcDir, root);
        }

        private static List<KeyValuePair<string, string>> Entries(string dict) =>
            XDocument.Load(dict).Root!
                .Elements("Entry")
                .Select(e => new KeyValuePair<string, string>(
                    e.Element("Chinese")!.Value, e.Element("English")?.Value ?? ""))
                .ToList();

        [Fact]
        public void EnrichTranslationsDictionary_AppendsNewChineseAsBlank_PreservesExisting()
        {
            var (dict, srcDir, root) = Setup("你好", "Hello");
            try
            {
                // Source contains the existing Chinese plus a new one, embedded in code text.
                File.WriteAllText(Path.Combine(srcDir, "A.cs"), "// 你好 comment\nvar s = \"再见\"; // farewell");

                var added = TranslationEnrichment.EnrichTranslationsDictionary(dict, new[] { srcDir });

                Assert.Equal(1, added); // only the new "再见"
                var entries = Entries(dict);
                Assert.Equal(2, entries.Count);
                Assert.Equal("Hello", entries.Single(e => e.Key == "你好").Value); // preserved, not lost
                Assert.Equal("", entries.Single(e => e.Key == "再见").Value);       // new, blank
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        [Fact]
        public void EnrichTranslationsDictionary_NoNewChinese_IsNoOp()
        {
            var (dict, srcDir, root) = Setup("你好", "Hello");
            try
            {
                File.WriteAllText(Path.Combine(srcDir, "A.cs"), "// 你好 only");
                var before = File.ReadAllText(dict);

                var added = TranslationEnrichment.EnrichTranslationsDictionary(dict, new[] { srcDir });

                Assert.Equal(0, added);
                Assert.Equal(before, File.ReadAllText(dict)); // file untouched byte-for-byte
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        [Fact]
        public void EnrichTranslationsDictionary_IgnoresBinAndObj()
        {
            var (dict, srcDir, root) = Setup("你好", "Hello");
            try
            {
                var objDir = Path.Combine(srcDir, "obj");
                Directory.CreateDirectory(objDir);
                File.WriteAllText(Path.Combine(objDir, "Generated.cs"), "// 垃圾 in obj");

                var added = TranslationEnrichment.EnrichTranslationsDictionary(dict, new[] { srcDir });

                Assert.Equal(0, added); // obj content ignored
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
