using System.Diagnostics;
using Align.Common;
using Prepare.Common.Alignment;

namespace Prepare.Tests
{
    /// <summary>
    /// Phase 0 of AlignSamples-Plan.md: the alignment driver is shared, so it must stay parameterized -
    /// only the configured scope may be merged, and each tool must read/advance its own checkpoint only.
    /// </summary>
    public class AlignmentDriverTests
    {
        // --- helpers ---------------------------------------------------------

        private static string NewTemp()
        {
            var dir = Path.Combine(Path.GetTempPath(), "ad-" + Guid.NewGuid().ToString("N"));
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

        private static void Git(string dir, string args)
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
            p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit();
        }

        /// <summary>Runs git and returns stdout (the assertions need the output, unlike <see cref="Git"/>).</summary>
        private static string GitOutput(string dir, string args)
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
            var output = p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        private static void Write(string root, string rel, string content)
        {
            var full = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, content.Replace("\r\n", "\n"));
        }

        private static string Read(string root, string rel) =>
            File.ReadAllText(Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar))).Replace("\r\n", "\n");

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

        /// <summary>Options for a fixture run: no transform is needed when trees are supplied directly.</summary>
        private static AlignmentOptions Options(string repo, string toolName, string[] scope, string checkpointFile,
            string branchPrefix) => new()
        {
            RepoRoot = repo,
            ToolName = toolName,
            Scope = scope,
            CheckpointFile = checkpointFile,
            BranchPrefix = branchPrefix,
            Transform = _ => throw new InvalidOperationException("the transform must not run when trees are supplied")
        };

        // --- tests -----------------------------------------------------------

        [Fact]
        public void AlignFromTrees_MergesOnlyTheConfiguredScope()
        {
            // Both trees carry an upstream change, but only 'lib' is in scope: 'samples' must be left alone.
            var repo = InitRepo(r =>
            {
                Write(r, "lib/A.cs", "l1\nl2\nl3\n");
                Write(r, "samples/App.cs", "ours sample\n");
            });
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "l1\nl2\nl3\n");
            Write(baseDir, "samples/App.cs", "base sample\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "l1\nUPSTREAM\nl3\n");
            Write(theirsDir, "samples/App.cs", "upstream sample\n");
            var checkpoint = Path.Combine(NewTemp(), "last-aligned.txt");

            try
            {
                var driver = new AlignmentDriver(Options(repo, "AlignProjects", new[] { "lib" }, checkpoint, "align"));

                var exit = driver.AlignFromTrees(baseDir, theirsDir, null, "target-sha");

                Assert.Equal(0, exit);
                Assert.Contains("UPSTREAM", Read(repo, "lib/A.cs"));            // in-scope delta applied
                Assert.Equal("ours sample\n", Read(repo, "samples/App.cs"));    // out-of-scope untouched
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void AlignFromTrees_AdvancesOnlyItsOwnCheckpoint()
        {
            var repo = InitRepo(r =>
            {
                Write(r, "lib/A.cs", "l1\n");
                Write(r, "samples/App.cs", "l1\n");
            });
            var baseDir = NewTemp();
            Write(baseDir, "samples/App.cs", "l1\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "samples/App.cs", "l1\nSAMPLE CHANGE\n");

            var checkpointDir = NewTemp();
            var samplesCheckpoint = Path.Combine(checkpointDir, "AlignSamples", "last-aligned.txt");
            var projectsCheckpoint = Path.Combine(checkpointDir, "AlignProjects", "last-aligned.txt");
            AlignmentCheckpoint.Write(projectsCheckpoint, "libraries-sha");

            try
            {
                // The samples tool: its own scope, its own checkpoint, its own throwaway branches.
                var driver = new AlignmentDriver(
                    Options(repo, "AlignSamples", new[] { "samples" }, samplesCheckpoint, "align-samples"));

                var exit = driver.AlignFromTrees(baseDir, theirsDir, null, "samples-sha");

                Assert.Equal(0, exit);
                Assert.Contains("SAMPLE CHANGE", Read(repo, "samples/App.cs"));
                Assert.Equal("samples-sha", AlignmentCheckpoint.Read(samplesCheckpoint, "none"));
                Assert.Contains("AlignSamples", File.ReadAllText(samplesCheckpoint));
                // The other tool's checkpoint is untouched - samples may lag or lead the libraries.
                Assert.Equal("libraries-sha", AlignmentCheckpoint.Read(projectsCheckpoint, "none"));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir); SafeDelete(checkpointDir);
            }
        }

        [Fact]
        public void AlignFromTrees_PostMergeCheckFailures_ExitNonZero_WithoutBlockingTheMerge()
        {
            var repo = InitRepo(r => Write(r, "lib/A.cs", "l1\n"));
            var baseDir = NewTemp();
            Write(baseDir, "lib/A.cs", "l1\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "lib/A.cs", "l1\nUPSTREAM\n");
            var checkpoint = Path.Combine(NewTemp(), "last-aligned.txt");

            try
            {
                var options = new AlignmentOptions
                {
                    RepoRoot = repo,
                    ToolName = "AlignSamples",
                    Scope = new[] { "lib" },
                    CheckpointFile = checkpoint,
                    BranchPrefix = "align-samples",
                    Transform = _ => Task.CompletedTask,
                    PostMergeChecks = () => new[] { "missing zh-Hans key: Gtk_MainForm_Menu_1" }
                };

                var exit = new AlignmentDriver(options).AlignFromTrees(baseDir, theirsDir, null, "target-sha");

                Assert.Equal(1, exit);
                Assert.Contains("UPSTREAM", Read(repo, "lib/A.cs"));
                Assert.Equal("target-sha", AlignmentCheckpoint.Read(checkpoint, "none"));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void AlignFromTrees_CommitsTheAdvancedCheckpoint()
        {
            // An uncommitted checkpoint is as good as none: the next `git checkout` reverts it and the
            // following run re-applies a delta that is already in.
            var repo = InitRepo(r =>
            {
                Write(r, "samples/App.cs", "l1\n");
                Write(r, "AlignSamples/last-aligned.txt", "# AlignSamples last-aligned upstream commit (auto-managed)\ne5227ae\n");
            });
            var baseDir = NewTemp();
            Write(baseDir, "samples/App.cs", "l1\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "samples/App.cs", "l1\nUPSTREAM\n");
            var checkpoint = Path.Combine(repo, "AlignSamples", "last-aligned.txt");

            try
            {
                var driver = new AlignmentDriver(
                    Options(repo, "AlignSamples", new[] { "samples" }, checkpoint, "align-samples"));

                var exit = driver.AlignFromTrees(baseDir, theirsDir, null, "bb7e9c923882ffa9772f5cf2d8b301ff455373c9");

                Assert.Equal(0, exit);
                Assert.Equal("bb7e9c923882ffa9772f5cf2d8b301ff455373c9", AlignmentCheckpoint.Read(checkpoint, "none"));
                // Committed, not left dirty - and by pathspec, so nothing else rode along.
                Assert.Equal(string.Empty, GitOutput(repo, "status --porcelain -- AlignSamples/last-aligned.txt").Trim());
                Assert.Contains("advance checkpoint to bb7e9c9238", GitOutput(repo, "log -1 --pretty=%s"));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void AlignFromTrees_StructuralDrift_AbortsWhenConfigured_WithoutAdvancingTheCheckpoint()
        {
            // OURS has a file f(base) does not - layout drift the rename/move/split reducer can't explain.
            var repo = InitRepo(r =>
            {
                Write(r, "samples/App.cs", "l1\n");
                Write(r, "samples/ForkOnly.cs", "added by a patch\n");
            });
            var baseDir = NewTemp();
            Write(baseDir, "samples/App.cs", "l1\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "samples/App.cs", "l1\nUPSTREAM\n");
            var checkpoint = Path.Combine(NewTemp(), "last-aligned.txt");

            try
            {
                var options = new AlignmentOptions
                {
                    RepoRoot = repo,
                    ToolName = "AlignSamples",
                    Scope = new[] { "samples" },
                    CheckpointFile = checkpoint,
                    BranchPrefix = "align-samples",
                    AbortOnStructuralDrift = true,
                    Transform = _ => Task.CompletedTask
                };

                var exit = new AlignmentDriver(options).AlignFromTrees(baseDir, theirsDir, null, "target-sha");

                Assert.Equal(2, exit);
                Assert.Equal("l1\n", Read(repo, "samples/App.cs"));           // nothing merged
                Assert.Equal("none", AlignmentCheckpoint.Read(checkpoint, "none"));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void AlignFromTrees_UnresolvedConflicts_DoNotAdvanceTheCheckpoint()
        {
            // OURS patched the same line upstream changed -> a real conflict. The test host has stdin
            // redirected, so the conflict pause is non-interactive and leaves the work in place.
            var repo = InitRepo(r => Write(r, "samples/App.cs", "line1\nPATCHED\nline3\n"));
            var baseDir = NewTemp();
            Write(baseDir, "samples/App.cs", "line1\nline2\nline3\n");
            var theirsDir = NewTemp();
            Write(theirsDir, "samples/App.cs", "line1\nUPSTREAM\nline3\n");
            var checkpoint = Path.Combine(NewTemp(), "last-aligned.txt");

            try
            {
                var driver = new AlignmentDriver(
                    Options(repo, "AlignSamples", new[] { "samples" }, checkpoint, "align-samples"));

                var exit = driver.AlignFromTrees(baseDir, theirsDir, null, "target-sha");

                Assert.Equal(3, exit);
                Assert.Equal("none", AlignmentCheckpoint.Read(checkpoint, "none"));
                // The delta was merged into the file for the operator, markers and all.
                Assert.Contains("<<<<<<<", Read(repo, "samples/App.cs"));
                Assert.True(File.Exists(Path.Combine(repo, "conflicts.log")));
            }
            finally
            {
                SafeDelete(repo); SafeDelete(baseDir); SafeDelete(theirsDir);
            }
        }

        [Fact]
        public void MirrorPlan_NeverMirrorsTheToolDirectoryOrTheScope()
        {
            var repoRoot = NewTemp();
            foreach (var d in new[] { "AlignSamples", "GtkWinFormsApp", "GtkSystemWinFormsApp11", "PrepareProjects", "Resources", "gtksystem-windows-forms" })
            {
                Directory.CreateDirectory(Path.Combine(repoRoot, d));
            }
            File.WriteAllText(Path.Combine(repoRoot, "Directory.Build.Props"), "<Project />");

            try
            {
                var options = new AlignmentOptions
                {
                    RepoRoot = repoRoot,
                    ToolName = "AlignSamples",
                    Scope = new[] { "GtkWinFormsApp", "GtkSystemWinFormsApp11" },
                    CheckpointFile = Path.Combine(repoRoot, "AlignSamples", "last-aligned.txt"),
                    Transform = _ => Task.CompletedTask
                };

                var (junctions, files, copiedDirs) = new TransformedTreeProducer(options).MirrorPlan();

                Assert.DoesNotContain("AlignSamples", junctions.Keys);              // the tool's own build/ stays out
                Assert.DoesNotContain("GtkWinFormsApp", junctions.Keys);            // scope dirs are produced, not mirrored
                Assert.DoesNotContain("GtkSystemWinFormsApp11", junctions.Keys);
                Assert.DoesNotContain("gtksystem-windows-forms", junctions.Keys);   // the source worktree is checked out separately
                Assert.DoesNotContain("Resources", junctions.Keys);                 // copied, never junctioned
                Assert.Contains("Resources", copiedDirs.Keys);
                Assert.Contains("PrepareProjects", junctions.Keys);
                Assert.Contains("Directory.Build.Props", files.Keys);
            }
            finally
            {
                SafeDelete(repoRoot);
            }
        }
    }
}
