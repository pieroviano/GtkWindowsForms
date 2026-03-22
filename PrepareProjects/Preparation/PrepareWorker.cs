using System.Reflection;
using Prepare.Common.Content;
using Prepare.Common.IO;
using PrepareProjects.Preparation.Directories;
using XmlDocSync.Roslyn;
using XmlDocSync.Services;

namespace PrepareProjects.Preparation;

public static class PrepareWorker
{
    public const string net4xGtkDrawing = "Net4x.Gtk.Drawing";

    public static string[] FilesToExclude = [];

    public static string[] EventsToExclude = new[]
    {
        "ParentChanged",
        "CheckStateChanged"
    };

    public static Dictionary<string, string> DefaultMap =>
        new()
        {
            { "GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventHandler", "GtkSystem.Windows.Forms.GtkControls.ControlBase.PaintGraphicsEventArgs" },
            { "System.Windows.Forms.FormClosingEventHandler", "System.Windows.Forms.FormClosingEventArgs" },
            { "System.Windows.Forms.LinkLabelLinkClickedEventHandler", "System.Windows.Forms.LinkLabelLinkClickedEventArgs" },
            { "System.Windows.Forms.ColumnClickEventHandler", "System.Windows.Forms.ColumnClickEventArgs" },
            { "System.Windows.Forms.ComponentRenameEventHandler", "ComponentRenameEventArgs" },
            { "System.Windows.Forms.DrawItemEventHandler", "System.Windows.Forms.DrawItemEventArgs" },
            { "System.Windows.Forms.ToolStripItemClickedEventHandler", "System.Windows.Forms.ToolStripItemClickedEventArgs" },
            { "System.Windows.Forms.TreeViewCancelEventHandler", "System.Windows.Forms.TreeViewCancelEventArgs" },
            { "System.Windows.Forms.ListControlConvertEventHandler", "System.Windows.Forms.ListControlConvertEventArgs" },
            { "System.Windows.Forms.DrawnHandler", "System.Windows.Forms.DrawnArgs" },
            { "System.Windows.Forms.PropertyValueChangedEventHandler", "System.Windows.Forms.PropertyValueChangedEventArgs" },
            { "System.Windows.Forms.ColumnReorderedEventHandler", "System.Windows.Forms.ColumnReorderedEventArgs" },
            {
                "System.Windows.Forms.DataGridViewAutoSizeColumnModeEventHandler",
                "System.Windows.Forms.DataGridViewAutoSizeColumnModeEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventHandler",
                "System.Windows.Forms.DataGridViewAutoSizeColumnsModeEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewAutoSizeModeEventHandler",
                "System.Windows.Forms.DataGridViewAutoSizeModeEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewBindingCompleteEventHandler",
                "System.Windows.Forms.DataGridViewBindingCompleteEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellCancelEventHandler",
                "System.Windows.Forms.DataGridViewCellCancelEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler",
                "System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellErrorTextNeededEventHandler",
                "System.Windows.Forms.DataGridViewCellErrorTextNeededEventArgs"
            },
            { "System.Windows.Forms.DataGridViewCellEventHandler", "System.Windows.Forms.DataGridViewCellEventArgs" },
            {
                "System.Windows.Forms.DataGridViewCellFormattingEventHandler",
                "System.Windows.Forms.DataGridViewCellFormattingEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellMouseEventHandler",
                "System.Windows.Forms.DataGridViewCellMouseEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellPaintingEventHandler",
                "System.Windows.Forms.DataGridViewCellPaintingEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellParsingEventHandler",
                "System.Windows.Forms.DataGridViewCellParsingEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellStateChangedEventHandler",
                "System.Windows.Forms.DataGridViewCellStateChangedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellStyleContentChangedEventHandler",
                "System.Windows.Forms.DataGridViewCellStyleContentChangedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler",
                "System.Windows.Forms.DataGridViewCellToolTipTextNeededEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellValidatingEventHandler",
                "System.Windows.Forms.DataGridViewCellValidatingEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewCellValueEventHandler",
                "System.Windows.Forms.DataGridViewCellValueEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventHandler",
                "System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewColumnEventHandler",
                "System.Windows.Forms.DataGridViewColumnEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewColumnStateChangedEventHandler",
                "System.Windows.Forms.DataGridViewColumnStateChangedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewDataErrorEventHandler",
                "System.Windows.Forms.DataGridViewDataErrorEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewEditingControlShowingEventHandler",
                "System.Windows.Forms.DataGridViewEditingControlShowingEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowCancelEventHandler",
                "System.Windows.Forms.DataGridViewRowCancelEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventHandler",
                "System.Windows.Forms.DataGridViewRowContextMenuStripNeededEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowDividerDoubleClickEventHandler",
                "System.Windows.Forms.DataGridViewRowDividerDoubleClickEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowErrorTextNeededEventHandler",
                "System.Windows.Forms.DataGridViewRowErrorTextNeededEventArgs"
            },
            { "System.Windows.Forms.DataGridViewRowEventHandler", "System.Windows.Forms.DataGridViewRowEventArgs" },
            {
                "System.Windows.Forms.DataGridViewRowHeightInfoNeededEventHandler",
                "System.Windows.Forms.DataGridViewRowHeightInfoNeededEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowHeightInfoPushedEventHandler",
                "System.Windows.Forms.DataGridViewRowHeightInfoPushedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowPostPaintEventHandler",
                "System.Windows.Forms.DataGridViewRowPostPaintEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowPrePaintEventHandler",
                "System.Windows.Forms.DataGridViewRowPrePaintEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowStateChangedEventHandler",
                "System.Windows.Forms.DataGridViewRowStateChangedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowsAddedEventHandler",
                "System.Windows.Forms.DataGridViewRowsAddedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewRowsRemovedEventHandler",
                "System.Windows.Forms.DataGridViewRowsRemovedEventArgs"
            },
            {
                "System.Windows.Forms.DataGridViewSortCompareEventHandler",
                "System.Windows.Forms.DataGridViewSortCompareEventArgs"
            },
            { "System.Windows.Forms.DateRangeEventHandler", "System.Windows.Forms.DateRangeEventArgs" },
            { "System.Windows.Forms.FormClosedEventHandler", "System.Windows.Forms.FormClosedEventArgs" },
            { "System.Windows.Forms.ItemCheckEventHandler", "System.Windows.Forms.ItemCheckEventArgs" },
            { "System.Windows.Forms.ItemCheckedEventHandler", "System.Windows.Forms.ItemCheckedEventArgs" },
            {
                "System.Windows.Forms.ListViewItemSelectionChangedEventHandler",
                "System.Windows.Forms.ListViewItemSelectionChangedEventArgs"
            },
            { "System.Windows.Forms.PaintEventHandler", "System.Windows.Forms.PaintEventArgs" },
            { "System.Windows.Forms.QuestionEventHandler", "System.Windows.Forms.QuestionEventArgs" },
            {
                "System.Windows.Forms.SelectedGridItemChangedEventHandler",
                "System.Windows.Forms.SelectedGridItemChangedEventArgs"
            },
            { "System.Windows.Forms.ScrollEventHandler", "System.Windows.Forms.ScrollEventArgs" },
            { "System.Windows.Forms.TreeViewEventHandler", "System.Windows.Forms.TreeViewEventArgs" },
            { "GtkSystem.Windows.Forms.GtkControls.ControlBase.FormBase.CloseWindowHandler", "System.EventArgs" },
            { "System.EventHandler", "System.EventArgs" },
            { "System.Windows.Forms.ControlEventHandler", "System.Windows.Forms.ControlEventArgs" },
            { "System.Windows.Forms.DragEventHandler", "System.Windows.Forms.DragEventArgs" },
            { "System.Windows.Forms.GiveFeedbackEventHandler", "System.Windows.Forms.GiveFeedbackEventArgs" },
            { "System.Windows.Forms.HelpEventHandler", "System.Windows.Forms.HelpEventArgs" },
            { "System.Windows.Forms.InvalidateEventHandler", "System.Windows.Forms.InvalidateEventArgs" },
            { "System.Windows.Forms.KeyEventHandler", "System.Windows.Forms.KeyEventArgs" },
            { "System.Windows.Forms.KeyPressEventHandler", "System.Windows.Forms.KeyPressEventArgs" },
            { "System.Windows.Forms.LayoutEventHandler", "System.Windows.Forms.LayoutEventArgs" },
            { "System.Windows.Forms.MouseEventHandler", "System.Windows.Forms.MouseEventArgs" },
            { "System.Windows.Forms.PreviewKeyDownEventHandler", "System.Windows.Forms.PreviewKeyDownEventArgs" },
            {
                "System.Windows.Forms.QueryAccessibilityHelpEventHandler",
                "System.Windows.Forms.QueryAccessibilityHelpEventArgs"
            },
            { "System.Windows.Forms.QueryContinueDragEventHandler", "System.Windows.Forms.QueryContinueDragEventArgs" },
            { "System.Windows.Forms.UICuesEventHandler", "System.Windows.Forms.UICuesEventArgs" },
            { "System.ComponentModel.CancelEventHandler", "System.ComponentModel.CancelEventArgs" },
            { "System.ComponentModel.CollectionChangeEventHandler", "System.ComponentModel.CollectionChangeEventArgs" },
            { "System.EventHandler<DialogEventArgs>", "System.Windows.Forms.DialogEventArgs" }
        };

