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
using XmlDocSync.Roslyn;
using XmlDocSync.Services;

namespace PrepareProjects;

public class Program
{
    public static async Task Main()
    {
        RenameDirectoriesInSourceRepository();

        RefreshNet4xGtkWindowsFormsXml();

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
        ConsoleUtilityEx.WriteInColor(@$"Preparing {net4xGtkWindowsForms}", ConsoleColor.Green, true);
        var (targetDir, sourceProjectPath, csprojPath) =
            await GtkWindowsFormsProjectHandler.PrepareAsync("GTKSystem.Windows.Forms", net4xGtkWindowsForms);

        const string net4XSystemResourcesExtensions = "Net4x.System.Resources.Extensions";
        ConsoleUtilityEx.WriteInColor(@$"Preparing {net4XSystemResourcesExtensions}",
            ConsoleColor.Green, true);
        SystemResourcesExtensionsProjectHandler.Prepare($"{net4XSystemResourcesExtensions}.csproj");
        var version = VersionPrepare.PrepareSolutionVersion();

        ConsoleUtilityEx.WriteInColor(@"Preparing PoC projects", ConsoleColor.Green, true);
        var projectName = await GtkSampleAppProjectsHandler.PrepareAsync("GTKWinFormsApp", "GtkWinFormsApp", version);
        var otherProjectName =
            await GtkSampleAppProjectsHandler.PrepareAsync("GTKSystemWinFormsApp11", "GtkSystemWinFormsApp11", version);

        ConsoleUtilityEx.WriteInColor(@"Regenerating *.Designer.cs from resources for PoC projects",
            ConsoleColor.Green, true);
        ResGenService.RegenerateDesignersFromResources(targetDir, projectName, otherProjectName);

        ConsoleUtilityEx.WriteInColor(@"Appling translations and first post processings",
            ConsoleColor.Green, true);
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            BackupUtility.PostProcessing);

        ConsoleUtilityEx.WriteInColor(@"Copying code from Resources", ConsoleColor.Green, true);
        ResourcesCopyService.CopyCodeFromResources(targetDir);

        ConsoleUtilityEx.WriteInColor(@"Normalyzing comments", ConsoleColor.Green, true);
        SummaryCommentTransformer.TransformFolder(targetDir);
        MemberSpacingFormatter.TransformFolder(targetDir);

        RemoveOldXmlComments(csprojPath);

        ConsoleUtilityEx.WriteInColor(@$"Reorganizing files for {net4xGtkWindowsForms}",
            ConsoleColor.Green, true);
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
        ConsoleUtilityEx.WriteInColor(@$"Separating {net4xGtkDrawing} from {net4xGtkWindowsForms}",
            ConsoleColor.Green, true);
        DrawingService.SeparateDrawing(net4xGtkDrawing);
        OptimizeNet4xGtkDrawingFileSystem(projectDirectory);

        ConsoleUtilityEx.WriteInColor(@"Applying files post processings", ConsoleColor.Green, true);

