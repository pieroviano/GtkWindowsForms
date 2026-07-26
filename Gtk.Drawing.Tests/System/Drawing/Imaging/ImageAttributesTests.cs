extern alias gtk;

using gtk::System.Drawing.Drawing2D;
using ColorChannelFlag = gtk::System.Drawing.Imaging.ColorChannelFlag;
using ColorMap = gtk::System.Drawing.Imaging.ColorMap;
using ImageAttributes = gtk::System.Drawing.Imaging.ImageAttributes;

namespace System.Drawing.Imaging;

public class ImageAttributesTests
{
    [Fact]
    public void Ctor_CreatesInstance()
    {
        var attr = new ImageAttributes();

        Assert.NotNull(attr);
    }

    [Fact]
    public void Clone_ReturnsNewInstance()
    {
        var attr = new ImageAttributes();

        var clone = (ImageAttributes)attr.Clone();

        Assert.NotNull(clone);
        Assert.NotSame(attr, clone);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var attr = new ImageAttributes();

        attr.Dispose();
        attr.Dispose(); // should not throw
    }

    // -------------------------
    // Delegation tests
    // -------------------------

    [Fact]
    public void ClearBrushRemapTable_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearBrushRemapTable();
    }

    [Fact]
    public void ClearColorKey_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearColorKey();
    }

    [Fact]
    public void ClearColorMatrix_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearColorMatrix();
    }

    [Fact]
    public void ClearGamma_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearGamma();
    }

    [Fact]
    public void ClearNoOp_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearNoOp();
    }

    [Fact]
    public void ClearOutputChannel_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearOutputChannel();
    }

    [Fact]
    public void ClearRemapTable_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearRemapTable();
    }

    [Fact]
    public void ClearThreshold_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.ClearThreshold();
    }

    // -------------------------
    // Set methods (delegation)
    // -------------------------

    [Fact]
    public void SetColorKey_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetColorKey(Color.Black, Color.White);
    }

    [Fact]
    public void SetGamma_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetGamma(2.2f);
    }

    [Fact]
    public void SetNoOp_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetNoOp();
    }

    [Fact]
    public void SetOutputChannel_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetOutputChannel(ColorChannelFlag.ColorChannelC);
    }

    [Fact]
    public void SetThreshold_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetThreshold(0.5f);
    }

    [Fact]
    public void SetWrapMode_Overloads_DoNotThrow()
    {
        var attr = new ImageAttributes();

        attr.SetWrapMode(WrapMode.Tile);
        attr.SetWrapMode(WrapMode.Clamp, Color.Red);
        attr.SetWrapMode(WrapMode.TileFlipXY, Color.Blue, false);
    }

    [Fact]
    public void SetRemapTable_Default_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        var map = new[]
        {
            new ColorMap { OldColor = Color.Black, NewColor = Color.White }
        };

        attr.SetRemapTable(map);
    }

    [Fact]
    public void SetBrushRemapTable_Delegates_DoesNotThrow()
    {
        var attr = new ImageAttributes();

        var map = new[]
        {
            new ColorMap { OldColor = Color.Red, NewColor = Color.Green }
        };

        attr.SetBrushRemapTable(map);
    }
}