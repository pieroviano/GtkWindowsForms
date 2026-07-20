using System.Xml.Linq;
using Prepare.Common.Languages;

namespace Prepare.Tests
{
    public class TranslationApplyGuardTests
    {
        [Fact]
        public void LoadTranslations_SkipsEmptyEnglishEntries()
        {
            var dir = Path.Combine(Path.GetTempPath(), "tr-guard-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var path = Path.Combine(dir, "Translations.xml");
                new XDocument(new XElement("Translations",
                    new XElement("Entry", new XElement("Chinese", "你好"), new XElement("English", "Hello")),
                    new XElement("Entry", new XElement("Chinese", "再见"), new XElement("English", ""))
                )).Save(path);

                var list = TranslationsHelper.LoadTranslations(path);

                Assert.Single(list);                    // the blank-English entry is skipped
                Assert.Equal("你好", list[0].Key);
                Assert.Equal("Hello", list[0].Value);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void Apply_BlankEnglishEntry_LeavesChineseInPlace()
        {
            var dir = Path.Combine(Path.GetTempPath(), "tr-guard-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var file = Path.Combine(dir, "A.cs");
                File.WriteAllText(file, "你好 再见");

                // Pass the blank-English entry straight to the apply layer (bypassing LoadTranslations)
                // to prove MakeRawTranslationOnFile itself does not erase untranslated Chinese.
                var raw = new List<KeyValuePair<string, string>>
                {
                    new("你好", "Hello"),
                    new("再见", "") // untranslated -> must be skipped, not erased
                };

                Translator.ReplaceStringsInFiles(
                    sourceProjectName: null,
                    targetDirectory: dir,
                    targetProjectName: null,
                    localizationResourceName: null,
                    patchProjectNamespace: (_, _, content) => content,
                    localizableItemsOrderedByValueLengthDecending: new List<KeyValuePair<string, string>>(),
                    rawTranslations: raw);

                var result = File.ReadAllText(file);
                Assert.Contains("Hello", result);   // filled entry applied
                Assert.Contains("再见", result);      // blank entry left intact, not erased
                Assert.DoesNotContain("你好", result);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
