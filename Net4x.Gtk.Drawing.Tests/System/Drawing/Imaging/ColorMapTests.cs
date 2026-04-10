extern alias gtk;

using ColorMap = gtk::System.Drawing.Imaging.ColorMap;

namespace System.Drawing.Imaging;

public class ColorMapTests
{
    #region Constructor

    [Fact]
    public void Constructor_DefaultValues()
    {
        var map = new ColorMap();

        Assert.Equal(default, map.OldColor);
        Assert.Equal(default, map.NewColor);
    }

    #endregion

    #region OldColor

    [Fact]
    public void OldColor_SetGet_Works()
    {
        var map = new ColorMap();
        var color = Color.Red;

        map.OldColor = color;

        Assert.Equal(color, map.OldColor);
    }

    [Fact]
    public void OldColor_CanBeChanged()
    {
        var map = new ColorMap
        {
            OldColor = Color.Red
        };

        map.OldColor = Color.Blue;

        Assert.Equal(Color.Blue, map.OldColor);
    }

    #endregion

    #region NewColor

    [Fact]
    public void NewColor_SetGet_Works()
    {
        var map = new ColorMap();
        var color = Color.Green;

        map.NewColor = color;

        Assert.Equal(color, map.NewColor);
    }

    [Fact]
    public void NewColor_CanBeChanged()
    {
        var map = new ColorMap
        {
            NewColor = Color.Green
        };

        map.NewColor = Color.Yellow;

        Assert.Equal(Color.Yellow, map.NewColor);
    }

    #endregion

    #region Combined Behavior

    [Fact]
    public void OldColor_And_NewColor_AreIndependent()
    {
        var map = new ColorMap
        {
            OldColor = Color.Black,
            NewColor = Color.White
        };

        Assert.Equal(Color.Black, map.OldColor);
        Assert.Equal(Color.White, map.NewColor);
    }

    #endregion
}