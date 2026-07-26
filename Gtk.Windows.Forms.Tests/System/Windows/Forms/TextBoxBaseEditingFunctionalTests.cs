namespace System.Windows.Forms;

extern alias gtk;

/// <summary>
/// Verifies the shared editing surface introduced on <see cref="TextBoxBase" /> — the clipboard, undo and
/// modified-state behaviour restored to <see cref="TextBox" /> and <see cref="RichTextBox" /> — plus the
/// control-hierarchy relationships that source ported from real WinForms depends on.
/// </summary>
/// <remarks>
/// Behaviour is asserted inside one realized-form lifecycle. The clipboard operations
/// (<see cref="TextBoxBase.Copy" />/<see cref="TextBoxBase.Cut" />/<see cref="TextBoxBase.Paste" />) are not
/// exercised here: taking GTK clipboard (X selection) ownership, or reading it back with <c>WaitForText</c>,
/// destabilizes this shared single-threaded GTK test host and crashes a later test. Their substantive edit
/// logic — the shared <c>ReplaceSelectedTextCore</c> that inserts/removes text — is instead covered through
/// the <see cref="TextBoxBase.SelectedText" />, <see cref="TextBoxBase.AppendText(System.String)" /> and
/// <see cref="TextBoxBase.Undo" /> assertions; only the thin <c>Gtk.Clipboard</c> bridge is left unasserted.
/// </remarks>
[Collection("NonParallelCollection")]
public class TextBoxBaseEditingFunctionalTests
{
    [Fact]
    public void TextControls_DeriveFromTextBoxBase()
    {
        // Verified by type metadata rather than by instantiating GTK widgets: this is a compile/hierarchy
        // claim ("control is TextBoxBase") and must hold without a realized form.
        Assert.True(typeof(TextBoxBase).IsAssignableFrom(typeof(TextBox)));
        Assert.True(typeof(TextBoxBase).IsAssignableFrom(typeof(RichTextBox)));
        Assert.True(typeof(TextBoxBase).IsAssignableFrom(typeof(MaskedTextBox)));
        // TextBoxBase itself sits directly on Control, as in real WinForms.
        Assert.Equal(typeof(Control), typeof(TextBoxBase).BaseType);
    }

    [Fact]
    public void EditingSurface_Behaves()
    {
        var form = new Form();
        var textBox = new TextBox();
        var richTextBox = new RichTextBox();
        form.Controls.Add(textBox);
        form.Controls.Add(richTextBox);

        var modifiedChangedRaised = 0;

        form.Load += (s, e) =>
        {
            try
            {
                // --- assigning Text clears Modified; editing sets it ---
                textBox.ModifiedChanged += (s2, e2) => modifiedChangedRaised++;
                textBox.Text = "Hello";
                Assert.False(textBox.Modified);          // programmatic assignment does not modify
                Assert.False(textBox.CanUndo);           // nothing captured after a plain assignment

                textBox.AppendText(" World");
                Assert.Equal("Hello World", textBox.Text);
                Assert.True(textBox.Modified);           // edit sets Modified
                Assert.True(textBox.CanUndo);
                Assert.Equal(1, modifiedChangedRaised);  // one false→true transition

                // --- TextLength ---
                Assert.Equal("Hello World".Length, textBox.TextLength);

                // --- Undo restores the pre-append text ---
                textBox.Undo();
                Assert.Equal("Hello", textBox.Text);

                // --- SelectedText replaces the selection ---
                textBox.Text = "Hello World";
                textBox.Select(0, 5);
                textBox.SelectedText = "Goodbye";
                Assert.Equal("Goodbye World", textBox.Text);

                // --- Clear empties the control ---
                textBox.Clear();
                Assert.Equal(string.Empty, textBox.Text);

                // --- RichTextBox shares the same base behaviour ---
                richTextBox.Text = "Hello";
                richTextBox.AppendText(" World");
                Assert.Equal("Hello World", richTextBox.Text);
                richTextBox.Undo();
                Assert.Equal("Hello", richTextBox.Text);
            }
            finally
            {
                form.Close();
            }
        };

        form.ShowDialog();
        form.Dispose();
    }
}
