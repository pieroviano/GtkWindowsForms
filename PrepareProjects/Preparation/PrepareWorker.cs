using Prepare.Common.IO;
using PrepareProjects.Preparation.Directories;
using System.Diagnostics;
using System.Reflection;
using XmlDocSync.Roslyn;
using XmlDocSync.Services;

namespace PrepareProjects.Preparation;

public static class PrepareWorker
{
    public const string net4xGtkDrawing = "Net4x.Gtk.Drawing";

    public static void EnsureOnEventRaisesEvent()
    {
        var myProjectDir = @"..\\..\\..\\..\\Net4x.Gtk.Windows.Forms\\";
        var targets = EventMethodDetector.DetectFiles(myProjectDir);

        var exclude = new[]{"ToolStripButton.cs",
            "UserControl.cs",
            "ToolStripDropDownButton.cs",
            "ToolStripDropDownMenu.cs",
            "ArrangedElement.cs"};
        ConsoleColor.Green.Write($"Processing files from: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in targets.Where(i => exclude.All(e => !i.EndsWith(e))))
        {
            ConsoleColor.DarkGray.Write($"Processing: {file}");
            EventInvocatorFileRewriter.RewriteEventInvocations(file);
            EventOnMethod(file);
        }

        ConsoleColor.Green.Write($"Applying code style for: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories))
        {
            ConsoleColor.DarkGray.Write($"Applying code style: {Path.GetFullPath(file)}");
            CsFileOrganizer.ProcessFile(file);
        }

        myProjectDir = @"..\\..\\..\\..\\Net4x.Gtk.Drawing\\";
        targets = EventMethodDetector.DetectFiles(myProjectDir);

        ConsoleColor.Green.Write($"Processing files from: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in targets)
        {
            ConsoleColor.DarkGray.Write($"Processing: {file}");
            EventInvocatorFileRewriter.RewriteEventInvocations(file);
            EventOnMethod(file);
        }

        ConsoleColor.Green.Write($"Applying code style for: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories))
        {
            ConsoleColor.DarkGray.Write($"Applying code style: {Path.GetFullPath(file)}");
            CsFileOrganizer.ProcessFile(file);
        }
    }

    private static void EventOnMethod(string file)
    {
        var source=File.ReadAllText(file);
        var result = XmlDocSync.Roslyn.EventOnMethodGenerator.Process(source, XmlDocSync.Roslyn.EventOnMethodGenerator.DefaultMap);
        if (result != source)
        {
            File.WriteAllText(file, result);
        }
    }

    public static string GetSubsequentPostprocessingXml(bool isLatest)
    {
        var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#pragma warning disable CS8604 // Possible null reference argument.
        var postprocessingXml =
            Path.GetFullPath(Path.Combine(directoryName, @"..\..\..\..\Resources",
                isLatest ? BackupUtility.LatestPostProcessing : BackupUtility.IntermediatePostProcessing));
#pragma warning restore CS8604 // Possible null reference argument.
        return postprocessingXml;
    }

    public static string GetSolutionPath(string projectDirectory)
    {
        var solutionPath = Path.Combine(projectDirectory, "..\\Net4x.Gtk.Windows.Forms.sln");
        return solutionPath;
    }

    public static async Task MergeXmlDocumentationAsync()
    {
        var fixes = new (string ToReplace, string DestinationValue)[]
        {
            ("supplied padding size for all edges.", "supplied padding size for all edges.</summary>"),
            ("</summary></param>", "</param>"),
            ("Gets or sets a value indicating whether the dialog should be shown",
                "<summary>Gets or sets a value indicating whether the dialog should be shown"),
            ("<devdov>", "&lt;devdov&gt;"),
            ("Displays a message box with specified text, caption, and style.", "Displays a message box with specified text, caption, and style.</summary>"),
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
        ConsoleColor.DarkMagenta.Write("Processing XML comments from Net4x.Gtk.Windows.Forms.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\Net4x.Gtk.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes);
        ConsoleColor.DarkMagenta.Write("Processing XML comments from Net4x.Gtk.Drawing.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\Net4x.Gtk.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes);
#if !SKIP_INITIAL
        ConsoleColor.DarkMagenta.Write("Processing XML comments from System.Windows.Forms.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\System.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes);
        ConsoleColor.DarkMagenta.Write("Processing XML comments from System.Drawing.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\System.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes);
#endif
    }

    public static void OptimizeNet4xGtkDrawingFileSystem(string projectDirectory)
    {
        DrawingDirectoryMover.MoveDrawingDirectory(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing2D\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory,
                @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing2D\Drawing\")));
        DrawingDirectoryMover.MoveDrawingDirectory(
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\")),
            Path.GetFullPath(Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Drawing\Drawing\")));
    }

    public static void RefreshXmlCommentFiles()
    {
        if (File.Exists(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.xml"))
        {
            File.Delete(@"..\..\..\..\Resources\Net4x.Gtk.Windows.Forms.xml");
            var destFileName = Path.GetFullPath(@"..\..\..\..\Resources\Net4x.Gtk.Windows.Forms.xml");
            LogUtility.FileCopyAndLog(@"..\..\..\..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.xml",
                destFileName);
        }

        if (File.Exists(@"..\..\..\..\Net4x.Gtk.Drawing\Net4x.Gtk.Drawing.xml"))
        {
            File.Delete(@"..\..\..\..\Resources\Net4x.Gtk.Drawing.xml");
            var destFileName = Path.GetFullPath(@"..\..\..\..\Resources\Net4x.Gtk.Drawing.xml");
            LogUtility.FileCopyAndLog(@"..\..\..\..\Net4x.Gtk.Drawing\Net4x.Gtk.Drawing.xml",
                destFileName);
        }
    }

    public static void RemoveOldXmlComments(string csprojPath)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var path = Path.Combine(Path.GetDirectoryName(csprojPath), "GTKSystem.Windows.Forms.xml");
#pragma warning restore CS8604 // Possible null reference argument.
        if (!File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void RenameDirectoriesInSourceRepository()
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
