using System.Drawing;

namespace System.Windows.Forms.CommonDialogs;

public class ColorDialogTests
{
    [Fact]
    public void Defaults_AreExpected()
    {
        var dlg = new ColorDialog();

        Assert.True(dlg.AllowFullOpen, "AllowFullOpen should default to true");
        Assert.False(dlg.AnyColor, "AnyColor should default to false");
        Assert.False(dlg.FullOpen, "FullOpen should default to false");
        Assert.False(dlg.ShowHelp, "ShowHelp should default to false");
        Assert.False(dlg.SolidColorOnly, "SolidColorOnly should default to false");
        Assert.True(dlg.Color.IsEmpty, "Color should be empty by default");
        Assert.Null(dlg.CustomColors);
    }

    [Fact]
    public void SetProperties_Persist()
    {
        var dlg = new ColorDialog
        {
            AllowFullOpen = false,
            AnyColor = true,
            FullOpen = true,
            ShowHelp = true,
            SolidColorOnly = true,
            Color = Color.Red,
            CustomColors = new[] { 0xFF0000, 0x00FF00, 0x0000FF }
        };

        Assert.False(dlg.AllowFullOpen);
        Assert.True(dlg.AnyColor);
        Assert.True(dlg.FullOpen);
        Assert.True(dlg.ShowHelp);
        Assert.True(dlg.SolidColorOnly);
        Assert.Equal(Color.Red, dlg.Color);
        Assert.NotNull(dlg.CustomColors);
        Assert.Equal(3, dlg.CustomColors!.Length);
    }

    [Fact]
    public void Reset_SetsColorToBlack()
    {
        var dlg = new ColorDialog
        {
            Color = Color.Red
        };

        dlg.Reset();

        Assert.Equal(Color.Black, dlg.Color);
    }

    [Fact]
    public void ToString_ReturnsColorName()
    {
        var dlg = new ColorDialog
        {
            Color = Color.Black
        };

        var s = dlg.ToString();

        Assert.Equal("Black", s);
    }
}