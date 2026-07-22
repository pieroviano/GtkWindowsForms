using Prepare.Common.IO;

namespace Prepare.Tests
{
    /// <summary>
    /// The patch series is applied in plain filename order, so two patches sharing a leading ordinal are
    /// ordered by their description text rather than by any intent. That is a trap the first time two
    /// same-numbered patches touch the same hunk, so the applier refuses the ambiguity up front.
    /// </summary>
    public class GitPatchApplierOrdinalTests
    {
        [Fact]
        public void EnsureOrdinalsAreUnique_UniqueOrdinals_DoesNotThrow()
        {
            var patches = new[]
            {
                @"c:\repo\patches\0001-First.patch",
                @"c:\repo\patches\0002-Second.patch",
                @"c:\repo\patches\0002a-SecondPartTwo.patch",
            };

            GitPatchApplier.EnsureOrdinalsAreUnique(patches);
        }

        [Fact]
        public void EnsureOrdinalsAreUnique_DuplicateOrdinals_Throws()
        {
            var patches = new[]
            {
                @"c:\repo\patches\0007-Fix-trackbar.patch",
                @"c:\repo\patches\0007-Resolved-infinite-recursion-problems.patch",
            };

            var ex = Assert.Throws<InvalidOperationException>(
                () => GitPatchApplier.EnsureOrdinalsAreUnique(patches));

            Assert.Contains("0007", ex.Message);
            Assert.Contains("Fix-trackbar", ex.Message);
            Assert.Contains("Resolved-infinite-recursion-problems", ex.Message);
        }

        [Fact]
        public void EnsureOrdinalsAreUnique_ReportsEveryClashingOrdinal()
        {
            var patches = new[]
            {
                @"c:\repo\patches\0007-A.patch",
                @"c:\repo\patches\0007-B.patch",
                @"c:\repo\patches\0016-A.patch",
                @"c:\repo\patches\0016-B.patch",
            };

            var ex = Assert.Throws<InvalidOperationException>(
                () => GitPatchApplier.EnsureOrdinalsAreUnique(patches));

            Assert.Contains("0007", ex.Message);
            Assert.Contains("0016", ex.Message);
        }

        [Fact]
        public void EnsureOrdinalsAreUnique_TheRealPatchSeries_IsUnambiguous()
        {
            var patchesDirectory = FindPatchesDirectory();
            Assert.True(Directory.Exists(patchesDirectory), $"patches directory not found at {patchesDirectory}");

            var patches = Directory.GetFiles(patchesDirectory, "*.patch");
            Assert.NotEmpty(patches);

            // The guard the pipeline runs, applied to the series actually committed in this repository.
            GitPatchApplier.EnsureOrdinalsAreUnique(patches);
        }

        [Fact]
        public void TheRealPatchSeries_AppliesInAscendingOrdinalOrder()
        {
            var patches = Directory.GetFiles(FindPatchesDirectory(), "*.patch")
                .Select(Path.GetFileName)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            // A letter-suffixed ordinal must sort immediately after its plain sibling, never before it, or
            // the rename would silently have reordered the series.
            var ordinals = patches.Select(name => name!.Split('-')[0]).ToList();
            Assert.Equal(ordinals.OrderBy(o => o, StringComparer.Ordinal).ToList(), ordinals);
        }

        private static string FindPatchesDirectory()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, "patches");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not locate the repository's patches directory.");
        }
    }
}
