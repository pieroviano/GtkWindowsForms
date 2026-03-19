namespace System.Windows.Forms;
/// <summary>
/// Interface IMessageBox
/// </summary>
public interface IMessageBox
{
    /// <summary>
    /// Occurs when dialog available.
    /// </summary>
    event EventHandler<DialogEventArgs>? DialogAvailable;

    /// <summary>
    /// Displays a message box with specified text, caption, and style with Help Button.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton);

    /// <summary>
    /// Displays a message box with specified text, caption, style and Help file Path .
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath);

    /// <summary>
    /// Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and keyword.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and keyword for a IWin32Window.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and HelpNavigator.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and HelpNavigator for IWin32Window.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param);

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object for a IWin32Window.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(string? text, string caption, MessageBoxButtons buttons);

    /// <summary>
    /// Displays a message box with specified text and caption.
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
    DialogResult Show(string? text, string caption);

    /// <summary>
    /// Displays a message box with specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
    DialogResult Show(string? text);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton, MessageBoxOptions options);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons);

    /// <summary>
    /// Displays a message box with specified text and caption.
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
    DialogResult Show(IWin32Window? owner, string? text, string caption);

    /// <summary>
    /// Displays a message box with specified text.
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
    DialogResult Show(IWin32Window? owner, string? text);

}