    public static void EnsureOnEventRaisesEvent()
    {
        var myProjectDir = "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms\\";
        var targets = Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories);

        ConsoleColor.Green.Write($"Processing files from: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in targets.Where(i => FilesToExclude.All(e => !i.EndsWith(e))))
        {
            ConsoleColor.DarkGray.Write($"Processing: {file}");
            EventInvocatorFileRewriter.RewriteEventInvocations(file, new[] { "Paint" });
            EventOnMethod(file);
        }

        ConsoleColor.Green.Write($"Applying code style for: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories))
        {
            ConsoleColor.DarkGray.Write($"Applying code style: {Path.GetFullPath(file)}");
            CsFileOrganizer.ProcessFile(file);
        }

        myProjectDir = "..\\..\\..\\..\\Net4x.Gtk.Drawing\\";
        targets = Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories);

        ConsoleColor.Green.Write($"Processing files from: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in targets)
        {
            ConsoleColor.DarkGray.Write($"Processing: {file}");
            EventInvocatorFileRewriter.RewriteEventInvocations(file, new[] { "Paint" });
            EventOnMethod(file);
        }

        ConsoleColor.Green.Write($"Applying code style for: {Path.GetFullPath(myProjectDir)}", true);
        foreach (var file in Directory.GetFiles(myProjectDir, "*.cs", SearchOption.AllDirectories))
        {
            ConsoleColor.DarkGray.Write($"Applying code style: {Path.GetFullPath(file)}");
            CsFileOrganizer.ProcessFile(file);
        }
    }

    public static string GetSolutionPath(string projectDirectory)
    {
        var solutionPath = Path.Combine(projectDirectory, "..\\Net4x.Gtk.Windows.Forms.sln");
        return solutionPath;
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

    public static async Task MergeXmlDocumentationAsync()
    {
        var fixes = new (string ToReplace, string DestinationValue)[]
        {
            ("Gets or sets a value indicating whether use ex dialog.",
                "<summary>Gets or sets a value indicating whether use ex dialog."),
            ("supplied padding size for all edges.", "supplied padding size for all edges.</summary>"),
            ("Gets or sets a value indicating if we use ex dialog.",
                "<summary>Gets or sets a value indicating if we use ex dialog."),
            ("</summary></param>", "</param>"),
            ("Gets or sets a value indicating whether the dialog should be shown",
                "<summary>Gets or sets a value indicating whether the dialog should be shown"),
            ("<devdov>", "&lt;devdov&gt;"),
            ("Displays a message box with specified text, caption, and style.",
                "Displays a message box with specified text, caption, and style.</summary>"),
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
#if DONT_SKIP_INITIAL
        ConsoleColor.DarkMagenta.Write("Processing XML comments from System.Windows.Forms.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\System.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes);
        ConsoleColor.DarkMagenta.Write("Processing XML comments from System.Drawing.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\System.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes);
#endif
        ConsoleColor.DarkMagenta.Write("Processing XML comments from Net4x.Gtk.Windows.Forms.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\Net4x.Gtk.Windows.Forms.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Windows.Forms", fixes, true, false);
        ConsoleColor.DarkMagenta.Write("Processing XML comments from Net4x.Gtk.Drawing.xml", true);
        await XmlDocumentationService.MergeXmlDocumentationAsync(
            "..\\..\\..\\..\\Resources\\Net4x.Gtk.Drawing.xml",
            "..\\..\\..\\..\\Net4x.Gtk.Drawing", fixes, true, false);
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
        LogUtility.FileCopyAndLog(@"..\..\..\..\Resources\GtkLogger.cs", Path.Combine(projectDirectory, @"..\Net4x.Gtk.Drawing\System\Diagnostics\GtkLogger.cs"));
    }

    public static async Task PatchCommentsInFile(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        text = text.Replace(
            "\n    //END ADD                                                                                                      //",
            "");
        text = text.Replace("Displays a message box with specified text, caption, and style with Help Button.",
            "Displays a message box with specified text, caption, and style with Help Button.\n    /// <summary>");
        text = text.Replace(
            "\n    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////",
            "");
        text = text.Replace("\n   /// <summary>\n   /// </summary>", "\n   /// </summary>");
        text = text.Replace(
            "/// Displays a message box with specified text, caption, and style with Help Button.\n    /// <summary>",
            "/// Displays a message box with specified text, caption, and style with Help Button.\n    /// </summary>");
        text = text.Replace("    /// </summary>\n    /// </summary>", "    /// </summary>");
        text = text.Replace("/// <param name = \"pixbuf\">The raw pixbuf bytes.",
            "/// <param name = \"image\">The image to use.");
        text = text.Replace("<summary>Gets or sets a value indicating if we use ex dialog.",
            "Gets or sets a value indicating if we use ex dialog.");
        await CsFile.WriteAllTextAsync(path, text);
    }

    public static async Task PatchFile(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        text = text.Replace("System.System.", "System.");
        await CsFile.WriteAllTextAsync(path, text);
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
        if (!File.Exists(path)) File.Delete(path);
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

    private static void EventOnMethod(string file)
    {
        var source = File.ReadAllText(file);
        var result = EventOnMethodGenerator.Process(source, EventsToExclude, DefaultMap);
        if (result != source) CsFile.WriteAllText(file, result);
    }
}