        var postprocessingXml = GetPostprocessingXml();
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms" },
            postprocessingXml);

        ResGenService.RegenerateSrDesigner(projectDirectory, csprojPath);

        BackupUtility.CleanEmptyFolders(projectDirectory);

        BackupUtility.CleanBackup(projectDirectory, BackupUtility.LatestBackup);

        await MergeXmlDocumentationAsync();

        ConsoleUtilityEx.WriteInColor("Ensuring On....(...EventArgs) are raising event.",
            ConsoleColor.DarkMagenta, true);
        EnsureOnEventRaisesEvent();


        ConsoleUtilityEx.WriteInColor("Formatting Net4x.Gtk.Windows.Forms.csproj",
            ConsoleColor.DarkMagenta, true);
        await DotnetFormatter.FormatAsync(csprojPath, Path.GetFullPath(@"..\..\..\..\.editorconfig"));
        ConsoleUtilityEx.WriteInColor("Formatting Net4x.Gtk.Drawing.csproj", ConsoleColor.DarkMagenta, true);
        await DotnetFormatter.FormatAsync("..\\..\\..\\..\\Net4x.Gtk.Drawing\\Net4x.Gtk.Drawing.csproj",
            Path.GetFullPath(@"..\..\..\..\.editorconfig"));

        ConsoleUtilityEx.WriteInColor(@"Building the solution", ConsoleColor.Green, true);
        var projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out _);
        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, projectDirectory, BackupUtility.LatestBackup);
        var solutionPath = GetSolutionPath(projectDirectory);
        ProjectCompilation.RestoreSolution(solutionPath);
        ProjectCompilation.BuildSolution(solutionPath);
        ConsoleUtilityEx.WriteInColor("Preparation complete", ConsoleColor.Green, true);
    }

    private static void EnsureOnEventRaisesEvent()
    {
        var myProjectDir = @"..\\..\\..\\..\\Net4x.Gtk.Windows.Forms\\";
        var targets = EventMethodDetector.DetectFiles(myProjectDir);

        var exclude = new[]{"ToolStripButton.cs",
            "UserControl.cs",
            "ToolStripDropDownButton.cs",
            "ToolStripDropDownMenu.cs",
            "ArrangedElement.cs"};
        foreach (var file in targets.Where(i => exclude.All(e => !i.EndsWith(e))))
        {
            ConsoleUtilityEx.WriteInColor($"Processing: {file}", ConsoleColor.DarkGray);
            EventInvocatorFileRewriter.RewriteEventInvocations(file);
        }

        myProjectDir = @"..\\..\\..\\..\\Net4x.Gtk.Drawing\\";
        targets = EventMethodDetector.DetectFiles(myProjectDir);

        foreach (var file in targets)
        {
            ConsoleUtilityEx.WriteInColor($"Processing: {file}", ConsoleColor.DarkGray);
            EventInvocatorFileRewriter.RewriteEventInvocations(file);
        }
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

    private static async Task MergeXmlDocumentationAsync()
    {
        var fixes = new (string ToReplace, string DestinationValue)[]
        {
            ("supplied padding size for all edges.", "supplied padding size for all edges.</summary>"),
            ("</summary></param>", "</param>"),
            ("Gets or sets a value indicating whether the dialog should be shown",
                "<summary>Gets or sets a value indicating whether the dialog should be shown"),
            ("<devdov>", "&lt;devdov&gt;"),
            ("</summary></exception>", "</exception>"),
            ("Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.",
                "Creates a GDI logical font (LOGFONT) structure from this System.Drawing.Font.</summary>"),
            ("Returns a handle to this System.Drawing.Font.",
                "Returns a handle to this System.Drawing.Font.</summary>"),
            ("and optionally releases the managed resources.",
                "and optionally releases the managed resources.</summary>"),
            ("Initializes a new System.Drawing.FontFamily from the specified generic font family.",
                "Initializes a new System.Drawing.FontFamily from the specified generic font family.</summary>"),
            ("</returns></summary></summary>", "</returns>")
        };
        ConsoleUtilityEx.WriteInColor("Processing Net4x.Gtk.Windows.Forms.xml", ConsoleColor.DarkMagenta, true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\PrepareProjects\\Resources\\Net4x.Gtk.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes);
        ConsoleUtilityEx.WriteInColor("Processing Net4x.Gtk.Drawing.xml", ConsoleColor.DarkMagenta, true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\PrepareProjects\\Resources\\Net4x.Gtk.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes);
#if !SKIP_INITIAL
        ConsoleUtilityEx.WriteInColor("Processing System.Windows.Forms.xml", ConsoleColor.DarkMagenta, true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\PrepareProjects\\Resources\\System.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes);
        ConsoleUtilityEx.WriteInColor("Processing System.Drawing.xml", ConsoleColor.DarkMagenta, true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\PrepareProjects\\Resources\\System.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes);
#endif
    }

    private static void OptimizeNet4xGtkDrawingFileSystem(string projectDirectory)
    {
        DrawingDirectoryMover.MoveDrawing2D(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing2D\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory,
                @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing2D\Drawing\")));
        DrawingDirectoryMover.MoveDrawing2D(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing\")));
    }

    private static void RefreshNet4xGtkWindowsFormsXml()
    {
        if (File.Exists(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.xml"))
        {
            File.Delete(@"..\..\..\Resources\Net4x.Gtk.Windows.Forms.xml");
            File.Copy(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.xml",
                @"..\..\..\Resources\Net4x.Gtk.Windows.Forms.xml");
        }

        if (File.Exists(@"..\..\..\..\Net4x.Gtk.Drawing\Net4x.Gtk.Drawing.xml"))
        {
            File.Delete(@"..\..\..\Resources\Net4x.Gtk.Drawing.xml");
            File.Copy(@"..\..\..\..\Net4x.Gtk.Drawing\Net4x.Gtk.Drawing.xml",
                @"..\..\..\Resources\Net4x.Gtk.Drawing.xml");
        }
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
