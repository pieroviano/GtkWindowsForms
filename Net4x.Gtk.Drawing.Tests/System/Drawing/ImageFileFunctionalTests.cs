extern alias gtk;
using Image = gtk::System.Drawing.Image;
using Bitmap = gtk::System.Drawing.Bitmap;
using ImageFormat = gtk::System.Drawing.Imaging.ImageFormat;

namespace System.Drawing;

/// <summary>File-based image behaviour: saving to disk produces a readable file, and loading infers the
/// format from the file extension.</summary>
[Collection("NonParallelCollection")]
public class ImageFileFunctionalTests
{
    private static Bitmap Sample(int w = 12, int h = 8)
    {
        var bmp = new Bitmap(w, h);
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                bmp.SetPixel(x, y, x < w / 2 ? Color.FromArgb(255, 210, 20, 20) : Color.FromArgb(255, 20, 20, 210));
            }
        }

        return bmp;
    }

    private static string TempPath(string extension) =>
        Path.Combine(Path.GetTempPath(), "imgfn-" + Guid.NewGuid().ToString("N") + extension);

    [Theory]
    [InlineData(".png")]
    [InlineData(".bmp")]
    [InlineData(".jpeg")]
    public void SaveToFile_ThenFromFile_PreservesDimensions(string extension)
    {
        var path = TempPath(extension);
        try
        {
            using (var source = Sample())
            {
                source.Save(path);
            }

            Assert.True(File.Exists(path), $"Save should have written {path}");
            Assert.True(new FileInfo(path).Length > 0, "the written file should not be empty");

            using var reloaded = Image.FromFile(path);
            Assert.Equal(12, reloaded.Width);
            Assert.Equal(8, reloaded.Height);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Theory]
    [InlineData(".png", "Png")]
    [InlineData(".bmp", "Bmp")]
    [InlineData(".jpeg", "Jpeg")]
    [InlineData(".tiff", "Tiff")]
    public void FromFile_InfersRawFormatFromTheExtension(string extension, string expectedFormat)
    {
        // Write PNG content under the target extension: FromFile derives RawFormat from the file name.
        var path = TempPath(extension);
        try
        {
            using (var source = Sample())
            using (var ms = new MemoryStream())
            {
                source.Save(ms, ImageFormat.Png);
                File.WriteAllBytes(path, ms.ToArray());
            }

            using var loaded = Image.FromFile(path);

            var expected = (ImageFormat)typeof(ImageFormat)
                .GetProperty(expectedFormat, Reflection.BindingFlags.Public | Reflection.BindingFlags.Static)!
                .GetValue(null)!;

            Assert.Equal(expected.Guid, loaded.RawFormat.Guid);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void FromFile_MissingFile_ThrowsFileNotFound()
    {
        var missing = TempPath(".png");
        Assert.Throws<FileNotFoundException>(() => Image.FromFile(missing));
    }

    [Fact]
    public void SaveToFile_PreservesPixelContent()
    {
        var path = TempPath(".png");
        try
        {
            using (var source = Sample(16, 8))
            {
                source.Save(path);
            }

            using var reloaded = (Bitmap)Image.FromFile(path);
            var left = reloaded.GetPixel(2, 4);
            var right = reloaded.GetPixel(13, 4);

            Assert.True(left.R > left.B, $"left half should still be red but was {left}");
            Assert.True(right.B > right.R, $"right half should still be blue but was {right}");
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
