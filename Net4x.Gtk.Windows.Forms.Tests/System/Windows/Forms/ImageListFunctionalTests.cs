extern alias gtk;
using System.Windows.Forms;
using Bitmap = gtk::System.Drawing.Bitmap;
using Image = gtk::System.Drawing.Image;
using Size = global::System.Drawing.Size;
using Color = global::System.Drawing.Color;

namespace Net4x.Gtk.Windows.Forms.Tests.System.Windows.Forms;

/// <summary>Behavioural tests for <see cref="ImageList"/> and its image collection. An ImageList is a keyed,
/// ordered store: images must come back in the order they were added, keys must resolve to the right image,
/// and removing one must shift the rest.</summary>
[Collection("NonParallelCollection")]
public class ImageListFunctionalTests
{
    private static Bitmap Swatch(Color color, int size = 8)
    {
        var bmp = new Bitmap(size, size);
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < size; y++)
            {
                bmp.SetPixel(x, y, color);
            }
        }

        return bmp;
    }

    [Fact]
    public void NewImageList_IsEmpty_WithTheDefaultImageSize()
    {
        using var list = new ImageList();

        Assert.Empty(list.Images);
        Assert.True(list.Images.Empty, "a new image list should report itself empty");
        Assert.Equal(new Size(16, 16), list.ImageSize);
    }

    [Fact]
    public void ImageSize_RoundTrips()
    {
        using var list = new ImageList { ImageSize = new Size(32, 24) };

        Assert.Equal(new Size(32, 24), list.ImageSize);
    }

    [Fact]
    public void Add_AppendsInOrder_AndCountTracksIt()
    {
        using var list = new ImageList();

        list.Images.Add(Swatch(Color.Red));
        list.Images.Add(Swatch(Color.Lime));

        Assert.Equal(2, list.Images.Count);
        Assert.False(list.Images.Empty, "a populated image list is not empty");
        Assert.NotNull(list.Images[0]);
        Assert.NotNull(list.Images[1]);
    }

    [Fact]
    public void Add_WithAKey_MakesTheImageReachableByThatKey()
    {
        using var list = new ImageList();

        list.Images.Add("red", Swatch(Color.Red));

        Assert.True(list.Images.ContainsKey("red"), "the key just added should be present");
        Assert.NotNull(list.Images["red"]);
        Assert.Equal(0, list.Images.IndexOfKey("red"));
    }

    [Fact]
    public void IndexOfKey_IsCaseInsensitive_LikeWinForms()
    {
        using var list = new ImageList();
        list.Images.Add("Red", Swatch(Color.Red));

        Assert.Equal(0, list.Images.IndexOfKey("red"));
        Assert.Equal(0, list.Images.IndexOfKey("RED"));
    }

    [Fact]
    public void IndexOfKey_ReturnsMinusOneForAnUnknownKey()
    {
        using var list = new ImageList();
        list.Images.Add("red", Swatch(Color.Red));

        Assert.Equal(-1, list.Images.IndexOfKey("green"));
        Assert.False(list.Images.ContainsKey("green"), "an unknown key should not be reported present");
    }

    [Fact]
    public void Keys_ReportOneEntryPerImage_InOrder()
    {
        using var list = new ImageList();
        list.Images.Add("a", Swatch(Color.Red));
        list.Images.Add("b", Swatch(Color.Lime));

        Assert.Equal(2, list.Images.Keys.Count);
        Assert.Equal("a", list.Images.Keys[0]);
        Assert.Equal("b", list.Images.Keys[1]);
    }

    [Fact]
    public void SetKeyName_RetargetsTheKeyLookup()
    {
        using var list = new ImageList();
        list.Images.Add("old", Swatch(Color.Red));

        list.Images.SetKeyName(0, "new");

        Assert.Equal(0, list.Images.IndexOfKey("new"));
        Assert.Equal(-1, list.Images.IndexOfKey("old"));
    }

    [Fact]
    public void AddRange_AppendsEveryImage()
    {
        using var list = new ImageList();

        list.Images.AddRange(new Image[] { Swatch(Color.Red), Swatch(Color.Lime), Swatch(Color.Blue) });

        Assert.Equal(3, list.Images.Count);
    }

    [Fact]
    public void RemoveAt_DropsThatImageAndShiftsTheRest()
    {
        using var list = new ImageList();
        list.Images.Add("a", Swatch(Color.Red));
        list.Images.Add("b", Swatch(Color.Lime));
        list.Images.Add("c", Swatch(Color.Blue));

        list.Images.RemoveAt(1);

        Assert.Equal(2, list.Images.Count);
        Assert.Equal(0, list.Images.IndexOfKey("a"));
        Assert.Equal(1, list.Images.IndexOfKey("c"));
        Assert.Equal(-1, list.Images.IndexOfKey("b"));
    }

    [Fact]
    public void RemoveByKey_DropsTheKeyedImage()
    {
        using var list = new ImageList();
        list.Images.Add("a", Swatch(Color.Red));
        list.Images.Add("b", Swatch(Color.Lime));

        list.Images.RemoveByKey("a");

        Assert.Single(list.Images);
        Assert.False(list.Images.ContainsKey("a"), "the removed key should be gone");
        Assert.True(list.Images.ContainsKey("b"), "the other key should survive");
    }

    [Fact]
    public void Clear_EmptiesTheCollection()
    {
        using var list = new ImageList();
        list.Images.Add("a", Swatch(Color.Red));
        list.Images.Add("b", Swatch(Color.Lime));

        list.Images.Clear();

        Assert.Empty(list.Images);
        Assert.True(list.Images.Empty, "a cleared image list should be empty");
        Assert.False(list.Images.ContainsKey("a"), "keys should be cleared along with the images");
    }

    [Fact]
    public void Enumerator_YieldsEveryImage()
    {
        using var list = new ImageList();
        list.Images.Add(Swatch(Color.Red));
        list.Images.Add(Swatch(Color.Lime));

        var seen = 0;
        foreach (var _ in list.Images)
        {
            seen++;
        }

        Assert.Equal(2, seen);
    }

    [Fact]
    public void GetBitmap_ByIndexAndByName_ReturnTheSameImage()
    {
        using var list = new ImageList();
        list.Images.Add("only", Swatch(Color.Red));

        var byIndex = list.GetBitmap(0);
        var byName = list.GetBitmap("only");

        Assert.NotNull(byIndex);
        Assert.NotNull(byName);
        Assert.Equal(byIndex!.Width, byName!.Width);
    }

    [Fact]
    public void GetBitmap_ForAnUnknownName_ThrowsNamingTheMissingKey()
    {
        using var list = new ImageList();
        list.Images.Add("only", Swatch(Color.Red));

        // Documented behaviour of this shim: an unknown key is a load error, not a silent null.
        var ex = Assert.Throws<FileNotFoundException>(() => list.GetBitmap("missing"));
        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public void Tag_RoundTrips()
    {
        using var list = new ImageList { Tag = "payload" };

        Assert.Equal("payload", list.Tag);
    }

    [Fact]
    public void TransparentColor_RoundTrips()
    {
        using var list = new ImageList { TransparentColor = Color.Magenta };

        Assert.Equal(Color.Magenta, list.TransparentColor);
    }

    [Fact]
    public void ColorDepth_RoundTrips()
    {
        using var list = new ImageList { ColorDepth = ColorDepth.Depth32Bit };

        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
    }

    [Fact]
    public void Indexer_ThrowsForAnOutOfRangeIndex()
    {
        using var list = new ImageList();
        list.Images.Add(Swatch(Color.Red));

        Assert.ThrowsAny<ArgumentException>(() => _ = list.Images[5]);
    }

    [Fact]
    public void UnknownKeyIndexer_ReturnsNull()
    {
        using var list = new ImageList();
        list.Images.Add("a", Swatch(Color.Red));

        Assert.Null(list.Images["missing"]);
    }
}
