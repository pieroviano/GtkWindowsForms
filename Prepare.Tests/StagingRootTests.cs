using System.Diagnostics;
using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    public class StagingRootTests
    {
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
            p.Start();
            var o = p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit();
            return o.Trim();
        }

        private static void Write(string root, string rel, string content)
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content);
        }

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
            catch { /* best-effort */ }
        }

        [Fact]
        public void Create_ProducesWorktreeJunctionsAndOutputs_DisposeLeavesTargetsIntact()
        {
            var work = Path.Combine(Path.GetTempPath(), "sr-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(work);
            var sub = Path.Combine(work, "gtksystem-windows-forms");
            Directory.CreateDirectory(sub);
            Git(sub, "init -q");
            Git(sub, "config user.email t@t");
            Git(sub, "config user.name t");
            Git(sub, "config core.autocrlf false");
            Write(sub, "Source/x.txt", "hello");
            Git(sub, "add -A");
            Git(sub, "commit -q -m src --no-verify");
            var sha = Git(sub, "rev-parse HEAD");

            Write(work, "Resources/r.txt", "res");
            Write(work, "mono/m.txt", "mono");
            Write(work, ".editorconfig", "cfg");

            var stagingPath = Path.Combine(work, "staging");
            try
            {
                using (var staging = StagingRoot.Create(
                    stagingPath,
                    sub,
                    "gtksystem-windows-forms",
                    sha,
                    junctions: new Dictionary<string, string>
                    {
                        ["Resources"] = Path.Combine(work, "Resources"),
                        ["mono"] = Path.Combine(work, "mono")
                    },
                    files: new Dictionary<string, string>
                    {
                        [".editorconfig"] = Path.Combine(work, ".editorconfig")
                    },
                    emptyDirs: new[] { "Gtk.Windows.Forms", "Gtk.Drawing" }))
                {
                    // worktree checked out at the commit
                    Assert.Equal("hello", File.ReadAllText(Path.Combine(stagingPath, "gtksystem-windows-forms", "Source", "x.txt")));
                    // shared dirs visible through junctions
                    Assert.Equal("res", File.ReadAllText(Path.Combine(stagingPath, "Resources", "r.txt")));
                    Assert.Equal("mono", File.ReadAllText(Path.Combine(stagingPath, "mono", "m.txt")));
                    // copied file + empty output dirs
                    Assert.Equal("cfg", File.ReadAllText(Path.Combine(stagingPath, ".editorconfig")));
                    Assert.True(Directory.Exists(Path.Combine(stagingPath, "Gtk.Windows.Forms")));
                    Assert.True(Directory.Exists(Path.Combine(stagingPath, "Gtk.Drawing")));
                }

                // after Dispose: staging gone, but junction TARGETS must survive
                Assert.False(Directory.Exists(stagingPath));
                Assert.True(File.Exists(Path.Combine(work, "Resources", "r.txt")));
                Assert.True(File.Exists(Path.Combine(work, "mono", "m.txt")));
            }
            finally
            {
                SafeDelete(work);
            }
        }
    }
}
