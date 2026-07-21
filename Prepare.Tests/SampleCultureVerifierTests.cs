using System.Text;
using Prepare.Common.Languages;

namespace Prepare.Tests
{
    /// <summary>
    /// Phase 3 of AlignSamples-Plan.md: after the merge the samples must still be culture-aware -
    /// identical key sets, no blank English, no orphan references, no stray Chinese.
    /// </summary>
    public class SampleCultureVerifierTests
    {
        private static string NewTemp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "scv-" + Guid.NewGuid().ToString("N"));
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

        private static string Resx(params (string key, string value)[] entries)
        {
            var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n");
            foreach (var (key, value) in entries)
            {
                sb.Append($"  <data name=\"{key}\" xml:space=\"preserve\">\r\n    <value>{value}</value>\r\n  </data>\r\n");
            }

            return sb.Append("</root>").ToString();
        }

        /// <summary>A generated sample that satisfies the invariant; each test then breaks one thing.</summary>
        private static string BuildSample(string? invariantResx = null, string? zhHansResx = null, string? code = null)
        {
            var root = NewTemp();
            var sample = Path.Combine(root, "GtkWinFormsApp");
            Directory.CreateDirectory(Path.Combine(sample, "Properties"));

            File.WriteAllText(Path.Combine(sample, "Properties", "GtkWinFormsAppResources.resx"),
                invariantResx ?? Resx(("Gtk_MainForm_Print", "Print")), new UTF8Encoding(true));
            File.WriteAllText(Path.Combine(sample, "Properties", "GtkWinFormsAppResources.zh-Hans.resx"),
                zhHansResx ?? Resx(("Gtk_MainForm_Print", "打印")), new UTF8Encoding(true));
            File.WriteAllText(Path.Combine(sample, "MainForm.cs"),
                code ?? "button1.Text = Properties.GtkWinFormsAppResources.Gtk_MainForm_Print;\r\n",
                new UTF8Encoding(true));

            return sample;
        }

        // --- tests -----------------------------------------------------------

        [Fact]
        public void CultureAwareSample_Passes()
        {
            var sample = BuildSample();
            try
            {
                Assert.Empty(SampleCultureVerifier.Verify(new[] { sample }));
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void MissingZhHansKey_Fails()
        {
            var sample = BuildSample(
                invariantResx: Resx(("Gtk_MainForm_Print", "Print"), ("Gtk_MainForm_Save", "Save")));
            try
            {
                var failures = SampleCultureVerifier.Verify(new[] { sample });

                var failure = Assert.Single(failures);
                Assert.Contains("Gtk_MainForm_Save", failure);
                Assert.Contains("not in zh-Hans", failure);
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void KeyOnlyInZhHans_Fails()
        {
            var sample = BuildSample(
                zhHansResx: Resx(("Gtk_MainForm_Print", "打印"), ("Gtk_MainForm_Save", "保存")));
            try
            {
                var failure = Assert.Single(SampleCultureVerifier.Verify(new[] { sample }));
                Assert.Contains("exists in zh-Hans but not in the invariant", failure);
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void BlankInvariantValue_Fails()
        {
            // The exact state Phase 2 leaves behind when the operator does not fill the English in.
            var sample = BuildSample(invariantResx: Resx(("Gtk_MainForm_Print", "")));
            try
            {
                var failure = Assert.Single(SampleCultureVerifier.Verify(new[] { sample }));
                Assert.Contains("blank", failure);
                Assert.Contains("Gtk_MainForm_Print", failure);
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void OrphanCodeReference_Fails()
        {
            var sample = BuildSample(
                code: "button1.Text = Properties.GtkWinFormsAppResources.Gtk_MainForm_Nonexistent;\r\n");
            try
            {
                var failures = SampleCultureVerifier.Verify(new[] { sample });

                Assert.Equal(2, failures.Count);   // missing from both resources
                Assert.All(failures, f => Assert.Contains("Gtk_MainForm_Nonexistent", f));
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void ResidualChinese_Fails_ButTheChineseCultureFilesAreWhitelisted()
        {
            var sample = BuildSample(code: "// 未翻译的注释\r\n");
            try
            {
                // The Chinese-culture data asset and its English sibling: the pair IS the culture
                // mechanism, so the Chinese half must not be reported.
                File.WriteAllText(Path.Combine(sample, "TestData1.json"), "{ \"name\": \"张三\" }", new UTF8Encoding(true));
                File.WriteAllText(Path.Combine(sample, "TestData1.en.json"), "{ \"name\": \"Zhang San\" }", new UTF8Encoding(true));

                var failures = SampleCultureVerifier.Verify(new[] { sample });

                var failure = Assert.Single(failures);
                Assert.Contains("MainForm.cs", failure);
                Assert.DoesNotContain(failures, f => f.Contains("TestData1"));
                // ...and neither is the zh-Hans resource, which is nothing but Chinese.
                Assert.DoesNotContain(failures, f => f.Contains("zh-Hans.resx"));
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }

        [Fact]
        public void MissingLocalizationResource_Fails()
        {
            var sample = BuildSample();
            try
            {
                File.Delete(Path.Combine(sample, "Properties", "GtkWinFormsAppResources.zh-Hans.resx"));

                var failure = Assert.Single(SampleCultureVerifier.Verify(new[] { sample }));
                Assert.Contains("not culture-aware", failure);
            }
            finally
            {
                SafeDelete(Path.GetDirectoryName(sample)!);
            }
        }
    }
}
