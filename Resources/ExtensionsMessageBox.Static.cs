namespace System.Windows.Forms;
/// <summary>
/// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public class MessageBox
{
    /// <summary>
    /// The active <see cref="IMessageBox"/> implementation used by the static <see cref="MessageBox"/> helpers.
    /// By default this is a <see cref="NonStaticMessageBox"/>; callers may replace it using <see cref="ReplaceInstance(IMessageBox)"/>.
    /// </summary>
    internal static IMessageBox Instance { get; set; } = new NonStaticMessageBox();

    /// <summary>
    /// Replaces the current <see cref="IMessageBox"/> implementation used by the static API.
    /// </summary>
    /// <param name="instance">
    /// The implementation to install, or <see langword="null"/> to restore the default <see cref="NonStaticMessageBox"/>.
    /// </param>
    /// <returns>The newly installed <see cref="IMessageBox"/> instance.</returns>
    public static IMessageBox ReplaceInstance(IMessageBox? instance = null)
    {
        return Instance = instance ?? new NonStaticMessageBox();
    }

    /// <summary>
    /// Replaces the current <see cref="IMessageBox"/> implementation used by the static API.
    /// </summary>
    /// <param name="timeout">
    /// Ignored except for its sign: any value installs a <see cref="NonStaticMessageBox"/>.
    /// The auto-closing implementation moved to the <c>Gtk.Windows.Extensions</c> package;
    /// call <c>AutoClosingMessageBox.Install(timeout)</c> there instead.
    /// </param>
    /// <returns>The newly installed <see cref="IMessageBox"/> instance.</returns>
    [Obsolete("The auto-closing implementation moved to the Gtk.Windows.Extensions package. " +
              "Call AutoClosingMessageBox.Install(timeout) to auto-close, or ReplaceInstance() for the default message box.")]
    public static IMessageBox ReplaceInstance(int timeout)
    {
        return ReplaceInstance(null);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons and icon.
    /// Other parameters are accepted for API compatibility but are ignored by the current implementations.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon and help file path.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons and icon.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon and help keyword.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons, icon and help keyword.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon and help navigator.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons, icon and help navigator.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons, icon, help navigator and parameter.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons, icon, help navigator and parameter.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons and icon.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons and default button.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text, caption and buttons.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, icon);
        }

        return Instance.Show(null, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified text and buttons.
    /// </summary>
    public static DialogResult Show(string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, buttons, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with the specified text and caption.
    /// </summary>
    public static DialogResult Show(string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with the specified text.
    /// </summary>
    public static DialogResult Show(string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons and icon.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons and default button.
    /// </summary>
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

    /// <summary>
    /// Displays a message box with the specified owner, text, caption, buttons and icon.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons,
        MessageBoxIcon icon)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, icon);
        }

        return Instance.Show(owner, text, caption, buttons, icon);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text, caption and buttons.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, buttons, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, buttons, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with the specified owner, text and caption.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

    /// <summary>
    /// Displays a message box with the specified owner and text.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }

}
