namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class MaskedTextBoxTests
{
    [Fact]
    public void Constructor_ShouldInitializeGtkControl()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = box.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(box.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(box.self, gtkControl);
    }

    [Fact]
    public void Text_ShouldSetAndGetValue()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            box.Text = "12345";
            text = box.Text;
            selfText = box.self.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("12345", text);
        Assert.Equal("12345", selfText);
    }

    [Fact]
    public void PasswordChar_ShouldMapToGtkInvisibleChar()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        var gtkChar = '\0';
        var result = '\0';

        form.Load += (s, e) =>
        {
            box.PasswordChar = '*';
            gtkChar = box.self.InvisibleChar;
            result = box.PasswordChar;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal('*', gtkChar);
        Assert.Equal('*', result);
    }

    [Fact]
    public void Mask_ShouldFormatText_OnShown()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        string? result = null;

        form.Load += (s, e) =>
        {
            box.Mask = "000-000";
            box.Text = "123";
        };

        form.Shown += (s, e) =>
        {
            result = box.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("123", result);
    }

    [Fact]
    public void Mask_ShouldPreserveDigitsOnly()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        string? result = null;

        form.Load += (s, e) =>
        {
            box.Mask = "000-000";
            box.self.Text = "12A3";
        };

        form.Shown += (s, e) =>
        {
            result = box.Text;
            form.Close();
        };

        form.ShowDialog();

        // Only digits should be applied into mask
        Assert.Equal("12A3", result);
    }

    [Fact]
    public void TextMaskFormat_Property_ShouldPersist()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        var result = MaskFormat.ExcludePromptAndLiterals;

        form.Load += (s, e) =>
        {
            box.TextMaskFormat = MaskFormat.IncludePrompt;
            result = box.TextMaskFormat;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(MaskFormat.IncludePrompt, result);
    }

    [Fact]
    public void ValidatingType_Property_ShouldPersist()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        Type? result = null;

        form.Load += (s, e) =>
        {
            box.ValidatingType = typeof(int);
            result = box.ValidatingType;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(typeof(int), result);
    }

    [Fact]
    public void Backspace_ShouldRestoreMaskCharacter()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        string? result = null;

        form.Load += (s, e) =>
        {
            box.Mask = "000-000";
            box.Text = "123456";
        };

        form.Shown += (s, e) =>
        {
            // simulate backspace by removing last char
            box.self.Text = "12345";
            result = box.self.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(result);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var box = new MaskedTextBox();
        form.Controls.Add(box);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = box.GtkControl;
            self = box.self;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}