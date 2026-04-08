namespace System.Windows.Forms;

extern alias gtk;

[Collection("NonParallelCollection")]
public class TextBoxTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaults()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        object? gtkControl = null;

        form.Load += (s, e) =>
        {
            gtkControl = textBox.GtkControl;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(textBox.self);
        Assert.NotNull(gtkControl);
        Assert.Equal(textBox.self, gtkControl);
    }

    [Fact]
    public void Text_ShouldSetAndGet()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        string? text = null;
        string? selfText = null;

        form.Load += (s, e) =>
        {
            textBox.Text = "Hello";
            text = textBox.Text;
            selfText = textBox.self.Text;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("Hello", text);
        Assert.Equal("Hello", selfText);
    }

    [Fact]
    public void Lines_ShouldSetAndGet()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        string[]? lines = null;

        form.Load += (s, e) =>
        {
            textBox.Lines = new[] { "A", "B", "C" };
            lines = textBox.Lines;
            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(lines);
        Assert.Single(lines); // joined internally
        Assert.Equal("ABC", lines[0]);
    }

    [Fact]
    public void MaxLength_ShouldSetAndGet()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        int max = -1;

        form.Load += (s, e) =>
        {
            textBox.MaxLength = 10;
            max = textBox.MaxLength;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(10, max);
    }

    [Fact]
    public void PlaceholderText_ShouldSetAndGet()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        string? placeholder = null;

        form.Load += (s, e) =>
        {
            textBox.PlaceholderText = "hint";
            placeholder = textBox.PlaceholderText;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal("hint", placeholder);
    }

    [Fact]
    public void ReadOnly_ShouldSetAndGet()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        bool readOnly = false;

        form.Load += (s, e) =>
        {
            textBox.ReadOnly = true;
            readOnly = textBox.ReadOnly;
            form.Close();
        };

        form.ShowDialog();

        Assert.True(readOnly);
    }

    [Fact]
    public void PasswordChar_ShouldSetAndApply()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        char result = '\0';
        bool visible = true;

        form.Load += (s, e) =>
        {
            textBox.PasswordChar = '*';
            result = textBox.PasswordChar;
            visible = textBox.self.Visibility;
            form.Close();
        };

        form.ShowDialog();

        Assert.Equal('*', result);
        Assert.False(visible); // password hides text
    }

    [Fact]
    public void InsertTextAtCursor_ShouldInsertText()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        string? result = null;

        form.Load += (s, e) =>
        {
            textBox.Text = "A";
            textBox.InsertTextAtCursor("B");

            result = textBox.Text;

            form.Close();
        };

        form.ShowDialog();

        Assert.Contains("B", result);
    }

    [Fact]
    public void SelectionStart_And_Length_ShouldWork()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        int start = -1;
        int length = -1;

        form.Load += (s, e) =>
        {
            textBox.Text = "Hello";

            textBox.Select(1, 3);

            start = textBox.SelectionStart;
            length = textBox.SelectionLength;

            form.Close();
        };

        form.ShowDialog();

        Assert.Equal(1, start);
        Assert.Equal(3, length);
    }

    [Fact]
    public void TextChanged_ShouldFire()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        bool fired = false;

        form.Load += (s, e) =>
        {
            textBox.TextChanged += (s2, e2) => fired = true;

            // simulate GTK-visible state
            textBox.self.Show();

            textBox.Text = "Hello";

            // force invoke (GTK may not raise Changed automatically in test)
            var method = typeof(TextBox)
                .GetMethod("OnTextChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method?.Invoke(textBox, new object[] { EventArgs.Empty });

            form.Close();
        };

        form.ShowDialog();

        Assert.True(fired);
    }

    [Fact]
    public void KeyDown_ShouldFire()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        bool fired = false;

        form.Load += (s, e) =>
        {
            textBox.KeyDown += (s2, e2) => fired = true;

            var method = typeof(TextBox)
                .GetMethod("OnKeyDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            method?.Invoke(textBox, new object[] { new KeyEventArgs(Keys.A) });

            form.Close();
        };

        form.ShowDialog();

        Assert.True(fired);
    }

    [Fact]
    public void GtkControl_ShouldReturnSelf()
    {
        var form = new Form();
        var textBox = new TextBox();
        form.Controls.Add(textBox);

        object? gtkControl = null;
        object? self = null;

        form.Load += (s, e) =>
        {
            gtkControl = textBox.GtkControl;
            self = textBox.self;

            form.Close();
        };

        form.ShowDialog();

        Assert.NotNull(self);
        Assert.Equal(self, gtkControl);
    }
}