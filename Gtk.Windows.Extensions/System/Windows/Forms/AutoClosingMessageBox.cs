using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// An IMessageBox implementation that wraps a platform message box and automatically disposes it after a timeout.
/// This class delegates actual presentation to a non-static MessageBox implementation and starts a background
/// task to auto-close/cleanup the underlying GTK <see cref="Gtk.Dialog" /> after
/// <see cref="MessageBoxTimeout" /> milliseconds.
/// </summary>
public class AutoClosingMessageBox : IMessageBox
{
    /// <summary>
    /// The underlying IMessageBox implementation used to show dialogs. Set by the constructor.
    /// </summary>
    internal static IMessageBox? InnerMessageBox;

    /// <summary>
    /// The currently visible GTK dialog instance (if any).
    /// </summary>
    private Dialog? _dialog;

    [ThreadStatic]
    private static AutoClosingMessageBox? instance;

    // IMessageBox implementation ------------------------------------------------

    /// <summary>
    /// Raised when the underlying dialog becomes available. Subscribers can inspect or modify the dialog.
    /// </summary>
    public virtual event EventHandler<DialogEventArgs>? DialogAvailable;

    private AutoClosingMessageBox()
    {
        instance = this;
        InnerMessageBox = new NonStaticMessageBox();
        Instance.Initialize();
        InnerMessageBox.DialogAvailable += FormsMessageBox_DialogAvailable;
    }

    /// <summary>
    /// Static constructor to perform one-time Windows Forms / GTK initialization required for message boxes.
    /// </summary>
    static AutoClosingMessageBox()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }

    /// <summary>
    /// Gets the thread-local singleton instance of <see cref="AutoClosingMessageBox" />.
    /// Accessing this property creates the instance on first use in the current thread.
    /// </summary>
    public static AutoClosingMessageBox Instance
    {
        get { return instance ??= new AutoClosingMessageBox(); }
    }

    /// <summary>
    /// Gets or sets the timeout in milliseconds after which the shown message box (GTK dialog) will be disposed.
    /// Default value is configured via <see cref="Init(int)" />.
    /// </summary>
    public int MessageBoxTimeout
    {
        get;
        set;
    }

    /// <summary>
    /// Initialize the auto-closing behavior and optionally set the default timeout interval.
    /// This is a convenience to ensure the singleton and timeout are configured.
    /// </summary>
    /// <param name="interval">
    /// Timeout in milliseconds for auto-disposal. Default is 5000 ms.
    /// </param>
    public static void Init(int interval = 5000)
    {
        Instance.Initialize(interval);
    }

    /// <summary>
    /// Installs this auto-closing implementation behind the static
    /// <see cref="MessageBox" /> helpers, so that every subsequent
    /// <c>MessageBox.Show(...)</c> auto-closes after <paramref name="interval" /> milliseconds.
    /// </summary>
    /// <param name="interval">
    /// Timeout in milliseconds for auto-disposal. Default is 5000 ms.
    /// </param>
    /// <returns>
    /// The installed <see cref="AutoClosingMessageBox" /> instance.
    /// </returns>
    public static AutoClosingMessageBox Install(int interval = 5000)
    {
        Init(interval);
        MessageBox.ReplaceInstance(Instance);
        return Instance;
    }

    /// <summary>
    /// Shows a message box and ensures it will be auto-closed after the configured timeout.
    /// This overload forwards all parameters to the underlying message box implementation.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options, displayHelpButton));
    }

    /// <summary>
    /// Shows a message box with a help file path and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath));
    }

    /// <summary>
    /// Shows a message box owned by a specific window and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath));
    }

    /// <summary>
    /// Shows a message box with keyword help and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword));
    }

    /// <summary>
    /// Shows an owner-owned message box with keyword help and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword));
    }

    /// <summary>
    /// Shows a message box that opens help by navigator and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator));
    }

    /// <summary>
    /// Shows an owner-owned message box that opens help by navigator and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator));
    }

    /// <summary>
    /// Shows a message box with help navigator and parameter and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param));
    }

    /// <summary>
    /// Shows an owner-owned message box with help navigator and parameter and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param));
    }

    /// <summary>
    /// Shows a message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton, options));
    }

    /// <summary>
    /// Shows a message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon, defaultButton));
    }

    /// <summary>
    /// Shows a message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons, icon));
    }

    /// <summary>
    /// Shows a message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption, buttons));
    }

    /// <summary>
    /// Shows a message box with text and caption and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text, string caption)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text, caption));
    }

    /// <summary>
    /// Shows a message box with specified text and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(string? text)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(text));
    }

    /// <summary>
    /// Shows an owner-owned message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options));
    }

    /// <summary>
    /// Shows an owner-owned message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons, icon, defaultButton));
    }

    /// <summary>
    /// Shows an owner-owned message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return InvokeAndAutoClose(() =>
        {
            return InnerMessageBox!.Show(owner, text, caption, buttons, icon);
        });
    }

    /// <summary>
    /// Shows an owner-owned message box and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption, buttons));
    }

    /// <summary>
    /// Shows an owner-owned message box with text and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text, string caption)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text, caption));
    }

    /// <summary>
    /// Shows an owner-owned message box with text and auto-closes after the configured timeout.
    /// </summary>
    public DialogResult Show(IWin32Window? owner, string? text)
    {
        return InvokeAndAutoClose(() => InnerMessageBox!.Show(owner, text));
    }

    /// <summary>
    /// Raises the DialogAvailable event.
    /// </summary>
    protected virtual void OnDialogAvailable(DialogEventArgs e)
    {
        DialogAvailable?.Invoke(this, e);
    }

    /// <summary>
    /// Runs an asynchronous task that waits for the configured timeout then disposes the current GTK dialog if still present.
    /// </summary>
    private void AutoDisposeAfterTimeout()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(MessageBoxTimeout));
            if (_dialog != null)
            {
                try
                {
                    _dialog.PangoContext?.Dispose();
                }
                catch (Exception ex)
                {
                    Diagnostics.GtkLogger.Debug($"Unexpected exception in AutoDisposeAfterTimeout", ex);
                }
                try
                {
                    _dialog.Dispose();
                }
                catch (Exception ex)
                {
                    Diagnostics.GtkLogger.Debug($"Unexpected exception in AutoDisposeAfterTimeout", ex);
                }
                GC.SuppressFinalize(_dialog);
                _dialog = null;
            }
        });
    }

    /// <summary>
    /// Handles the DialogAvailable event from the underlying message box implementation, stores the GTK
    /// dialog instance, and forwards the notification to this instance's own subscribers.
    /// </summary>
    /// <param name="sender">Event sender.
    /// </param>
    /// <param name="e">Dialog event data containing the GTK dialog.
    /// </param>
    private void FormsMessageBox_DialogAvailable(object? sender, DialogEventArgs e)
    {
        _dialog = e.Dialog;
        OnDialogAvailable(e);
    }

    /// <summary>
    /// Internal initializer for the instance; sets the default timeout.
    /// </summary>
    /// <param name="interval">Timeout interval in milliseconds.
    /// </param>
    private void Initialize(int interval = 5000)
    {
        MessageBoxTimeout = interval;
    }

    /// <summary>
    /// Invokes the supplied show function and arranges for automatic disposal of the dialog after the timeout.
    /// </summary>
    /// <param name="invoke">A function that shows the dialog and returns the DialogResult.
    /// </param>
    /// <returns>The dialog result returned by the underlying message box.
    /// </returns>
    private DialogResult InvokeAndAutoClose(Func<DialogResult> invoke)
    {
        AutoDisposeAfterTimeout();

        return invoke();
    }
}
