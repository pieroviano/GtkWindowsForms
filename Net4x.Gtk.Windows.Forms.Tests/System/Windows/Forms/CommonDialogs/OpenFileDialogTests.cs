using Gtk;

namespace System.Windows.Forms.CommonDialogs;

public class OpenFileDialogTests
{
    // Minimal concrete OpenFileDialog so tests do not open native UI.
    internal class TestOpenFileDialog : OpenFileDialog
    {
        protected override bool RunDialog(IWin32Window? owner) => false;
    }

    [Fact]
    public void Defaults_AreCorrect()
    {
        var dlg = new TestOpenFileDialog();

        Assert.False(dlg.Multiselect);
        Assert.False(dlg.ReadOnlyChecked);
        Assert.False(dlg.ShowReadOnly);

        // When FileName is null, SafeFileName returns empty string per implementation.
        Assert.Equal(string.Empty, dlg.SafeFileName);

        // When FileNames is null or empty, SafeFileNames returns an empty array.
        Assert.Empty(dlg.SafeFileNames);
    }

    [Fact]
    public void Multiselect_Setting_ReflectedOnBase()
    {
        var dlg = new TestOpenFileDialog
        {
            Multiselect = true
        };

        Assert.True(dlg.Multiselect);

        // The FileDialog.Multiselect (internal) should also reflect the value.
        var baseAsFileDialog = (FileDialog)dlg;
        Assert.True(baseAsFileDialog.Multiselect);
    }

    [Fact]
    public void SafeFileNameAndSafeFileNames_ReturnFileNamesOnly()
    {
        var dlg = new TestOpenFileDialog
        {
            FileName = Path.Combine("C:", "folder", "myfile.txt")
        };

        Assert.Equal("myfile.txt", dlg.SafeFileName);

        // FileNames has an internal setter; tests assembly has access via InternalsVisibleTo.
        dlg.FileNames = new[] { Path.Combine("C:", "a", "one.txt"), Path.Combine("D:", "b", "two.txt") };
        Assert.Equal(new[] { "one.txt", "two.txt" }, dlg.SafeFileNames);
    }

    [Fact]
    public void OpenFile_ReturnsStream_WhenFileExists_AndNull_WhenMissing()
    {
        var dlg = new TestOpenFileDialog
        {
            // ensure non-existent returns null
            FileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tmp")
        };

        var missing = dlg.OpenFile();
        Assert.Null(missing);

        // create temp file and verify OpenFile returns readable stream
        var tmp = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tmp, "hello");
            dlg.FileName = tmp;

            using var stream = dlg.OpenFile();
            Assert.NotNull(stream);
            Assert.True(stream!.CanRead);

            // validate content length
            Assert.Equal(new FileInfo(tmp).Length, stream.Length);
        }
        finally
        {
            // cleanup
            try { File.Delete(tmp); } catch { }
        }
    }

    [Fact]
    public void ShowDialog_SetsActionTypeToOpen()
    {
        var dlg = new TestOpenFileDialog();

        // Call ShowDialog (RunDialog overridden to return false) - safe, does not open UI.
        var result = dlg.ShowDialog(null);

        // ShowDialog should have set the internal ActionType to Open.
        var baseAsFileDialog = (FileDialog)dlg;
        Assert.Equal(FileChooserAction.Open, baseAsFileDialog.ActionType);

        // Since RunDialog returned false, ShowDialog should map to DialogResult.Cancel.
        Assert.Equal(DialogResult.Cancel, result);
    }
}