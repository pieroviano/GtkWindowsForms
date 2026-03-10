namespace System.Windows.Forms;
/// <summary>
/// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public class MessageBox
{
    internal static IMessageBox Instance { get; set; } = new NonStaticMessageBox();

    public static IMessageBox ReplaceInstance(int timeout = -1)
    {
        Instance = timeout <= 0 ? new NonStaticMessageBox() : AutoClosingMessageBox.Instance;
        if (timeout > 0)
        {
            AutoClosingMessageBox.Instance.MessageBoxTimeout = timeout;
        }

        return Instance;
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
        object param)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator,
        object param)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, buttons, MessageBoxIcon.None);
    }

    public static DialogResult Show(string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    public static DialogResult Show(string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, buttons, MessageBoxIcon.None);
    }

    public static DialogResult Show(IWin32Window? owner, string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    public static DialogResult Show(IWin32Window? owner, string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

}
