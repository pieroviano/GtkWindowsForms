using System.Diagnostics;
using Prepare.Common.IO;
using Prepare.Common.Logging;
using Prepare.Common.Projects;
using PrepareProjects.Preparation;

namespace PrepareProjects;

public class Program
{
    public static async Task Main()
    {
        // The transform rewrites the generated trees from the submodule, so any hand edit sitting there
        // uncommitted is about to be destroyed - and, not being in patches/, will not come back. Say so
        // before doing it rather than after.
        if (!GeneratedTreeGuard.VerifyBeforeTransform(Path.GetFullPath(@"..\..\..\..\")))
        {
            ConsoleColor.Red.Write("Aborting: capture the changes above first.", true);

            return;
        }

        // Pin the source fork to the fixed generation base (detached) so every run transforms
        // the same source and the patch series applies. Returns the branch to advance on exit.
        var sourceTrackingBranch = SourceRepositorySync.CheckoutGenerationBase();

        // The transform f (copy → translate → reorg → split → format → type-split), shared with AlignProjects.
        var t = await TransformPipeline.RunTransform();

        ConsoleColor.Green.Write(@"Applying patches the solution", true);
        GitPatchApplier.CommitAndApplyPatches();

        ConsoleColor.Green.Write(@"Persisting latest project files", true);
        var projectFiles = ProjectUtility.GetProjectFiles(t.CsprojPath, out _);
        ProjectUtility.PersistProjectFiles(t.CsprojPath, projectFiles, t.CsProjectDirectory, BackupUtility.LatestBackup);
        var directoryName = t.GtkDrawingDirectory;
        projectFiles = ProjectUtility.GetProjectFiles(t.Net4xGtkDrawingPath, out _);
        ProjectUtility.PersistProjectFiles(t.Net4xGtkDrawingPath, projectFiles, directoryName, BackupUtility.LatestBackup);
        BackupUtility.DeleteBackupFolders("..\\..\\..\\..\\");

        ConsoleColor.Green.Write(@"Building the solution", true);
        t.UpdatePropsFiles.Invoke();
        var solutionPath = PrepareWorker.GetSolutionPath(t.CsProjectDirectory);
        ProjectCompilation.RestoreSolution(solutionPath);
        ProjectCompilation.BuildSolution(solutionPath);
        ProjectCompilation.BuildSolution(Path.GetFullPath("..\\..\\..\\..\\Prepare.Common\\Prepare.Common.csproj"));
        var folder = Path.GetFullPath(@"..\..\..\..\Packages");

        var files = Directory
            .EnumerateFiles(folder, $"*-a{DateTime.Today.Year}*", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                RedConsole.Write($"Error deleting file {file}: {ex}");
                throw;
            }
        }

        if (Environment.GetEnvironmentVariable("SkipPush") == "")
        {
            var exe = Environment.ExpandEnvironmentVariables(
                "%USERPROFILE%\\.nuget\\packages\\net4x.publishnuget\\1.0.0\\tools\\PublishNuget.exe");
            var process = Process.Start(exe);
            await process.WaitForExitAsync();
        }

        // Advance the source fork to the latest gitee upstream so it stays aligned for next time.
        ConsoleColor.Green.Write(@"Advancing gtksystem-windows-forms to latest gitee", true);
        SourceRepositorySync.AdvanceToLatestGitee(sourceTrackingBranch);

        ConsoleColor.Green.Write("Preparation complete", true);
    }
}
