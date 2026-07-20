using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class TreeDiffTests
    {
        private static string NewDir()
        {
            var dir = Path.Combine(Path.GetTempPath(), "td-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void Write(string root, string rel, string content)
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content);
        }

        [Fact]
        public void Compare_IdenticalTrees_IsIdentical()
        {
            var a = NewDir();
            var b = NewDir();
            try
            {
                Write(a, "x/A.cs", "same");
                Write(b, "x/A.cs", "same");
                Assert.True(TreeDiff.Compare(a, b).Identical);
            }
            finally { Directory.Delete(a, true); Directory.Delete(b, true); }
        }

        [Fact]
        public void Compare_DetectsContentDiff_AndExtraFiles()
        {
            var a = NewDir();
            var b = NewDir();
            try
            {
                Write(a, "x/A.cs", "one");
                Write(b, "x/A.cs", "TWO");       // content differs
                Write(a, "x/OnlyLeft.cs", "l");  // only in left
                Write(b, "x/OnlyRight.cs", "r"); // only in right

                var result = TreeDiff.Compare(a, b);

                Assert.False(result.Identical);
                Assert.Contains("x/A.cs", result.ContentDiffers);
                Assert.Contains("x/OnlyLeft.cs", result.OnlyInLeft);
                Assert.Contains("x/OnlyRight.cs", result.OnlyInRight);
            }
            finally { Directory.Delete(a, true); Directory.Delete(b, true); }
        }
    }
}
