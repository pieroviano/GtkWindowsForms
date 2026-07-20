using System.Diagnostics;
using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class DeltaMergeTests
    {
        // --- helpers ---------------------------------------------------------

        private static string NewTemp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "dm-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        // git marks pack/object files read-only; clear attributes so temp cleanup can't throw
        // (and never let cleanup mask a real assertion failure).
        private static void SafeDelete(string dir)
        {
            if (!Directory.Exists(dir)) return;
            try
            {
                foreach (var f in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(f, FileAttributes.Normal);
                }
                Directory.Delete(dir, true);
            }
            catch
            {
                // best-effort temp cleanup
            }
        }

        private static string Git(string dir, string args)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = args,
                    WorkingDirectory = dir,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            p.StartInfo.Environment["GIT_EDITOR"] = "true";
            p.Start();
            var outp = p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit();
            return outp;
        }

        private static void Write(string root, string rel, string content)
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content.Replace("\r\n", "\n"));
        }

        private static string Read(string root, string rel) =>
            File.ReadAllText(Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar))).Replace("\r\n", "\n");

        /// <summary>Creates a git repo with the given committed OURS files.</summary>
        private static string InitRepo(Action<string> writeOurs)
        {
            var repo = NewTemp();
            Git(repo, "init -q");
            Git(repo, "config user.email test@test");
            Git(repo, "config user.name Test");
            Git(repo, "config core.autocrlf false");
            Git(repo, "config commit.gpgsign false");
            writeOurs(repo);
            Git(repo, "add -A");
            Git(repo, "commit -q -m ours --no-verify");
            return repo;
        }

        // --- tests -----------------------------------------------------------

        [Fact]
        public void Apply_NonOverlappingChange_MergesCleanly_AndKeepsOutOfScope()
        {
            // OURS = base + a patch on line2; THEIRS changes line6 (well-separated, non-overlapping).
            var repo = InitRepo(r =>
            {
                Write(r, "lib/A.cs", "l1\nPATCHED2\nl3\nl4\nl5\nl6\n");
                Write(r, "outside/other.cs", "keep me\n");
            });
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "l1\nl2\nl3\nl4\nl5\nl6\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "l1\nl2\nl3\nl4\nl5\nUPSTREAM6\n");

            try
            {
                var result = new DeltaMerge(repo, new[] { "lib" }).Apply(baseDir, theirsDir);

                Assert.True(result.Clean);
                Assert.Empty(result.Conflicts);
                var merged = Read(repo, "lib/A.cs");
                Assert.Contains("PATCHED2", merged);   // fork patch preserved
                Assert.Contains("UPSTREAM6", merged);  // upstream change applied
                Assert.Equal("keep me\n", Read(repo, "outside/other.cs")); // out-of-scope untouched
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void Apply_OverlappingChange_ReportsConflict()
        {
            var repo = InitRepo(r => Write(r, "lib/A.cs", "line1\nPATCHED2\nline3\n"));
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "line1\nline2\nline3\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "line1\nUPSTREAM2\nline3\n"); // same line as the patch

            var merge = new DeltaMerge(repo, new[] { "lib" });
            try
            {
                var result = merge.Apply(baseDir, theirsDir);

                Assert.False(result.Clean);
                Assert.Contains("lib/A.cs", result.Conflicts);
            }
            finally
            {
                merge.Abort();
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void Apply_NewUpstreamFile_IsAdded()
        {
            var repo = InitRepo(r => Write(r, "lib/A.cs", "same\n"));
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "same\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "same\n");
            Write(theirsDir, "lib/B.cs", "brand new\n"); // added upstream

            try
            {
                var result = new DeltaMerge(repo, new[] { "lib" }).Apply(baseDir, theirsDir);

                Assert.True(result.Clean);
                Assert.True(File.Exists(Path.Combine(repo, "lib", "B.cs")));
                Assert.Equal("brand new\n", Read(repo, "lib/B.cs"));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void Apply_EmptyDelta_IsCleanNoOp()
        {
            var repo = InitRepo(r => Write(r, "lib/A.cs", "line1\nPATCHED2\n"));
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "line1\nline2\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "line1\nline2\n"); // base == theirs -> no upstream change

            try
            {
                var result = new DeltaMerge(repo, new[] { "lib" }).Apply(baseDir, theirsDir);

                Assert.True(result.Clean);
                Assert.Empty(result.Conflicts);
                Assert.Equal("line1\nPATCHED2\n", Read(repo, "lib/A.cs")); // OURS unchanged
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }
    }
}
