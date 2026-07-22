using Prepare.Common.Alignment;
using Prepare.Common.IO;
using PrepareProjects;

namespace Prepare.Tests
{
    /// <summary>
    /// Phase 1 of AlignSamples-Plan.md: <c>fs</c> must be callable against an arbitrary repo root, write
    /// nothing outside it, reproduce byte for byte, and emit an exact generated → upstream manifest.
    /// </summary>
    public class SampleTransformPipelineTests
    {
        private const string UpstreamSample = "FixtureSampleApp";
        private const string GeneratedSample = "GeneratedSampleApp";

        /// <summary>The real repository root - the fixture borrows its Resources overlay from it.</summary>
        private static string RealRepoRoot =>
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

        private static string NewTemp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "fs-" + Guid.NewGuid().ToString("N"));
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

        /// <summary>
        /// A minimal repo root: one upstream sample under the submodule path, plus the real
        /// <c>Resources</c> overlay files the transform reads (localization xml, data json, the Designer
        /// template and the sample translation dictionary).
        /// </summary>
        private static string BuildFixtureRoot()
        {
            var root = NewTemp();
            var sampleDir = Path.Combine(root, "gtksystem-windows-forms", "Samples", UpstreamSample);
            Directory.CreateDirectory(Path.Combine(sampleDir, "Resources"));

            File.WriteAllText(Path.Combine(sampleDir, $"{UpstreamSample}.csproj"),
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>WinExe</OutputType>
                    <TargetFramework>net8.0</TargetFramework>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="GTKSystem.Windows.Forms" Version="1.0.0" />
                  </ItemGroup>
                </Project>
                """);

            // Chinese comments straight from the upstream samples - the three the pipeline used to fix up
            // with hardcoded string replacements (now dictionary entries).
            File.WriteAllText(Path.Combine(sampleDir, "Form1.cs"),
                """
                namespace FixtureSampleApp
                {
                    public class Form1
                    {
                        public void Load()
                        {
                            //1、数据集列表数据源
                            //2、datatable数据源
                            //打印
                        }
                    }
                }
                """);

            File.WriteAllText(Path.Combine(sampleDir, "工程说明.txt"), "upstream notes");
            File.WriteAllText(Path.Combine(sampleDir, "Resources", "010.jpg"), "not really a jpeg");
            File.WriteAllText(Path.Combine(sampleDir, "TestData1.json"), "{ \"upstream\": true }");

            // The overlay the transform copies from - taken from the real repo so the fixture exercises
            // the actual resource files rather than stand-ins.
            var resources = Path.Combine(root, "Resources");
            Directory.CreateDirectory(resources);
            foreach (var name in new[]
                     {
                         "GtkWinFormsAppResources.xml", "GtkWinFormsAppResources.zh-Hans.xml",
                         "GtkWinFormsAppResources.Designer.cs", "TestData1.json", "TestData1.en.json",
                         "samplesTranslations.xml"
                     })
            {
                File.Copy(Path.Combine(RealRepoRoot, "Resources", name), Path.Combine(resources, name), true);
            }

            return root;
        }

        private static Task<string> RunAsync(string root, ProvenanceManifest? provenance = null) =>
            SampleTransformPipeline.RunSampleTransformAsync(root, UpstreamSample, GeneratedSample,
                new SampleTransformOptions { EnsureLatestGtkSharp = false, Provenance = provenance });

        /// <summary>Relative path → content for every generated file, so two runs can be compared.</summary>
        private static SortedDictionary<string, byte[]> Snapshot(string dir)
        {
            var snapshot = new SortedDictionary<string, byte[]>(StringComparer.Ordinal);
            foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            {
                snapshot[Path.GetRelativePath(dir, file).Replace('\\', '/')] = File.ReadAllBytes(file);
            }

            return snapshot;
        }

        // --- tests -----------------------------------------------------------

        [Fact]
        public async Task RunSampleTransform_IsRepoRootIsolated()
        {
            var root = BuildFixtureRoot();
            var rootBefore = RepoPaths.Root;
            try
            {
                await RunAsync(root);

                // It produced the generated sample inside the given root...
                var generated = Path.Combine(root, GeneratedSample);
                Assert.True(File.Exists(Path.Combine(generated, $"{GeneratedSample}.csproj")));
                Assert.True(File.Exists(Path.Combine(generated, "Form1.cs")));

                // ...and left the caller's repo root untouched, so the live tree is never at risk.
                Assert.Equal(rootBefore, RepoPaths.Root);
                Assert.False(Directory.Exists(Path.Combine(RealRepoRoot, GeneratedSample)));

                // The upstream sample is read-only input: it must come out exactly as it went in.
                var upstream = Path.Combine(root, "gtksystem-windows-forms", "Samples", UpstreamSample);
                Assert.Equal("upstream notes", File.ReadAllText(Path.Combine(upstream, "工程说明.txt")));
                Assert.Contains("//打印", File.ReadAllText(Path.Combine(upstream, "Form1.cs")));
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public async Task RunSampleTransform_IsDeterministic()
        {
            var rootA = BuildFixtureRoot();
            var rootB = BuildFixtureRoot();
            try
            {
                await RunAsync(rootA);
                await RunAsync(rootB);

                var a = Snapshot(Path.Combine(rootA, GeneratedSample));
                var b = Snapshot(Path.Combine(rootB, GeneratedSample));

                Assert.Equal(a.Keys, b.Keys);
                foreach (var rel in a.Keys)
                {
                    Assert.True(a[rel].AsSpan().SequenceEqual(b[rel]), $"{rel} differs between two runs");
                }
            }
            finally
            {
                SafeDelete(rootA); SafeDelete(rootB);
            }
        }

        [Fact]
        public async Task RunSampleTransform_RecordsProvenanceForRenamedOverlaidAndDeletedFiles()
        {
            var root = BuildFixtureRoot();
            try
            {
                var provenance = new ProvenanceManifest();
                await RunAsync(root, provenance);
                var map = provenance.Entries;

                var upstream = $"gtksystem-windows-forms/Samples/{UpstreamSample}";

                // Renamed: the generated csproj still names the upstream original.
                Assert.Equal($"{upstream}/{UpstreamSample}.csproj", map[$"{GeneratedSample}/{GeneratedSample}.csproj"]);
                // Straight copy + translation in place.
                Assert.Equal($"{upstream}/Form1.cs", map[$"{GeneratedSample}/Form1.cs"]);
                // Renamed asset.
                Assert.Equal($"{upstream}/Resources/010.jpg", map[$"{GeneratedSample}/Resources/img11.jpg"]);
                // Overlaid from the fork's own Resources, NOT from upstream.
                Assert.Equal("Resources/GtkWinFormsAppResources.zh-Hans.xml",
                    map[$"{GeneratedSample}/Properties/GtkWinFormsAppResources.zh-Hans.resx"]);
                Assert.Equal("Resources/TestData1.json", map[$"{GeneratedSample}/TestData1.json"]);
                // Deleted by the transform - no entry may point at a file that is not there.
                Assert.DoesNotContain($"{GeneratedSample}/工程说明.txt", map.Keys);

                // Every recorded path is repo-relative on both sides (the staging root is transient).
                foreach (var kv in map)
                {
                    Assert.False(Path.IsPathRooted(kv.Key));
                    Assert.False(Path.IsPathRooted(kv.Value));
                }
            }
            finally
            {
                SafeDelete(root);
            }
        }

        [Fact]
        public async Task RunSampleTransform_TranslatesTheMigratedHardcodedFixups()
        {
            var root = BuildFixtureRoot();
            try
            {
                await RunAsync(root);

                var form1 = File.ReadAllText(Path.Combine(root, GeneratedSample, "Form1.cs"));

                // These three used to be hardcoded string replacements on Form1.cs/Form3.cs; they now come
                // from Resources/samplesTranslations.xml and must render exactly as before.
                Assert.Contains("//1. Dataset list data source", form1);
                Assert.Contains("//2. Datatable data source", form1);
                Assert.Contains("// Print", form1);
                Assert.DoesNotContain("数据源", form1);
                Assert.DoesNotContain("打印", form1);
            }
            finally
            {
                SafeDelete(root);
            }
        }

        /// <summary>
        /// The GtkWinFormsApp sample must stay drivable by GtkWinFormsApp.UITests after every regeneration:
        /// its Designer fields promoted to <c>internal</c> and an <c>InternalsVisibleTo</c> grant for the
        /// test assembly. Only the <c>GtkWinFormsApp</c> target gets these (the other sample is untouched).
        /// </summary>
        [Fact]
        public async Task RunSampleTransform_ForGtkWinFormsApp_InternalizesDesignerFieldsAndGrantsInternalsVisibleTo()
        {
            var root = BuildFixtureRoot();
            try
            {
                // A generated Designer control field the UI tests reach without reflection.
                var upstreamDir = Path.Combine(root, "gtksystem-windows-forms", "Samples", UpstreamSample);
                File.WriteAllText(Path.Combine(upstreamDir, "Form1.Designer.cs"),
                    """
                    namespace FixtureSampleApp
                    {
                        partial class Form1
                        {
                            private System.Windows.Forms.Button button1;
                        }
                    }
                    """);

                // Run fs with the UI-testable target name so the internalize + IVT steps fire.
                await SampleTransformPipeline.RunSampleTransformAsync(root, UpstreamSample, "GtkWinFormsApp",
                    new SampleTransformOptions { EnsureLatestGtkSharp = false });

                var designer = File.ReadAllText(Path.Combine(root, "GtkWinFormsApp", "Form1.Designer.cs"));
                Assert.Contains("internal System.Windows.Forms.Button button1", designer);
                Assert.DoesNotContain("private System.Windows.Forms.Button", designer);

                var csproj = File.ReadAllText(Path.Combine(root, "GtkWinFormsApp", "GtkWinFormsApp.csproj"));
                Assert.Contains("InternalsVisibleTo", csproj);
                Assert.Contains("GtkWinFormsApp.UITests, PublicKey=", csproj);
            }
            finally
            {
                SafeDelete(root);
            }
        }
    }
}
