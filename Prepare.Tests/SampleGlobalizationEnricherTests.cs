using System.Text;
using Prepare.Common.Languages;

namespace Prepare.Tests
{
    /// <summary>
    /// Phase 2 of AlignSamples-Plan.md: new Chinese from upstream must land somewhere fillable - a proposed
    /// resource key, a blank dictionary entry, or an English data-file sibling - without ever disturbing
    /// what is already there.
    /// </summary>
    public class SampleGlobalizationEnricherTests
    {
        private const string ResxHeader =
            "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n";

        private static string NewTemp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "sge-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void SafeDelete(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }
            catch
            {
                // best-effort temp cleanup
            }
        }

        /// <summary>A repo root with the overlay dictionaries and one upstream sample directory.</summary>
        private static (string root, string sampleDir) BuildRoot(string? existingZhHansEntries = null,
            string? existingDictionaryEntries = null)
        {
            var root = NewTemp();
            var resources = Path.Combine(root, "Resources");
            Directory.CreateDirectory(resources);

            WriteUtf8Bom(Path.Combine(resources, "GtkWinFormsAppResources.zh-Hans.xml"),
                ResxHeader + (existingZhHansEntries ?? string.Empty) + "</root>");
            WriteUtf8Bom(Path.Combine(resources, "GtkWinFormsAppResources.xml"),
                ResxHeader + (existingZhHansEntries == null ? string.Empty : BlankOut(existingZhHansEntries)) + "</root>");
            WriteUtf8Bom(Path.Combine(resources, "samplesTranslations.xml"),
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>\r\n<Translations>\r\n"
                + (existingDictionaryEntries ?? string.Empty) + "</Translations>");

            var sampleDir = Path.Combine(root, "gtksystem-windows-forms", "Samples", "GTKWinFormsApp");
            Directory.CreateDirectory(sampleDir);
            return (root, sampleDir);
        }

        private static void WriteUtf8Bom(string path, string content) =>
            File.WriteAllText(path, content.TrimStart('﻿'), new UTF8Encoding(true));

        private static string BlankOut(string resxEntries) =>
            System.Text.RegularExpressions.Regex.Replace(resxEntries, "<value>[^<]*</value>", "<value></value>");

        private static string Resx(string root, string suffix = ".zh-Hans") =>
            File.ReadAllText(Path.Combine(root, "Resources", $"GtkWinFormsAppResources{suffix}.xml"));

        private static string Dictionary(string root) =>
            File.ReadAllText(Path.Combine(root, "Resources", "samplesTranslations.xml"));

        // --- key generation --------------------------------------------------

