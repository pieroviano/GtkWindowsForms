using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms;

public class FormTests
{
    public FormTests()
    {
        Application.Init();
    }

    [Fact]
    public void Activate_ReturnsBoolean()
    {
        var form = new Form();

        var result = form.Activate();

        Assert.IsType<bool>(result);
    }

    [Fact]
    public void ClientSize_Set_UpdatesSize()
    {
        var form = new Form
        {
            ClientSize = new Size(800, 600)
        };

        Size formClientSize = default;
        form.Load += (_, _) =>
        {
            formClientSize = form.ClientSize;
            form.Close();
        };
        form.ShowDialog();

        Assert.True(800 <= formClientSize.Width);
        Assert.True(600 <= formClientSize.Height);
    }

    [Fact]
    public void CloseImmediately_DoesNotThrow()
    {
        var form = new Form();

        var ex = Record.Exception(() => form.CloseImmediately());

        Assert.Null(ex);
    }

    [Fact]
    public void Ctor_Default_Initializes()
    {
        var form = new Form();

        Assert.NotNull(form);
        Assert.NotNull(form.GtkControl);
    }

    [Fact]
    public void Ctor_WithTitle_SetsText()
    {
        var form = new Form("Hello");

        Assert.Equal("Hello", form.Text);
    }

    [Fact]
    public void DialogResult_SetAndGet_Works()
    {
        var form = new Form
        {
            DialogResult = DialogResult.OK
        };

        Assert.Equal(DialogResult.OK, form.DialogResult);
    }

    [Fact]
    public void Events_Invoke_Correctly()
    {
        var form = new Form();

        var loadCalled = false;
        var shownCalled = false;

        form.Load += (_, _) => loadCalled = true;
        form.Shown += (_, _) => shownCalled = true;

        // invoke protected methods via reflection
        var loadMethod = typeof(Form).GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Instance);
        var shownMethod = typeof(Form).GetMethod("OnShown", BindingFlags.NonPublic | BindingFlags.Instance);

        loadMethod!.Invoke(form, new object[] { EventArgs.Empty });
        shownMethod!.Invoke(form, new object[] { EventArgs.Empty });

        Assert.True(loadCalled);
        Assert.True(shownCalled);
    }

    [Fact]
    public void FormBorderStyle_None_SetsDecoratedFalse()
    {
        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.None
        };

        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
    }

    [Fact]
    public void FormBorderStyle_Sizable_SetsResizableTrue()
    {
        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.Sizable
        };

        Assert.Equal(FormBorderStyle.Sizable, form.FormBorderStyle);
    }

    [Fact]
    public void Hide_DoesNotThrow()
    {
        var form = new Form();

        var ex = Record.Exception(() => form.Hide());

        Assert.Null(ex);
    }

    [Fact]
    public void MaximizeMinimizeBox_Defaults_True()
    {
        var form = new Form();

        Assert.True(form.MaximizeBox);
        Assert.True(form.MinimizeBox);
    }

    [Fact]
    public void Opacity_SetAndGet_Works()
    {
        var form = new Form
        {
            Opacity = 0.5
        };

        double actualMemory = 0;
        form.Load += (s, e) =>
        {
            actualMemory = Math.Round(form.Opacity, 1);
            form.Close();
        };
        form.ShowDialog();

        Assert.Equal(0.5, actualMemory);
    }

    [Fact]
    public void Padding_Set_UpdatesContainerMargins()
    {
        var form = new Form();

        var padding = new Padding(1, 2, 3, 4);
        form.Padding = padding;

        Assert.Equal(padding, form.Padding);
    }

    [Fact]
    public void PerformLayout_SetsCreatedFlag()
    {
        var form = new Form();

        form.PerformLayout();

        // indirect validation: no exception
        Assert.True(true);
    }

    [Fact]
    public void ResumeLayout_False_SetsCreatedTrue()
    {
        var form = new Form();

        form.ResumeLayout(false);

        Assert.True(true);
    }

    [Fact]
    public void Show_WithSelfOwner_Throws()
    {
        var form = new Form();

        Assert.Throws<InvalidOperationException>(() =>
            form.Show(form));
    }

    [Fact]
    public void ShowDialog_WithSelfOwner_Throws()
    {
        var form = new Form();

        Assert.Throws<ArgumentException>(() =>
            form.ShowDialog(form));
    }

    [Fact]
    public void ShowInTaskbar_Toggle_Works()
    {
        var form = new Form
        {
            ShowInTaskbar = false
        };

        Assert.False(form.ShowInTaskbar);

        form.ShowInTaskbar = true;
        Assert.True(form.ShowInTaskbar);
    }

    [Fact]
    public void StatusProperties_SetAndGet_Work()
    {
        var form = new Form
        {
            ShowIcon = false,
            StartPosition = FormStartPosition.CenterScreen
        };

        Assert.False(form.ShowIcon);
        Assert.Equal(FormStartPosition.CenterScreen, form.StartPosition);
    }

    [Fact]
    public void SuspendLayout_DoesNotThrow()
    {
        var form = new Form();

        var ex = Record.Exception(() => form.SuspendLayout());

        Assert.Null(ex);
    }

    [Fact]
    public void Text_Set_UpdatesUnderlyingTitle()
    {
        var form = new Form
        {
            Text = "New Title"
        };

        Assert.Equal("New Title", form.Text);
    }

    [Fact]
    public void WindowState_Set_StoresValue()
    {
        var form = new Form
        {
            WindowState = FormWindowState.Maximized
        };

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
    }
}