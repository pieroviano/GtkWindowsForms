using Prepare.Common.IO;

namespace Prepare.Tests
{
    /// <summary>
    /// The generate-from-submodule design has one silent failure mode: a fix hand-edited into
    /// <c>Gtk.Windows.Forms/</c> or <c>Gtk.Drawing/</c> is overwritten by the next pipeline
    /// run and, not being in <c>patches/</c>, never comes back. The guard names those edits before the
    /// transform destroys them.
    /// </summary>
    public class GeneratedTreeGuardTests
    {
        [Fact]
        public void FindEndangeredEdits_CleanTree_FindsNothing()
        {
            Assert.Empty(GeneratedTreeGuard.FindEndangeredEdits(string.Empty));
            Assert.Empty(GeneratedTreeGuard.FindEndangeredEdits(null));
        }

        [Fact]
        public void FindEndangeredEdits_EditsOutsideGeneratedTrees_AreNotReported()
        {
            var porcelain = string.Join("\n", new[]
            {
                " M Prepare.Common/IO/GitPatchApplier.cs",
                "?? Gtk.Drawing.Tests/System/Drawing/NewTests.cs",
                " M Solution-Review.md",
            });

            // Tests, tooling and documents are ordinary tracked source - the transform never touches them.
            Assert.Empty(GeneratedTreeGuard.FindEndangeredEdits(porcelain));
        }

        [Fact]
        public void FindEndangeredEdits_EditsInsideGeneratedTrees_AreReported()
        {
            var porcelain = string.Join("\n", new[]
            {
                " M Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs",
                " M Gtk.Drawing/System/Drawing/Graphics.cs",
                " M Prepare.Common/IO/GitPatchApplier.cs",
            });

            var endangered = GeneratedTreeGuard.FindEndangeredEdits(porcelain);

            Assert.Equal(2, endangered.Count);
            Assert.Contains("Gtk.Windows.Forms/System/Windows/Forms/Control/Control.cs", endangered);
            Assert.Contains("Gtk.Drawing/System/Drawing/Graphics.cs", endangered);
        }

        [Fact]
        public void FindEndangeredEdits_ProjectFiles_AreNotReported()
        {
            // csproj files are maintained here rather than produced by the transform, which is exactly why
            // the patch recipe in Patches.txt excludes them.
            var porcelain = " M Gtk.Drawing/Gtk.Drawing.csproj";

            Assert.Empty(GeneratedTreeGuard.FindEndangeredEdits(porcelain));
        }

        [Fact]
        public void FindEndangeredEdits_UntrackedAndStagedEdits_AreBothReported()
        {
            var porcelain = string.Join("\n", new[]
            {
                "?? Gtk.Drawing/System/Drawing/Brand.cs",
                "M  Gtk.Windows.Forms/System/Windows/Forms/Form.cs",
            });

            var endangered = GeneratedTreeGuard.FindEndangeredEdits(porcelain);

            Assert.Equal(2, endangered.Count);
        }

        [Fact]
        public void FindEndangeredEdits_Rename_ReportsTheDestination()
        {
            var porcelain = "R  Gtk.Drawing/System/Drawing/Old.cs -> Gtk.Drawing/System/Drawing/New.cs";

            var endangered = GeneratedTreeGuard.FindEndangeredEdits(porcelain);

            Assert.Single(endangered);
            Assert.Equal("Gtk.Drawing/System/Drawing/New.cs", endangered[0]);
        }

        [Fact]
        public void FindEndangeredEdits_WindowsSeparators_AreMatched()
        {
            var porcelain = @" M Gtk.Drawing\System\Drawing\Graphics.cs";

            Assert.Single(GeneratedTreeGuard.FindEndangeredEdits(porcelain));
        }

        [Fact]
        public void FindEndangeredEdits_HonoursTheRootsItIsGiven()
        {
            var porcelain = " M SomeOther.Generated/File.cs";

            Assert.Empty(GeneratedTreeGuard.FindEndangeredEdits(porcelain));
            Assert.Single(GeneratedTreeGuard.FindEndangeredEdits(porcelain, new[] { "SomeOther.Generated/" }));
        }
    }
}
