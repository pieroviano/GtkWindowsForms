using System.Text.RegularExpressions;

namespace Prepare.Tests
{
    /// <summary>
    /// Structural checks over the committed <c>patches/</c> series.
    /// <para>
    /// A patch is the only durable form a change to the generated trees can take, and a damaged one is
    /// almost invisible: <c>git apply</c> is atomic per file, so a single malformed hunk rejects the
    /// <em>whole</em> patch and every unrelated fix in it, and the only signal is a red line at the end of a
    /// pipeline run that CI deliberately never performs.
    /// </para>
    /// <para>
    /// That is not hypothetical. <c>0006-Cleaned-code.patch</c> reached the repository as a
    /// <em>rendered dump</em> of a patch - two junk lines on top, CRLF endings, 7 truncated hunk headers,
    /// and only 12 of the 130 file diffs its own summary listed. It had been silently contributing nothing
    /// for as long as it had been there. It now lives in <c>patches/corrupt/</c>; these tests are what stop
    /// the next one going unnoticed.
    /// </para>
    /// The three properties asserted here are cheap and need no git: a patch is a mailbox file, its hunk
    /// headers are complete, and the number of file diffs matches the number its summary promises.
    /// </summary>
    public class PatchSeriesIntegrityTests
    {
        /// <summary>Canonical unified-diff hunk header: <c>@@ -a,b +c,d @@</c> (counts optional).</summary>
        private static readonly Regex HunkHeader =
            new(@"^@@ -\d+(,\d+)? \+\d+(,\d+)? @@", RegexOptions.Compiled);

        /// <summary>A diffstat entry: <c>path | 12 ++--</c>.</summary>
        private static readonly Regex DiffstatEntry =
            new(@"^ \S.*\|\s+\d+\s*[+-]*$", RegexOptions.Compiled);

        public static TheoryData<string> Patches()
        {
            var data = new TheoryData<string>();
            foreach (var file in Directory.GetFiles(PatchesDirectory, "*.patch").OrderBy(f => f, StringComparer.Ordinal))
            {
                data.Add(Path.GetFileName(file));
            }

            return data;
        }

        /// <summary>
        /// The series is applied by <c>Directory.GetFiles(patchesPath, "*.patch")</c>, so anything matching
        /// that glob is applied. If this ever returns nothing the rest of the suite would vacuously pass.
        /// </summary>
        [Fact]
        public void TheSeriesIsNotEmpty()
        {
            Assert.NotEmpty(Directory.GetFiles(PatchesDirectory, "*.patch"));
        }

        [Theory]
        [MemberData(nameof(Patches))]
        public void EveryPatch_IsAMailboxPatchAndNotARenderedDump(string name)
        {
            var lines = LinesOf(name);

            Assert.True(lines.Length > 0, $"{name} is empty.");
            Assert.True(
                lines[0].StartsWith("From ", StringComparison.Ordinal),
                $"{name} does not start with a mailbox 'From <sha>' line - it starts with '{Truncate(lines[0])}'. " +
                "A patch that begins with anything else (a 'File:' header, a markdown fence) is a rendering " +
                "of a patch rather than a patch, and git will not apply it.");

            Assert.DoesNotContain(lines, l => l.StartsWith("```", StringComparison.Ordinal));
        }

        [Theory]
        [MemberData(nameof(Patches))]
        public void EveryHunkHeader_IsComplete(string name)
        {
            var malformed = LinesOf(name)
                .Select((line, index) => (line, number: index + 1))
                .Where(x => x.line.StartsWith("@@", StringComparison.Ordinal) && !HunkHeader.IsMatch(x.line))
                .Select(x => $"line {x.number}: '{Truncate(x.line)}'")
                .ToList();

            Assert.True(malformed.Count == 0,
                $"{name} has {malformed.Count} truncated hunk header(s): {string.Join("; ", malformed)}. " +
                "git rejects the entire patch on the first one.");
        }

        /// <summary>
        /// <c>git format-patch</c> writes a diffstat listing every file it then diffs. When the two disagree
        /// the file has lost content in transit - which is exactly how the corrupt patch presented, with 130
        /// files summarised and 12 actually present.
        /// <para>
        /// Hand-authored patches in this series carry a prose commit body and no diffstat at all; those are
        /// skipped rather than failed, because "no summary" is not the same as "a summary that disagrees".
        /// A dump that lost its diffstat too is still caught by the two checks above.
        /// </para>
        /// </summary>
        [Theory]
        [MemberData(nameof(Patches))]
        public void TheDiffstat_AccountsForEveryFileDiff(string name)
        {
            var lines = LinesOf(name);
            var summarised = lines.Count(l => DiffstatEntry.IsMatch(l));

            if (summarised == 0)
            {
                return;
            }

            var diffed = lines.Count(l => l.StartsWith("diff --git ", StringComparison.Ordinal));

            Assert.True(summarised == diffed,
                $"{name} summarises {summarised} file(s) in its diffstat but contains {diffed} 'diff --git' " +
                "block(s). A patch whose body is shorter than its own summary has been truncated.");
        }

        /// <summary>
        /// Whatever its provenance, a patch has to contain at least one file diff - otherwise it is a commit
        /// message with nothing attached and contributes nothing to the generated tree.
        /// </summary>
        [Theory]
        [MemberData(nameof(Patches))]
        public void EveryPatch_ContainsAtLeastOneFileDiff(string name)
        {
            Assert.Contains(LinesOf(name), l => l.StartsWith("diff --git ", StringComparison.Ordinal));
        }

        private static string PatchesDirectory => Path.Combine(RepositoryRoot, "patches");

        private static string[] LinesOf(string name) =>
            File.ReadAllLines(Path.Combine(PatchesDirectory, name));

        private static string Truncate(string value) =>
            value.Length <= 60 ? value : value.Substring(0, 60) + "...";

        private static string RepositoryRoot
        {
            get
            {
                var directory = new DirectoryInfo(AppContext.BaseDirectory);
                while (directory != null)
                {
                    if (File.Exists(Path.Combine(directory.FullName, "Directory.Nuget.Props")))
                    {
                        return directory.FullName;
                    }

                    directory = directory.Parent;
                }

                throw new DirectoryNotFoundException("Could not locate the repository root.");
            }
        }
    }
}
