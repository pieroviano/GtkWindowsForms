using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class ProvenanceManifestTests
    {
        [Fact]
        public void SaveThenLoad_RoundTrips_NormalizingSeparators()
        {
            var dir = Path.Combine(Path.GetTempPath(), "prov-" + Guid.NewGuid().ToString("N"));
            var path = Path.Combine(dir, "provenance.tsv");
            try
            {
                var m = new ProvenanceManifest();
                m.Add(@"Gtk.Drawing\System\Drawing\Pen.cs", @"C:\src\GTKControls\Drawing\Pen.cs");
                m.Save(path);

                var loaded = ProvenanceManifest.Load(path);

                Assert.Single(loaded);
                Assert.Equal(@"C:\src\GTKControls\Drawing\Pen.cs",
                    loaded["Gtk.Drawing/System/Drawing/Pen.cs"]); // key normalized to '/'
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void Load_MissingOrNull_ReturnsEmpty()
        {
            Assert.Empty(ProvenanceManifest.Load(null));
            Assert.Empty(ProvenanceManifest.Load(Path.Combine(Path.GetTempPath(), "nope-" + Guid.NewGuid().ToString("N") + ".tsv")));
        }
    }
}
