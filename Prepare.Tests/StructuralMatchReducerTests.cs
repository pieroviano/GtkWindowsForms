using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class StructuralMatchReducerTests
    {
        [Fact]
        public void Reduce_RealReportedCase_KeepsOnlyGenuineResidual()
        {
            // The exact drift AlignProjects reported (obj/* already excluded by StructuralMatch).
            var match = new StructuralMatchResult(
                onlyInBase: new[]
                {
                    "Net4x.Gtk.Windows.Forms.props",                                  // rename (Net4x. prefix)
                    "Net4x.Gtk.Windows.Forms.targets",                               // rename
                    "System/Windows/Forms/Application/ApplicationConfiguration.cs",   // patch DELETE (genuine)
                    "System/Windows/Forms/Application/InitAppliction.cs",             // patch DELETE (genuine)
                    "System/Windows/Forms/Collection/Control.cs",                     // split source
                    "System/Windows/Forms/ImageList/ColorDepth.cs"                    // moved
                },
                onlyInOurs: new[]
                {
                    "Gtk.Windows.Forms.props",                                        // rename target
                    "Gtk.Windows.Forms.targets",                                      // rename target
                    "Net4x.Gtk.Windows.Forms.csproj.DotSettings",                     // committed setting (genuine)
                    "System/Windows/Forms/Collection/Control.ArrangedElementWidget.cs", // split part
                    "System/Windows/Forms/Collection/Control.ControlCollection.cs",     // split part
                    "System/Windows/Forms/Enum/ColorDepth.cs"                         // moved target
                });

            var reduced = StructuralMatchReducer.Reduce(match);

            // renames/moves/splits are recognized and dropped; only genuine residual remains.
            Assert.Equal(
                new[]
                {
                    "System/Windows/Forms/Application/ApplicationConfiguration.cs",
                    "System/Windows/Forms/Application/InitAppliction.cs"
                },
                reduced.BaseOnly);
            Assert.Equal(new[] { "Net4x.Gtk.Windows.Forms.csproj.DotSettings" }, reduced.OursOnly);
        }

        [Fact]
        public void Reduce_NoDifferences_HasNoResidual()
        {
            var match = new StructuralMatchResult(System.Array.Empty<string>(), System.Array.Empty<string>());
            Assert.False(StructuralMatchReducer.Reduce(match).HasResidual);
        }
    }
}
