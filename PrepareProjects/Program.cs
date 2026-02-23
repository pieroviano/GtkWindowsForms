using System.Reflection;
using System.Text;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Projects;
using Prepare.Common.Text;
using PrepareProjects.Preparation.Compilation;
using PrepareProjects.Preparation.Directories;
using PrepareProjects.Preparation.Source;
using PrepareProjects.ProjectHandling;

namespace PrepareProjects;

public class Program
{
    public static async Task Main()
    {
        RenameDirectoriesInSourceRepository();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if REFERENCE
        var fullPath =
 Path.GetFullPath(Path.Combine(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.csproj"));
        if (File.Exists(fullPath))
        {
            ProcessFileUpdatesProcessor.ProcessFilesUpdates(fullPath);
        }
#endif
        const string net4xGtkWindowsForms = "Net4x.Gtk.Windows.Forms";
        ConsoleUtility.WriteInGreen(@$"Preparing {net4xGtkWindowsForms}");
        var (targetDir, sourceProjectPath, csprojPath) =
            await GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", net4xGtkWindowsForms);

        const string net4XSystemResourcesExtensions = "Net4x.System.Resources.Extensions";
        ConsoleUtility.WriteInGreen(@$"Preparing {net4XSystemResourcesExtensions}");
        SystemResourcesExtensionsProjectHandler.Prepare($"{net4XSystemResourcesExtensions}.csproj");
        var version = VersionPrepare.PrepareSolutionVersion();

        ConsoleUtility.WriteInGreen(@"Preparing PoC projects");
        var projectName = await GtkSampleAppProjectsHandler.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName =
            await GtkSampleAppProjectsHandler.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        ConsoleUtility.WriteInGreen(@"Regenerating *.Designer.cs from resources for PoC projects");
        ResGenService.RegenerateDesignersFromResources(targetDir, projectName, otherProjectName);

        ConsoleUtility.WriteInGreen(@"Appling translations and first post processings");
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            BackupUtility.PostProcessing);

        ConsoleUtility.WriteInGreen(@"Copying code from Resources");
        ResourcesCopyService.CopyCodeFromResources(targetDir);

        ConsoleUtility.WriteInGreen(@"Normalyzing comments");
        SummaryCommentTransformer.TransformFolder(targetDir);
        MemberSpacingFormatter.TransformFolder(targetDir);

        RemoveOldXmlComments(csprojPath);

        ConsoleUtility.WriteInGreen(@$"Reorganizing files for {net4xGtkWindowsForms}");
        FolderFixer.ReorganizeFiles(csprojPath, new[]
        {
            "Properties",
            "Content",
            "Resources"
        }, ["GTKControls", "GTKSystem"], ["Application"]);

        GtkFoldersService.NormalizeGtkFolders(csprojPath);
        FixService.FixClasses(csprojPath);

        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        GtkFoldersService.RenameFolder(Path.Combine(projectDirectory, "GtkSystem", "Windows", "Forms"),
            "GTKControls", "GtkControls", csprojPath);

        const string net4xGtkDrawing = "Net4x.Gtk.Drawing";
        ConsoleUtility.WriteInGreen(@$"Separating {net4xGtkDrawing} from {net4xGtkWindowsForms}");
        DrawingService.SeparateDrawing(net4xGtkDrawing);
        DrawingDirectoryMover.MoveDrawing2D(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\Drawing2D\Drawing\")), 
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\")), 
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\Drawing2D\Drawing\")));
        DrawingDirectoryMover.MoveDrawing2D(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\Drawing\")), 
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\")), 
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawings\System\Drawing\Drawing\")));

        ConsoleUtility.WriteInGreen(@"Applying files post processings");

        var postprocessingXml = GetPostprocessingXml();
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            postprocessingXml);

        ResGenService.RegenerateSrDesigner(projectDirectory, csprojPath);

        BackupUtility.CleanEmptyFolders(projectDirectory);

        BackupUtility.CleanBackup(projectDirectory, BackupUtility.LatestBackup);

        ConsoleUtility.WriteInGreen(@"Building the solution");
        var projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out _);
        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, projectDirectory, BackupUtility.LatestBackup);
        var solutionPath = GetSolutionPath(projectDirectory);
        ProjectCompilation.RestoreSolution(solutionPath);
        ProjectCompilation.BuildSolution(solutionPath);
    }

    private static string GetPostprocessingXml()
    {
        var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#pragma warning disable CS8604 // Possible null reference argument.
        var postprocessingXml =
            Path.GetFullPath(Path.Combine(directoryName, @"..\..\..\", BackupUtility.LatestPostProcessing));
#pragma warning restore CS8604 // Possible null reference argument.
        return postprocessingXml;
    }

    private static string GetSolutionPath(string projectDirectory)
    {
        var solutionPath = Path.Combine(projectDirectory, "..\\Net4x.Gtk.Windows.Forms.sln");
        return solutionPath;
    }

    private static void RemoveOldXmlComments(string csprojPath)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var path = Path.Combine(Path.GetDirectoryName(csprojPath), "GTKSystem.Windows.Forms.xml");
#pragma warning restore CS8604 // Possible null reference argument.
        if (!File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void RenameDirectoriesInSourceRepository()
    {
        var newPath = @"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GtkControls";
        if (!CaseSensitiveChecker.DirectoryExists(newPath))
        {
            Directory.Move(@"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKControls",
                @"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKControls1");
            Directory.Move(@"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKControls1", newPath);
        }

        newPath = @"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GtkSystem";
        if (!CaseSensitiveChecker.DirectoryExists(newPath))
        {
            Directory.Move(@"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKSystem",
                @"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKSystem1");
            Directory.Move(@"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKSystem1", newPath);
        }
    }
}