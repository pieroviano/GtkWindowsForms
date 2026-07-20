using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class TreePathResolverTests : IDisposable
    {
        private readonly string _tree = Path.Combine(Path.GetTempPath(), "treeresolve-" + Guid.NewGuid().ToString("N"));

        public TreePathResolverTests() => TreePathResolver.ClearCache();

        private void Write(string relativePath)
        {
            var full = Path.Combine(_tree, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, "// x");
        }

        [Fact]
        public void Resolve_ExactPath_Wins()
        {
            Write("Net4x.Gtk.Drawing/System/Drawing/Bitmap.cs");

            var resolved = TreePathResolver.Resolve(_tree, "Net4x.Gtk.Drawing/System/Drawing/Bitmap.cs");

            Assert.Equal(Path.Combine(_tree, "Net4x.Gtk.Drawing", "System", "Drawing", "Bitmap.cs"), resolved);
        }

        [Fact]
        public void Resolve_FollowsRename_WhenForkLayoutDiffersFromTree()
        {
            // The real case: the transform emits Drawing2D/Bitmap.cs, a patch moved the fork's copy up a
            // level, and git reports the conflict under the FORK path.
            Write("Net4x.Gtk.Drawing/System/Drawing/Drawing2D/Bitmap.cs");

            var resolved = TreePathResolver.Resolve(_tree, "Net4x.Gtk.Drawing/System/Drawing/Bitmap.cs");

            Assert.Equal(Path.Combine(_tree, "Net4x.Gtk.Drawing", "System", "Drawing", "Drawing2D", "Bitmap.cs"), resolved);
        }

        [Fact]
        public void Resolve_PrefersSameProject_WhenTheNameExistsInBoth()
        {
            Write("Net4x.Gtk.Drawing/A/ResourceManager.cs");
            Write("Net4x.Gtk.Windows.Forms/B/ResourceManager.cs");

            var resolved = TreePathResolver.Resolve(_tree, "Net4x.Gtk.Windows.Forms/Somewhere/ResourceManager.cs");

            Assert.Equal(Path.Combine(_tree, "Net4x.Gtk.Windows.Forms", "B", "ResourceManager.cs"), resolved);
        }

        [Fact]
        public void Resolve_ReturnsNull_WhenAmbiguousWithinTheSameProject()
        {
            // Two candidates: guessing could merge the delta into the wrong file, so refuse.
            Write("Net4x.Gtk.Drawing/A/Pen.cs");
            Write("Net4x.Gtk.Drawing/B/Pen.cs");

            Assert.Null(TreePathResolver.Resolve(_tree, "Net4x.Gtk.Drawing/System/Drawing/Pen.cs"));
        }

        [Fact]
        public void Resolve_ReturnsNull_WhenAbsentOrTreeMissing()
        {
            Write("Net4x.Gtk.Drawing/A/Pen.cs");

            Assert.Null(TreePathResolver.Resolve(_tree, "Net4x.Gtk.Drawing/System/Drawing/Brush.cs"));
            Assert.Null(TreePathResolver.Resolve(Path.Combine(_tree, "nope"), "Net4x.Gtk.Drawing/A/Pen.cs"));
        }

        [Fact]
        public void Resolve_IgnoresCapturedManifests()
        {
            Write("_manifests/Net4x.Gtk.Drawing.Files.xml");
            Write("Net4x.Gtk.Drawing/A/Net4x.Gtk.Drawing.Files.xml");

            var resolved = TreePathResolver.Resolve(_tree, "Net4x.Gtk.Drawing/X/Net4x.Gtk.Drawing.Files.xml");

            // Without the _manifests exclusion this would be ambiguous and resolve to null.
            Assert.Equal(Path.Combine(_tree, "Net4x.Gtk.Drawing", "A", "Net4x.Gtk.Drawing.Files.xml"), resolved);
        }

        public void Dispose()
        {
            TreePathResolver.ClearCache();
            try { Directory.Delete(_tree, true); } catch { /* best-effort */ }
        }
    }
}
