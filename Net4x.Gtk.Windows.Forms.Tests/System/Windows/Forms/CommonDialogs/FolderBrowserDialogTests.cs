using System;
using Xunit;

namespace System.Windows.Forms.CommonDialogs;

public class FolderBrowserDialogTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var dlg = new FolderBrowserDialog();

        // RootFolder is initialized to Desktop by the implementation
        Assert.Equal(Environment.SpecialFolder.Desktop, dlg.RootFolder);

        // No selected path initially
        Assert.Null(dlg.SelectedPath);

        // Flags default to false (unset)
        Assert.False(dlg.SelectedPathNeedsCheck);
        Assert.False(dlg.ShowNewFolderButton);

        // SelectedPaths delegates to base.FileNames which is null by default
        Assert.Null(dlg.SelectedPaths);
    }

    [Fact]
    public void SelectedPath_GetterAndSetter_Work()
    {
        var dlg = new FolderBrowserDialog
        {
            SelectedPath = @"C:\temp\folder"
        };

        Assert.Equal(@"C:\temp\folder", dlg.SelectedPath);

        // Setting SelectedPath does not implicitly populate SelectedPaths (FileNames)
        Assert.Null(dlg.SelectedPaths);
    }

    [Fact]
    public void SelectedPaths_Returns_FileNames()
    {
        var dlg = new FolderBrowserDialog
        {
            // FileNames has an internal setter on FileDialog; tests assembly is allowed via InternalsVisibleTo
            FileNames = new[] { "one", "two" }
        };

        Assert.Equal(new[] { "one", "two" }, dlg.SelectedPaths);
    }

    [Fact]
    public void Reset_RestoresExpectedDefaults()
    {
        var dlg = new FolderBrowserDialog
        {
            RootFolder = Environment.SpecialFolder.MyComputer,
            Description = "desc",
            SelectedPath = @"C:\temp",
            SelectedPathNeedsCheck = true,
            ShowNewFolderButton = false,
            // Also exercise FileNames being non-null before reset
            FileNames = new[] { "a", "b" }
        };

        dlg.Reset();

        // FolderBrowserDialog.Reset sets these explicitly
        Assert.Equal(Environment.SpecialFolder.Desktop, dlg.RootFolder);
        Assert.Equal(string.Empty, dlg.Description);
        Assert.Equal(string.Empty, dlg.SelectedPath);
        Assert.False(dlg.SelectedPathNeedsCheck);
        Assert.True(dlg.ShowNewFolderButton);

        // Base Reset should clear FileNames
        Assert.Null(dlg.SelectedPaths);
    }
}