using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class ConflictReporterTests
    {
        [Fact]
        public void Describe_ResolvesAllPaths_AndProvenanceSource()
        {
            var provenance = new Dictionary<string, string>
            {
                ["Net4x.Gtk.Drawing/System/Drawing/Pen.cs"] = @"C:\src\gtksystem\Source\GTKSystem.Windows.Forms\GTKControls\Drawing\Pen.cs"
            };

            var conflicts = ConflictReporter.Describe(
                repoDir: @"C:\repo",
                baseTreeDir: @"C:\build\base",
                theirsTreeDir: @"C:\build\theirs",
                provenance: provenance,
                conflictRelativePaths: new[] { "Net4x.Gtk.Drawing/System/Drawing/Pen.cs" });

            var c = Assert.Single(conflicts);
            Assert.Equal("Net4x.Gtk.Drawing/System/Drawing/Pen.cs", c.RelativePath);
            Assert.EndsWith("Pen.cs", c.Net4xFullPath);
            Assert.Contains("repo", c.Net4xFullPath);
            Assert.Contains("base", c.BaseRefFullPath);
            Assert.Contains("theirs", c.TheirsRefFullPath);
            Assert.Equal(provenance["Net4x.Gtk.Drawing/System/Drawing/Pen.cs"], c.SourceFullPath);
        }

        [Fact]
        public void Describe_UnresolvedProvenance_LeavesSourceNull_AndNoticeSaysSo()
        {
            var conflicts = ConflictReporter.Describe(
                @"C:\repo", @"C:\build\base", @"C:\build\theirs",
                provenance: null,
                conflictRelativePaths: new[] { "Net4x.Gtk.Windows.Forms/Form.cs" });

            Assert.Null(conflicts[0].SourceFullPath);
            var notice = ConflictReporter.FormatNotice(conflicts);
            Assert.Contains("1 file(s)", notice);
            Assert.Contains("Form.cs", notice);
            Assert.Contains("unresolved", notice);
        }

        [Fact]
        public void FormatOne_LeadsWithTheDiffedTransformedPair_AndMarksRawSourceContextOnly()
        {
            var conflicts = ConflictReporter.Describe(
                @"C:\repo", @"C:\build\base", @"C:\build\theirs",
                provenance: new Dictionary<string, string>
                {
                    ["Net4x.Gtk.Drawing/Pen.cs"] = @"C:\src\gtksystem\Source\Pen.cs"
                },
                conflictRelativePaths: new[] { "Net4x.Gtk.Drawing/Pen.cs" });

            var text = ConflictReporter.FormatOne(conflicts[0]);

            // The delta was computed between the two transformed trees, so they must be the primary
            // reference: both listed, explicitly labelled as diffed, and both ahead of the raw source.
            Assert.Contains("DIFFED base", text);
            Assert.Contains("DIFFED theirs", text);
            Assert.True(text.IndexOf(@"C:\build\base", StringComparison.Ordinal) <
                        text.IndexOf(@"C:\src\gtksystem", StringComparison.Ordinal));
            Assert.True(text.IndexOf(@"C:\build\theirs", StringComparison.Ordinal) <
                        text.IndexOf(@"C:\src\gtksystem", StringComparison.Ordinal));

            // The raw upstream file is unformatted and pre-transform: usable for intent, not for hunks.
            Assert.Contains("context only", text);
        }
    }
}
