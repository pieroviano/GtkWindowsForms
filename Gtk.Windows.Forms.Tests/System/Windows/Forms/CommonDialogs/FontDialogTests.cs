extern alias gtk;
using Font = gtk::System.Drawing.Font;

namespace System.Windows.Forms.CommonDialogs;

public class FontDialogTests
{
    // Minimal concrete FontDialog so tests do not open native UI.
    internal class TestFontDialog : FontDialog
    {
        protected override bool RunDialog(IWin32Window? owner) => false;
    }

    [Fact]
    public void Defaults_AreCorrect()
    {
        var dlg = new TestFontDialog();

        // Implementation currently only resets the Font in Reset(); other boolean properties are default(bool) == false.
        Assert.False(dlg.AllowFullOpen);
        Assert.False(dlg.FullOpen);
        Assert.False(dlg.ShowHelp);

        Assert.Null(dlg.Font);
    }

    [Fact]
    public void Font_GetterAndSetter_Works()
    {
        var dlg = new TestFontDialog();
        var font = new Font("Arial", 12);

        dlg.Font = font;

        Assert.NotNull(dlg.Font);
        Assert.Equal(font.Name, dlg.Font?.Name);
        Assert.Equal(font.Size, dlg.Font?.Size);
    }

    [Fact]
    public void Reset_ClearsFont()
    {
        var dlg = new TestFontDialog
        {
            Font = new Font("Times New Roman", 10)
        };

        dlg.Reset();

        Assert.Null(dlg.Font);
    }

    [Fact]
    public void ToString_IncludesNameAndSize_WhenFontSet()
    {
        var dlg = new TestFontDialog();
        var font = new Font("Courier New", 11);
        dlg.Font = font;

        Assert.Equal($"{font.Name} {font.Size}", dlg.ToString());
    }

    [Fact]
    public void ToString_ReturnsSingleSpace_WhenFontIsNull()
    {
        var dlg = new TestFontDialog();
        // Default constructed dialog resets font to null
        Assert.Equal(" ", dlg.ToString());
    }
}