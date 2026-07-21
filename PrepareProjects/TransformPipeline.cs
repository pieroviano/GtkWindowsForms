using System.Reflection;
using System.Text;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Logging;
using Prepare.Common.Projects;
using Prepare.Common.Text;
using PrepareProjects.Preparation;
using PrepareProjects.Preparation.Compilation;
using PrepareProjects.Preparation.Directories;
using PrepareProjects.Preparation.Source;
using PrepareProjects.ProjectHandling;
using XmlDocSync.Services;

namespace PrepareProjects;

/// <summary>
/// The transform <c>f</c>: regenerates the <c>Net4x.*</c> projects from the source submodule
/// (copy → translate → reorganize → split drawing → format → type-split), <b>excluding</b> the
/// source hooks, patch application, build, and publish. Extracted from <see cref="Program"/> so both
/// PrepareProjects and AlignProjects run the identical transform. See AlignProjects-Plan.md §3.1.
/// </summary>
public static class TransformPipeline
{
    /// <summary>
    /// Runs the transform against the source submodule and returns the paths/state the caller needs
    /// for the subsequent patch/build/publish steps. Behavior-identical to the original inline body
    /// in <see cref="Program"/>.
    /// </summary>
    public static async Task<TransformResult> RunTransform()
    {
        PrepareWorker.RenameDirectoriesInSourceRepository();

#if REFRESH_XML_COMMENT_FILES
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
        // The sample transform fs - shared with AlignSamples (see SampleTransformPipeline).
        var projectName = await GtkSampleAppProjectsHandler.PrepareAsync(
            SampleTransformPipeline.Samples[0].SourceProjectName, SampleTransformPipeline.Samples[0].TargetProjectName);
        var otherProjectName = await GtkSampleAppProjectsHandler.PrepareAsync(
            SampleTransformPipeline.Samples[1].SourceProjectName, SampleTransformPipeline.Samples[1].TargetProjectName);

        ConsoleColor.Green.Write(@"Regenerating *.Designer.cs from resources for PoC projects", true);
        ResGenService.RegenerateDesignersFromResources(targetDir, projectName, otherProjectName);

        ConsoleColor.Green.Write(@"Copying code from Resources", true);
        ResourcesCopyService.CopyCodeFromResources(targetDir);

        ConsoleColor.Green.Write(@"Appling translations and first post processings", true);
        var fullPath = RepoPaths.At(Path.Combine("Resources", BackupUtility.PostProcessing));
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms", "mono" }, fullPath);

        ConsoleColor.Green.Write(@"Normalyzing comments", true);
        SummaryCommentTransformer.TransformFolder(targetDir);
        MemberSpacingFormatter.TransformFolder(targetDir);

        ConsoleColor.Green.Write(@"Removing old Xml comments generated from submodule", true);
        PrepareWorker.RemoveOldXmlComments(csprojPath);

        ConsoleColor.Green.Write($@"Reorganizing files for {net4xGtkWindowsForms}", true);
        FolderFixer.ReorganizeFiles(csprojPath, new[]
        {
            "Properties",
            "Content",
            "Resources"
        }, ["GTKControls", "GTKSystem"], ["Application"]);

        ConsoleColor.Green.Write(@"Normalizing Gtk folders", true);
        GtkFoldersService.NormalizeGtkFolders(csprojPath);
        FixService.FixClasses(csprojPath);
        var csProjectDirectory = Path.GetDirectoryName(csprojPath) ?? string.Empty;
        GtkFoldersService.RenameFolder(Path.Combine(csProjectDirectory, "GtkSystem", "Windows", "Forms"),
            "GTKControls", "GtkControls", csprojPath);

        ConsoleColor.Green.Write($@"Separating {PrepareWorker.net4xGtkDrawing} from {net4xGtkWindowsForms}", true);
        DrawingService.SeparateDrawing(PrepareWorker.net4xGtkDrawing);
        PrepareWorker.OptimizeNet4xGtkDrawingFileSystem(csProjectDirectory);

        ConsoleColor.Green.Write($@"Replacing unimplemented classes with mono equivalent", true);
        FileContentReplacer.ReplaceContent(
            RepoPaths.At(@"mono\mcs\class\System.Drawing\System.Drawing\CharacterRange.cs"),
            RepoPaths.At(@"Net4x.Gtk.Drawing\System\Drawing\CharacterRange.cs")
        );

        ConsoleColor.Green.Write(@"Applying files first post processing", true);
        var postprocessingXml = PrepareWorker.GetSubsequentPostprocessingXml(false);
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms", "mono" },
            postprocessingXml);

