using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class FilesXmlProvenanceTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "filesxmlprov-" + Guid.NewGuid().ToString("N"));

        private string Manifests => Path.Combine(_root, "manifests");
        private string Upstream => Path.Combine(_root, "upstream");

        private void WriteManifest(string fileName, string xml)
        {
            Directory.CreateDirectory(Manifests);
            File.WriteAllText(Path.Combine(Manifests, fileName), xml);
        }

        private void WriteUpstream(string relativePath)
        {
            var full = Path.Combine(Upstream, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, "// upstream");
        }

        [Fact]
        public void Load_KeysByRepoRelativeGeneratedPath_AndResolvesSourceAgainstUpstream()
        {
            WriteUpstream(@"GtkControls\ListView.cs");
            WriteManifest("Gtk.Windows.Forms.Files.xml",
                """
                <Files>
                  <Item Source="GtkControls\ListView.cs" Destination="System\Windows\Forms\ListView.cs" />
                </Files>
                """);

            var map = FilesXmlProvenance.Load(Manifests, Upstream);

            // The key must match the repo-relative path git reports for a conflict.
            var source = Assert.Contains("Gtk.Windows.Forms/System/Windows/Forms/ListView.cs", map);
            Assert.Equal(Path.Combine(Upstream, "GtkControls", "ListView.cs"), source);
        }

        [Fact]
        public void Load_DropsEntriesWhoseSourceDoesNotExist()
        {
            // The Drawing manifest is largely Source == Destination identity rows that never existed
            // upstream; printing those paths would send the operator to a non-existent file.
            WriteManifest("Gtk.Drawing.LatestFiles.xml",
                """
                <Files>
                  <Item Source="System\Drawing\Pen.cs" Destination="System\Drawing\Pen.cs" />
                </Files>
                """);

            Assert.Empty(FilesXmlProvenance.Load(Manifests, Upstream));
        }

        [Fact]
        public void Load_LatestFilesWinsOverEarlierManifests()
        {
            WriteUpstream(@"GtkControls\Old.cs");
            WriteUpstream(@"GtkControls\New.cs");
            WriteManifest("Gtk.Windows.Forms.Files.xml",
                """
                <Files>
                  <Item Source="GtkControls\Old.cs" Destination="System\Windows\Forms\Thing.cs" />
                </Files>
                """);
            WriteManifest("Gtk.Windows.Forms.LatestFiles.xml",
                """
                <Files>
                  <Item Source="GtkControls\New.cs" Destination="System\Windows\Forms\Thing.cs" />
                </Files>
                """);

            var map = FilesXmlProvenance.Load(Manifests, Upstream);

            Assert.Equal(Path.Combine(Upstream, "GtkControls", "New.cs"),
                map["Gtk.Windows.Forms/System/Windows/Forms/Thing.cs"]);
        }

        [Fact]
        public void Load_MissingDirectoryOrMalformedManifest_YieldsEmptyMapInsteadOfThrowing()
        {
            Assert.Empty(FilesXmlProvenance.Load(Path.Combine(_root, "nope"), Upstream));

            WriteManifest("Gtk.Windows.Forms.Files.xml", "<Files><Item Source=");
            Assert.Empty(FilesXmlProvenance.Load(Manifests, Upstream));
        }

        public void Dispose()
        {
            try { Directory.Delete(_root, true); } catch { /* best-effort */ }
        }
    }
}
