using Gtk;
using System.IO;

namespace System.Windows.Forms;

using FormsMessageBoxButtons = MessageBoxButtons;

public class AutoClosingMessageBox : IMessageBox
{
    internal static IMessageBox? MessageBox;

    private Dialog? _dialog;

    [ThreadStatic]
    private static AutoClosingMessageBox? instance;

    // IMessageBox implementation ------------------------------------------------

    public event EventHandler<DialogEventArgs>? DialogAvailable;

    private AutoClosingMessageBox()
    {
        instance = this;
        MessageBox = new NonStaticMessageBox();
        Instance.Initialize();
        MessageBox.DialogAvailable += FormsMessageBox_DialogAvailable;
    }

    static AutoClosingMessageBox()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }

    public static AutoClosingMessageBox Instance
    {
        get { return instance ??= new AutoClosingMessageBox(); }
    }

    public int MessageBoxTimeout { get; set; }

    public static void Init(int interval = 5000)
    {
        Instance.Initialize(interval);
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options, displayHelpButton));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, navigator, param));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton, options));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon, defaultButton));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons, icon));
    }

    public DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption, buttons));
    }

    public DialogResult Show(string? text, string caption)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text, caption));
    }

    public DialogResult Show(string? text)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(text));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton, options));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons, icon, defaultButton));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return InvokeAndAutoClose(() =>
        {
            return MessageBox!.Show(owner, text, caption, buttons, icon);
        });
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption, buttons));
    }

    public DialogResult Show(IWin32Window? owner, string? text, string caption)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text, caption));
    }

    public DialogResult Show(IWin32Window? owner, string? text)
    {
        return InvokeAndAutoClose(() => MessageBox!.Show(owner, text));
    }

    private void AutoDisposeAfterTimeout()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(System.TimeSpan.FromMilliseconds(MessageBoxTimeout));
            if (_dialog != null)
            {
                try
                {
                    _dialog.PangoContext?.Dispose();
                }
                catch { }
                try
                {
                    _dialog.Dispose();
                }
                catch { }
                GC.SuppressFinalize(_dialog);
                _dialog = null;
            }
        });
    }

    private void FormsMessageBox_DialogAvailable(object? sender, DialogEventArgs e)
    {
        _dialog = e.Dialog;
    }

    private void Initialize(int interval = 5000)
    {
        MessageBoxTimeout = interval;
    }

    private DialogResult InvokeAndAutoClose(Func<DialogResult> invoke)
    {
        AutoDisposeAfterTimeout();

        return invoke();
    }

}
