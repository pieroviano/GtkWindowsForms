namespace System.Windows.Forms.CommonDialogs;

public class FileDialogTests
{
    // Minimal concrete FileDialog so we can exercise base-class behavior without showing UI.
    internal class TestFileDialog : FileDialog
    {
        // Expose RunDialog for the abstract contract; keep it non-blocking for tests.
        protected override bool RunDialog(IWin32Window? owner) => false;
    }

    [Fact]
    public void Filter_Default_IsEmptyString()
    {
        var dlg = new TestFileDialog();

        Assert.Equal(string.Empty, dlg.Filter);
    }

    [Fact]
    public void SetFilter_Valid_DoesNotThrow()
    {
        var dlg = new TestFileDialog();

        var valid = "Text files|*.txt|All files|*.*";

        var ex = Record.Exception(() => dlg.Filter = valid);

        Assert.Null(ex);
        Assert.Equal(valid, dlg.Filter);
    }

    [Fact]
    public void SetFilter_SingleSegment_ThrowsArgumentException()
    {
        var dlg = new TestFileDialog();

        Assert.Throws<ArgumentException>(() => dlg.Filter = "InvalidFilterWithoutPipes");
    }

    [Fact]
    public void SetFilter_PatternWithoutDot_ThrowsArgumentException()
    {
        var dlg = new TestFileDialog();

        // second token (pattern) has no dot -> invalid per setter validation
        var invalid = "Name|*|Other|*.*";

        Assert.Throws<ArgumentException>(() => dlg.Filter = invalid);
    }

    [Fact]
    public void Reset_RestoresExpectedDefaults()
    {
        var dlg = new TestFileDialog
        {
            // Set several properties to non-default values.
            AddExtension = false,
            Title = "T",
            InitialDirectory = "C:\\",
            FileName = "f",
            FileNames = new[] { "a", "b" },
            Filter = "Text files|*.txt|All files|*.*",
            FilterIndex = 2,
            SupportMultiDottedExtensions = true
        };


        // Call Reset and verify expected values per implementation.
        dlg.Reset();

        Assert.True(dlg.AddExtension);
        Assert.Null(dlg.Title);
        Assert.Null(dlg.InitialDirectory);
        Assert.Null(dlg.FileName);
        Assert.Null(dlg.FileNames);
        Assert.Equal(string.Empty, dlg.Filter);
        Assert.Equal(1, dlg.FilterIndex);
        Assert.False(dlg.SupportMultiDottedExtensions);
    }
}