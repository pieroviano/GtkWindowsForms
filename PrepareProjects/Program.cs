using System.Diagnostics;
using System.Reflection;
using System.Text;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Projects;
using Prepare.Common.Text;
using PrepareProjects.Preparation;
using PrepareProjects.Preparation.Compilation;
using PrepareProjects.Preparation.Directories;
using PrepareProjects.Preparation.Source;
using PrepareProjects.ProjectHandling;

namespace PrepareProjects;

public class Program
{
    public static async Task Main()
    {
        PrepareWorker.RenameDirectoriesInSourceRepository();

#if RELEASE
        PrepareWorker.RefreshXmlCommentFiles();
#endif

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        const string net4xGtkWindowsForms = "Net4x.Gtk.Windows.Forms";
        ConsoleColor.Green.Write(@$"Preparing {net4xGtkWindowsForms}", true);
        var (targetDir, sourceProjectPath, csprojPath) =
            await GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", net4xGtkWindowsForms);

        const string net4XSystemResourcesExtensions = "Net4x.System.Resources.Extensions";
        ConsoleColor.Green.Write(@$"Preparing {net4XSystemResourcesExtensions}", true);
        SystemResourcesExtensionsProjectHandler.Prepare($"{net4XSystemResourcesExtensions}.csproj");
        var (version, updatePropsFiles) = VersionPrepare.PrepareSolutionVersion();

        ConsoleColor.Green.Write(@"Preparing PoC projects", true);
        var projectName = await GtkSampleAppProjectsHandler.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName =
            await GtkSampleAppProjectsHandler.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        ConsoleColor.Green.Write(@"Regenerating *.Designer.cs from resources for PoC projects", true);
        ResGenService.RegenerateDesignersFromResources(targetDir, projectName, otherProjectName);

        ConsoleColor.Green.Write(@"Copying code from Resources", true);
        ResourcesCopyService.CopyCodeFromResources(targetDir);

        ConsoleColor.Green.Write(@"Appling translations and first post processings", true);
        var fullPath = Path.GetFullPath(Path.Combine(
#pragma warning disable CS8604 // Possible null reference argument.
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\Resources\", BackupUtility.PostProcessing));
#pragma warning restore CS8604 // Possible null reference argument.
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" }, fullPath);

        ConsoleColor.Green.Write(@"Normalyzing comments", true);
        SummaryCommentTransformer.TransformFolder(targetDir);
        MemberSpacingFormatter.TransformFolder(targetDir);

        ConsoleColor.Green.Write($@"Removing old Xml comments generated from submodule", true);
        PrepareWorker.RemoveOldXmlComments(csprojPath);

        ConsoleColor.Green.Write($@"Reorganizing files for {net4xGtkWindowsForms}", true);
        FolderFixer.ReorganizeFiles(csprojPath, new[]
        {
            "Properties",
            "Content",
            "Resources"
        }, ["GTKControls", "GTKSystem"], ["Application"]);

        ConsoleColor.Green.Write($@"Normalizing Gtk folders", true);
        GtkFoldersService.NormalizeGtkFolders(csprojPath);
        FixService.FixClasses(csprojPath);
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        GtkFoldersService.RenameFolder(Path.Combine(projectDirectory, "GtkSystem", "Windows", "Forms"),
            "GTKControls", "GtkControls", csprojPath);

        ConsoleColor.Green.Write($@"Separating {PrepareWorker.net4xGtkDrawing} from {net4xGtkWindowsForms}", true);
        DrawingService.SeparateDrawing(PrepareWorker.net4xGtkDrawing);
        PrepareWorker.OptimizeNet4xGtkDrawingFileSystem(projectDirectory);

        ConsoleColor.Green.Write(@"Applying files first post processings", true);
        var postprocessingXml = PrepareWorker.GetSubsequentPostprocessingXml(false);
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            postprocessingXml);

        ConsoleColor.Green.Write(@"Regenerating SR.Designer.cs", true);
        ResGenService.RegenerateSrDesigner(projectDirectory, csprojPath);

        ConsoleColor.Green.Write(@"Cleaning IntermediateBackup", true);
        BackupUtility.CleanEmptyFolders(projectDirectory);
        BackupUtility.CleanBackup(projectDirectory, BackupUtility.IntermediateBackup, "Net4x.Gtk.Windows.Forms");

        ConsoleColor.Green.Write(@"Persisting IntermediateBackup", true);
        var projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out _);
        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, projectDirectory, BackupUtility.IntermediateBackup);

#if RELEASE
        ConsoleColor.Green.Write(@"Merging Xml documentation", true);
        await PrepareWorker.MergeXmlDocumentationAsync();
#endif

        ConsoleColor.DarkMagenta.Write("Ensuring On....(...EventArgs) are raising event.", true);
        PrepareWorker.EnsureOnEventRaisesEvent();


        ConsoleColor.DarkMagenta.Write("Formatting Net4x.Gtk.Windows.Forms.csproj", true);
        DotnetFormatter.Format(csprojPath, Path.GetFullPath(@"..\..\..\..\.editorconfig"));
        ConsoleColor.DarkMagenta.Write("Formatting Net4x.Gtk.Drawing.csproj", true);
        var net4xGtkDrawingPath = "..\\..\\..\\..\\Net4x.Gtk.Drawing\\Net4x.Gtk.Drawing.csproj";
        DotnetFormatter.Format(net4xGtkDrawingPath,
            Path.GetFullPath(@"..\..\..\..\.editorconfig"));

        ConsoleColor.Green.Write(@"Applying files latest post processings", true);
        postprocessingXml = PrepareWorker.GetSubsequentPostprocessingXml(true);
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            postprocessingXml);
        BackupUtility.CleanBackup(projectDirectory, BackupUtility.LatestBackup, "Net4x.Gtk.Windows.Forms");

        await PrepareWorker.PatchFile(@"..\..\..\..\Net4x.Gtk.Windows.Forms\System\Windows\Forms\Dialog\FileDialog.cs");
        await PrepareWorker.PatchFile(@"..\..\..\..\Net4x.Gtk.Windows.Forms\System\Windows\Forms\Printing\PrintPreviewDialog.cs");

        ConsoleColor.Green.Write(@"Persisting latest project files", true);
        projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out _);
        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, projectDirectory, BackupUtility.LatestBackup);
        var directoryName = Path.GetDirectoryName(net4xGtkDrawingPath) ?? string.Empty;
        projectFiles = ProjectUtility.GetProjectFiles(net4xGtkDrawingPath, out _);
        ProjectUtility.PersistProjectFiles(net4xGtkDrawingPath, projectFiles, directoryName, BackupUtility.LatestBackup);

        ConsoleColor.Green.Write(@"Building the solution", true);
        updatePropsFiles.Invoke();
        var solutionPath = PrepareWorker.GetSolutionPath(projectDirectory);
        ProjectCompilation.RestoreSolution(solutionPath);
        ProjectCompilation.BuildSolution(solutionPath);
#if RELEASE
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
                ConsoleColor.Red.Write($"Error deleting file {file}: {ex}", true);
                throw;
            }
        }

        var exe = Environment.ExpandEnvironmentVariables(
            "%USERPROFILE%\\.nuget\\packages\\net4x.publishnuget\\1.0.0\\tools\\PublishNuget.exe");
        var process = Process.Start(exe);
        await process.WaitForExitAsync();

#endif
        ConsoleColor.Green.Write("Preparation complete", true);
    }
}