        ConsoleColor.Green.Write(@"Regenerating SR.Designer.cs", true);
        ResGenService.RegenerateSrDesigner(csProjectDirectory, csprojPath);

        ConsoleColor.Green.Write(@"Cleaning IntermediateBackup", true);
        BackupUtility.CleanEmptyFolders(csProjectDirectory);
        BackupUtility.CleanBackup(csProjectDirectory, BackupUtility.IntermediateBackup, "Net4x.Gtk.Windows.Forms");

        ConsoleColor.Green.Write(@"Persisting IntermediateBackup", true);
        var projectFiles = ProjectUtility.GetProjectFiles(csprojPath, out _);
        ProjectUtility.PersistProjectFiles(csprojPath, projectFiles, csProjectDirectory, BackupUtility.IntermediateBackup);

        ConsoleColor.Green.Write(@"Merging Xml documentation", true);
        await PrepareWorker.MergeXmlDocumentationAsync();
        ConsoleColor.White.Write("Xml comments merged.");

        ConsoleColor.DarkMagenta.Write("Ensuring On....(...EventArgs) are raising event.", true);
        PrepareWorker.EnsureOnEventRaisesEvent();


        ConsoleColor.DarkMagenta.Write("Formatting Net4x.Gtk.Windows.Forms.csproj", true);
        DotnetFormatter.Format(csprojPath, RepoPaths.At(".editorconfig"));
        ConsoleColor.DarkMagenta.Write("Formatting Net4x.Gtk.Drawing.csproj", true);
        var net4xGtkDrawingPath = RepoPaths.At(@"Net4x.Gtk.Drawing\Net4x.Gtk.Drawing.csproj");
        DotnetFormatter.Format(net4xGtkDrawingPath, RepoPaths.At(".editorconfig"));

        ConsoleColor.Green.Write(@"Applying files latest post processing", true);
        postprocessingXml = PrepareWorker.GetSubsequentPostprocessingXml(true);
        PostProcessing.ApplyPostProcessing(targetDir, new[] { "gtksystem-windows-forms", "mono" },
            postprocessingXml);
        BackupUtility.CleanBackup(csProjectDirectory, BackupUtility.LatestBackup, "Net4x.Gtk.Windows.Forms");

        await PrepareWorker.PatchFile(RepoPaths.At(@"Net4x.Gtk.Windows.Forms\System\Windows\Forms\Dialog\FileDialog.cs"));
        await PrepareWorker.PatchFile(RepoPaths.At(@"Net4x.Gtk.Windows.Forms\System\Windows\Forms\Printing\PrintPreviewDialog.cs"));
        await PrepareWorker.PatchCommentsInFile(RepoPaths.At(@"Net4x.Gtk.Windows.Forms\System\Windows\Forms\Dialog\MessageBox\MessageBox.cs"));
        await PrepareWorker.PatchCommentsInFile(RepoPaths.At(@"Net4x.Gtk.Drawing\System\Drawing\Drawing2D\ImageAnimator.cs"));
        await PrepareWorker.PatchCommentsInFile(RepoPaths.At(@"Net4x.Gtk.Windows.Forms\System\Windows\Forms\Printing\PrintDialog.cs"));


