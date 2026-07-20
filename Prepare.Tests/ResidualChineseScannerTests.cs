using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class ResidualChineseScannerTests
    {
        private static void Write(string root, string rel, string content)
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content);
        }

        [Fact]
        public void Scan_FindsChinese_IgnoresEnglish_And_BinObj()
        {
            var dir = Path.Combine(Path.GetTempPath(), "rcs-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                Write(dir, "A.cs", "// 你好 world\nvar x = 1;\n");   // 2 Chinese chars
                Write(dir, "B.cs", "// pure english only\n");         // none
                Write(dir, "obj/Gen.cs", "// 垃圾 generated\n");       // ignored (obj)
                Write(dir, "bin/Debug/C.cs", "// 忽略 me\n");          // ignored (bin)

                var results = ResidualChineseScanner.Scan(dir);

                Assert.Single(results);
                Assert.EndsWith("A.cs", results[0].Key);
                Assert.Equal(2, results[0].Value); // 你 好
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Fact]
        public void Scan_NoChinese_ReturnsEmpty()
        {
            var dir = Path.Combine(Path.GetTempPath(), "rcs-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                Write(dir, "A.cs", "all english here");
                Assert.Empty(ResidualChineseScanner.Scan(dir));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
