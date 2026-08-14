extern alias gtk;
using System.Windows.Forms;
using gtk::System.Diagnostics;

namespace Mocks;

extern alias gtk;

public class MockMessageBox : IMessageBox, IDisposable
{
    // Per-instance, not static: MessageBox.Instance is process-global, so a static "previous" field is
    // overwritten by a second mock and restores the wrong implementation (or the mock itself) on dispose.
    private IMessageBox? _oldInstance;

    /// <summary>
    ///     Stores all log messages captured by the custom GtkLogger action.
    /// </summary>
    public List<string> LoggedMessages { get; } = [];

    public void Dispose()
    {
        gtk::System.Diagnostics.GtkLogger.SetCustomLogAction(null);
        MessageBox.Instance = _oldInstance;
        GC.SuppressFinalize(this);
    }

#pragma warning disable CS0067 // The event is never used
    public event EventHandler<DialogEventArgs>? DialogAvailable;
#pragma warning restore CS0067 // The event is never used

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
        object param)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
        object param)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text, string caption)
        => LogAndReturn(text, caption);

    public DialogResult Show(string? text)
        => LogAndReturn(text, "");

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text, string caption)
        => LogAndReturn(text, caption);

    public DialogResult Show(IWin32Window? owner, string? text)
        => LogAndReturn(text, "");

    /// <summary>
    ///     Replaces MessageBox.Instance with a new MockMessageBox for testing and captures GtkLogger.Info output.
    ///     The caller must dispose the returned mock (<c>using</c>): it installs process-global state that every
    ///     later test in the assembly would otherwise inherit.
    /// </summary>
    public static MockMessageBox ReplaceMessageBoxWithMock()
    {
        GtkLogger.IsEnabled = true;
        var mockMessageBox = new MockMessageBox();
        mockMessageBox.LoggedMessages.Clear();
        gtk::System.Diagnostics.GtkLogger.SetCustomLogAction((message, level, exception) =>
        {
            var formatted = $"[{level}] {message}" + (exception != null ? $" Exception: {exception}" : "");
            mockMessageBox.LoggedMessages.Add(formatted);
        });
        mockMessageBox._oldInstance = MessageBox.Instance;
        MessageBox.Instance = mockMessageBox;
        return mockMessageBox;
    }

    private DialogResult LogAndReturn(string? text, string caption)
    {
        gtk::System.Diagnostics.GtkLogger.Info($"MessageBox.Show called. Caption: '{caption}', Text: '{text}'");
        return DialogResult.OK;
    }

    ~MockMessageBox() => Dispose();
}