        ConsoleColor.Green.Write(@"Beautifying on project files", true);
#pragma warning disable CS8604 // Possible null reference argument.
        var gtkDrawingDirectory = Path.GetDirectoryName(net4xGtkDrawingPath);
        var exclude = new string[] { "GtkSystem\\Resources\\ResourceManager.cs" };
        var removeUnusedUsings = true;
        await CsBeautyfier.ProcessFolderAsync(gtkDrawingDirectory, exclude, removeUnusedUsings);
        await CsBeautyfier.ProcessFolderAsync(csProjectDirectory, exclude, removeUnusedUsings);
#pragma warning restore CS8604 // Possible null reference argument.

        ConsoleColor.Green.Write(@"Normalizing end of line on project files", true);
        LineEndingNormalizer.NormalizeCsFiles(gtkDrawingDirectory);
        LineEndingNormalizer.NormalizeCsFiles(csProjectDirectory);

        ConsoleColor.Green.Write(@"Splitting project files with multiple types", true);
        TypeSplitService.ProcessDirectory(gtkDrawingDirectory, [".Designer.cs"]);
        TypeSplitService.ProcessDirectory(csProjectDirectory,
                                                  [
                                                      ".Designer.cs",
                                                      @"System\Windows\Forms\MenuStripBase\ToolStripDropDownItem.cs",
                                                      @"System\Windows\Forms\PropertyGridBase\PropertyGrid.Flags.cs",
                                                      @"System\Windows\Forms\PropertyGridBase\PropertyGrid.PropertyTabCollection.cs",
                                                      @"System\Windows\Forms\PropertyGridBase\PropertyGrid.TabInfo.cs",
                                                      @"System\Resources\ResXResourceReader.cs",
                                                      @"System\Resources\ResXResourceReader.ReaderAliasResolver.cs",
                                                      @"System\Resources\ResXFileRef.cs",
                                                      @"System\Resources\ResxFileRef.Converter.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.IntegerCollection.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.IntegerCollection.CustomTabOffsetsEnumerator.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.ObjectCollection.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.SelectedIndexCollection.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.SelectedIndexCollection.SelectedIndexEnumerator.cs",
                                                      @"System\Windows\Forms\ListBoxBase\ListBox.SelectedObjectCollection.cs",
                                                      @"System\Windows\Forms\ComboBoxBase\ComboBox.ObjectCollection.cs",
                                                      @"System\Windows\Forms\ComboBoxBase\ComboBox.ObjectCollection.Entry.cs",
                                                      @"System\Windows\Forms\ComboBoxBase\ComboBox.ObjectCollection.EntryEnumerator.cs",
                                                      @"Content\System.ComponentModel.cs",
                                                      @"Content\System.Resources.ResourceManager.cs",
                                                      @"GtkSystem\Windows\Forms\GtkControls\ControlBase\GtkControlOverride.cs",
                                                      @"System\Resources\ResXFileRef.cs",
                                                      @"System\Resources\ResXResourceReader.cs",
                                                      //@""
                                                  ]);

        return new TransformResult(targetDir, csprojPath, csProjectDirectory,
            net4xGtkDrawingPath, gtkDrawingDirectory ?? string.Empty, updatePropsFiles);
    }
}

/// <summary>Paths and deferred actions the transform produces, consumed by the patch/build/publish steps.</summary>
public sealed class TransformResult
{
    public TransformResult(string targetDir, string csprojPath, string csProjectDirectory,
        string net4xGtkDrawingPath, string gtkDrawingDirectory, Action updatePropsFiles)
    {
        TargetDir = targetDir;
        CsprojPath = csprojPath;
        CsProjectDirectory = csProjectDirectory;
        Net4xGtkDrawingPath = net4xGtkDrawingPath;
        GtkDrawingDirectory = gtkDrawingDirectory;
        UpdatePropsFiles = updatePropsFiles;
    }

    public string TargetDir { get; }
    public string CsprojPath { get; }
    public string CsProjectDirectory { get; }
    public string Net4xGtkDrawingPath { get; }
    public string GtkDrawingDirectory { get; }
    public Action UpdatePropsFiles { get; }
}