        [Fact]
        public void StringLiterals_BecomeProposedKeys_InTheHandAuthoredStyle()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.Designer.cs"),
                    "this.button1.Text = \"datatable数据源\";\r\nthis.label1.Text = \"打印\";\r\n");

                var result = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                // Hint from the ASCII content when there is any; a per-form ordinal otherwise.
                Assert.Equal(new[] { "Gtk_MainForm_datatable", "Gtk_MainForm_1" }, result.NewKeys.Select(k => k.Key));
                Assert.Equal(new[] { "datatable数据源", "打印" }, result.NewKeys.Select(k => k.Chinese));
                Assert.All(result.NewKeys, k => Assert.True(k.Line > 0));

                // zh-Hans gets the Chinese; the invariant gets the SAME key with a blank value to fill.
                Assert.Contains("<data name=\"Gtk_MainForm_datatable\" xml:space=\"preserve\">", Resx(root));
                Assert.Contains("<value>datatable数据源</value>", Resx(root));
                Assert.Contains("<data name=\"Gtk_MainForm_datatable\" xml:space=\"preserve\">", Resx(root, string.Empty));
                Assert.DoesNotContain("datatable数据源", Resx(root, string.Empty));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void KeyCollisions_GetOrdinalSuffixes()
        {
            var existing =
                "  <data name=\"Gtk_MainForm_print\" xml:space=\"preserve\">\r\n    <value>别的</value>\r\n  </data>\r\n";
            var (root, sampleDir) = BuildRoot(existing);
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.cs"),
                    "var a = \"print打印\";\r\nvar b = \"print打印机\";\r\n");

                var result = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                Assert.Equal(new[] { "Gtk_MainForm_print_2", "Gtk_MainForm_print_3" }, result.NewKeys.Select(k => k.Key));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void AlreadyKeyedChinese_IsNotProposedAgain()
        {
            var existing =
                "  <data name=\"Gtk_MainForm_Print\" xml:space=\"preserve\">\r\n    <value>打印</value>\r\n  </data>\r\n";
            var (root, sampleDir) = BuildRoot(existing);
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.cs"), "var a = \"打印\";\r\n");

                var result = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                Assert.Empty(result.NewKeys);
                Assert.False(result.HasWork);
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void CommentsAndInlineChinese_GoToTheDictionary_NotTheResx()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "Form1.cs"),
                    "//打印\r\nvar x = 1; // 数据源说明\r\n");

                var result = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                Assert.Empty(result.NewKeys);
                Assert.Equal(new[] { "打印", "数据源说明" }, result.NewDictionaryEntries);
                var dictionary = Dictionary(root);
                Assert.Contains("<Chinese>打印</Chinese>", dictionary);
                Assert.Contains("<English></English>", dictionary);   // blank: filled by hand at the pause
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void ResxValues_AreTreatedAsUiStrings()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "Form1.resx"),
                    "<root>\r\n  <data name=\"btn.Text\"><value>确定</value></data>\r\n</root>");

                var result = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                Assert.Single(result.NewKeys);
                Assert.Equal("确定", result.NewKeys[0].Chinese);
                Assert.StartsWith("Gtk_Form1_", result.NewKeys[0].Key);
            }
            finally
            {
                SafeDelete(root);
            }
        }

        // --- additivity ------------------------------------------------------

        [Fact]
        public void ExistingEntriesAndBom_AreLeftUntouched()
        {
            var existing =
                "  <data name=\"Gtk_MainForm_Kept\" xml:space=\"preserve\">\r\n    <value>保留</value>\r\n  </data>\r\n";
            var existingDictionary =
                "\t<Entry>\r\n\t\t<Chinese>保留</Chinese>\r\n\t\t<English>Kept</English>\r\n\t</Entry>\r\n";
            var (root, sampleDir) = BuildRoot(existing, existingDictionary);
            var zhHansPath = Path.Combine(root, "Resources", "GtkWinFormsAppResources.zh-Hans.xml");
            var before = File.ReadAllText(zhHansPath);
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.cs"), "var a = \"新的\"; //注释\r\n");

                SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                var after = File.ReadAllText(zhHansPath);
                // Everything that was there is still there, byte for byte, in the same order.
                Assert.StartsWith(before.Substring(0, before.LastIndexOf("</root>", StringComparison.Ordinal)), after);
                Assert.Contains("<value>新的</value>", after);
                Assert.EndsWith("</root>", after);

                var bom = File.ReadAllBytes(zhHansPath);
                Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bom.Take(3));

                // A filled dictionary entry is cached forever - never re-added or blanked.
                var dictionary = Dictionary(root);
                Assert.Contains("<English>Kept</English>", dictionary);
                Assert.Equal(1, CountOccurrences(dictionary, "<Chinese>保留</Chinese>"));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void RunningTwice_AddsNothingTheSecondTime()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.cs"), "var a = \"新的\"; //注释\r\n");

                SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });
                var afterFirst = Resx(root) + Dictionary(root);

                var second = SampleGlobalizationEnricher.EnrichSampleResources(root, new[] { sampleDir });

                Assert.False(second.HasWork);
                Assert.Equal(afterFirst, Resx(root) + Dictionary(root));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        // --- data files ------------------------------------------------------

        [Fact]
        public void ChineseDataFiles_GetAnEnglishSiblingInTheOverlay()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "TestData2.json"), "{ \"name\": \"张三\" }");
                File.WriteAllText(Path.Combine(sampleDir, "readme-en.txt"), "no chinese here");

                var gaps = SampleGlobalizationEnricher.EnrichSampleDataFiles(root, new[] { sampleDir });

                var gap = Assert.Single(gaps);
                Assert.True(gap.Created);
                Assert.EndsWith("TestData2.en.json", gap.EnglishSibling);
                // Both the Chinese original and the sibling now live in the overlay the transform copies from.
                Assert.True(File.Exists(Path.Combine(root, "Resources", "TestData2.json")));
                // Seeded with the Chinese content so the file stays valid until it is translated.
                Assert.Contains("张三", File.ReadAllText(gap.EnglishSibling));
                // A file with no Chinese is not a culture-specific asset.
                Assert.False(File.Exists(Path.Combine(root, "Resources", "readme-en.en.txt")));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void ExistingSibling_IsNeverOverwritten_ButAGapIsReported()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "TestData2.json"), "{ \"name\": \"张三\" }");
                File.WriteAllText(Path.Combine(root, "Resources", "TestData2.json"), "{ \"name\": \"张三\" }");
                File.WriteAllText(Path.Combine(root, "Resources", "TestData2.en.json"), "{ \"name\": \"Zhang San\" }");

                var translated = SampleGlobalizationEnricher.EnrichSampleDataFiles(root, new[] { sampleDir });
                Assert.Empty(translated);   // fully translated -> nothing to do

                // A partially-filled sibling is reported, and left exactly as the operator wrote it.
                File.WriteAllText(Path.Combine(root, "Resources", "TestData2.en.json"), "{ \"name\": \"张三\", \"ok\": 1 }");
                var gaps = SampleGlobalizationEnricher.EnrichSampleDataFiles(root, new[] { sampleDir });

                var gap = Assert.Single(gaps);
                Assert.False(gap.Created);
                Assert.Equal("{ \"name\": \"张三\", \"ok\": 1 }",
                    File.ReadAllText(Path.Combine(root, "Resources", "TestData2.en.json")));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void FilesTheTransformDeletes_GetNoTranslationSlot()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                // The transform drops this file, so nothing it contains ever reaches a generated sample.
                File.WriteAllText(Path.Combine(sampleDir, "工程说明.txt"), "项目说明");

                var gaps = SampleGlobalizationEnricher.EnrichSampleDataFiles(root, new[] { sampleDir });

                Assert.Empty(gaps);
                Assert.False(File.Exists(Path.Combine(root, "Resources", "工程说明.en.txt")));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public void Enrich_CombinesBothHalves_ForTheManualPause()
        {
            var (root, sampleDir) = BuildRoot();
            try
            {
                File.WriteAllText(Path.Combine(sampleDir, "MainForm.cs"), "var a = \"新的\"; //注释\r\n");
                File.WriteAllText(Path.Combine(sampleDir, "TestData2.json"), "{ \"name\": \"张三\" }");

                var result = SampleGlobalizationEnricher.Enrich(root, new[] { sampleDir });

                Assert.True(result.HasWork);
                Assert.Single(result.NewKeys);
                Assert.Single(result.NewDictionaryEntries);
                Assert.Single(result.DataFiles);

                var listing = string.Join(Environment.NewLine, result.Describe());
                Assert.Contains("Gtk_MainForm_1", listing);
                Assert.Contains("MainForm.cs:1", listing);
                Assert.Contains("TestData2.en.json", listing);
            }
            finally
            {
                SafeDelete(root);
            }
        }

        private static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var index = 0;
            while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }

            return count;
        }
    }
}
