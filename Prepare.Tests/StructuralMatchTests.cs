using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class StructuralMatchTests
    {
        private static string NewDir()
        {
            var dir = Path.Combine(Path.GetTempPath(), "struct-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void Write(string root, string rel, string content = "x")
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content);
        }

        [Fact]
        public void Compare_SamePaths_Matches_EvenWhenContentDiffers()
        {
            var a = NewDir();
            var b = NewDir();
            try
            {
                Write(a, "System/Drawing/Pen.cs", "old");
                Write(b, "System/Drawing/Pen.cs", "new content"); // content differs, path same
                Write(a, "Form.cs");
                Write(b, "Form.cs");

                var result = StructuralMatch.Compare(a, b);

                Assert.True(result.Matches);
                Assert.Empty(result.OnlyInBase);
                Assert.Empty(result.OnlyInOurs);
            }
            finally { Directory.Delete(a, true); Directory.Delete(b, true); }
        }

        [Fact]
        public void Compare_ExtraPathInBase_ReportedAndNotMatching()
        {
            var baseDir = NewDir();
            var oursDir = NewDir();
            try
            {
                Write(baseDir, "Form.cs");
                Write(baseDir, "System/Drawing/NewlySplit.cs"); // exists only in base (layout drift)
                Write(oursDir, "Form.cs");

                var result = StructuralMatch.Compare(baseDir, oursDir);

                Assert.False(result.Matches);
                Assert.Contains("System/Drawing/NewlySplit.cs", result.OnlyInBase);
                Assert.Empty(result.OnlyInOurs);
            }
            finally { Directory.Delete(baseDir, true); Directory.Delete(oursDir, true); }
        }

        [Fact]
        public void Compare_ExtraPathInOurs_Reported()
        {
            var baseDir = NewDir();
            var oursDir = NewDir();
            try
            {
                Write(baseDir, "Form.cs");
                Write(oursDir, "Form.cs");
                Write(oursDir, "PatchAdded.cs"); // exists only in ours

                var result = StructuralMatch.Compare(baseDir, oursDir);

                Assert.False(result.Matches);
                Assert.Contains("PatchAdded.cs", result.OnlyInOurs);
            }
            finally { Directory.Delete(baseDir, true); Directory.Delete(oursDir, true); }
        }
    }
}
