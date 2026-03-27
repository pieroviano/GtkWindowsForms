namespace System.Windows.Forms;
/// <summary>
/// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
/// </summary>
public class MessageBox
{
    /// <summary>
    /// The active
    /// <see cref="IMessageBox" /> implementation used by the static
    /// <see cref="MessageBox" /> helpers.
    /// By default this is a
    /// <see cref="NonStaticMessageBox" />; callers may replace it using
    /// <see cref="ReplaceInstance(int)" />.
    /// </summary>
    internal static IMessageBox? Instance
    {
        get;
        set;
    } = new NonStaticMessageBox();

    /// <summary>
    /// Replaces the current
    /// <see cref="T:System.Windows.Forms.IMessageBox" /> implementation used by the static API.
    /// </summary>
    /// <param name="timeout">
    /// When greater than zero, switch to the
    /// <see cref="T:System.Windows.Forms.AutoClosingMessageBox" /> instance and set its
    /// <see cref="T:System.Windows.Forms.AutoClosingMessageBox.MessageBoxTimeout" />; when less than or equal to zero, use a
    /// <see cref="T:System.Windows.Forms.NonStaticMessageBox" />.
    /// </param>
    /// <returns>
    /// The newly installed
    /// <see cref="T:System.Windows.Forms.IMessageBox" /> instance.
    /// </returns>
    public static IMessageBox ReplaceInstance(int timeout = -1)
    {
        Instance = timeout <= 0 ? new NonStaticMessageBox() : AutoClosingMessageBox.Instance;
        if (timeout > 0)
        {
            AutoClosingMessageBox.Instance.MessageBoxTimeout = timeout;
        }

        return Instance;
    }

    /// <summary>
    /// Displays a message box with the specified text, caption, buttons and icon.
    /// Other parameters are accepted for API compatibility but are ignored by the current implementations.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="displayHelpButton">
    /// if set to
    /// <c>
    /// true
    /// </c>display help button.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="keyword">
    /// The keyword.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="keyword">
    /// The keyword.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="navigator">
    /// The navigator.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="navigator">
    /// The navigator.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="navigator">
    /// The navigator.
    /// </param>
    /// <param name="param">
    /// The parameter.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <param name="helpFilePath">
    /// The help file path.
    /// </param>
    /// <param name="navigator">
    /// The navigator.
    /// </param>
    /// <param name="param">
    /// The parameter.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <param name="options">
    /// The options.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="defaultButton">
    /// The default button.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <param name="buttons">
    /// The buttons.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="caption">
    /// The caption.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
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
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public static DialogResult Show(IWin32Window? owner, string? text)
    {
        if (Instance is NonStaticMessageBox nonStaticMessageBox)
        {
            return MessageBoxOld.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        return Instance.Show(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
    }
}