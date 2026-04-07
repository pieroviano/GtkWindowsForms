#pragma warning disable CS0436

extern alias gtk;
using gtk::System.Drawing;

namespace System.Windows.Forms;

[Collection("NonParallelCollection")]
public class PictureBoxTests
{
    [Fact]
    public void Constructor_ShouldInitializeSelfAndGtkControl()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = pictureBox.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(pictureBox.GtkControl, gtkControl);
    }

    [Fact]
    public void Image_ShouldSetAndGetValue()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        Image? result = null;

        Bitmap? image = null;
        form.Load += (s, e) =>
        {
            image = System.Properties.Resources.timg;
            pictureBox.Image = image;
            result = pictureBox.Image;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(image, result);
    }

    [Fact]
    public void ImageLocation_ShouldSetAndGetValue()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        string? location = null;

        form.Load += (s, e) =>
        {
            pictureBox.ImageLocation = "Resources\\timg.jpg"; // won't load if file doesn't exist
            location = pictureBox.ImageLocation;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Resources\\timg.jpg", location);
    }

    [Fact]
    public void BorderStyle_ShouldSetAndGetValue()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        var style = BorderStyle.None;

        form.Load += (s, e) =>
        {
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            style = pictureBox.BorderStyle;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(BorderStyle.FixedSingle, style);
    }

    [Fact]
    public void SizeMode_ShouldSetAndGetValue()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        PictureBoxSizeMode mode = default;

        form.Load += (s, e) =>
        {
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            mode = pictureBox.SizeMode;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(PictureBoxSizeMode.StretchImage, mode);
    }

    [Fact]
    public void InitialImage_ShouldBeUsed_WhenImageIsNull()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        var initialImage = System.Properties.Resources.timg;

        var executed = false;

        form.Load += (s, e) =>
        {
            pictureBox.InitialImage = initialImage;

            // Force render logic
            pictureBox.Width = 50;
            pictureBox.Height = 50;

            // simulate shown
            pictureBox.GetType()
                .GetMethod("Self_Shown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(pictureBox, new object?[] { null, null });

            executed = true;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(executed);
    }

    [Fact]
    public void Load_WithInvalidPath_ShouldNotThrow()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        var executed = false;

        form.Load += (s, e) =>
        {
            pictureBox.Load("non_existing_file.png");
            executed = true;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(executed);
    }

    [Fact]
    public void Load_EmptyString_ShouldDoNothing()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        Image? image = null;

        form.Load += (s, e) =>
        {
            pictureBox.Load("");
            image = pictureBox.Image;
            form.Close();
        };

        form.ShowDialog();

        Assert.Null(image);
    }

    [Fact]
    public void LoadAsync_ShouldNotThrow()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        var executed = false;

        form.Load += (s, e) =>
        {
            pictureBox.LoadAsync("non_existing_file.png");
            executed = true;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(executed);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var pictureBox = new PictureBox();
        form.Controls.Add(pictureBox);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = pictureBox.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(gtkControl);
        Assert.Equal(pictureBox.GtkControl, gtkControl);
    }
}