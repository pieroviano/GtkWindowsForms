/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace System.Windows.Forms;

/// <summary>
/// Prompts the user to select a folder. This class cannot be inherited.
/// </summary>
public sealed class FolderBrowserDialog : FileDialog
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.FolderBrowserDialog" /> class.
    /// </summary>
    public FolderBrowserDialog()
    {

    }

    /// <summary>
    /// Gets or sets the root folder.
    /// </summary>
    /// <value>
    /// The root folder.
    /// </value>
    public Environment.SpecialFolder RootFolder
    {
        get;
        set;
    } = Environment.SpecialFolder.Desktop;

    /// <summary>
    /// Gets or sets the selected path.
    /// </summary>
    /// <value>
    /// The selected path.
    /// </value>
    public string? SelectedPath
    {
        get => SelectedDirectory;
        set => SelectedDirectory = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether selected path needs check.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if selected path needs check; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool SelectedPathNeedsCheck
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the selected paths.
    /// </summary>
    /// <value>
    /// The selected paths.
    /// </value>
    public string[]? SelectedPaths => base.FileNames;

    /// <summary>
    /// Gets or sets a value indicating whether show new folder button.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show new folder button; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowNewFolderButton
    {
        get;
        set;
    }

    private new bool Multiselect => base.Multiselect;

    private new string? Title => base.Title;

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public override void Reset()
    {
        RootFolder = Environment.SpecialFolder.Desktop;
        Description = string.Empty;
        SelectedPath = string.Empty;
        SelectedPathNeedsCheck = false;
        ShowNewFolderButton = true;
        base.Reset();
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public override DialogResult ShowDialog(IWin32Window? owner)
    {
        ActionType = Gtk.FileChooserAction.SelectFolder;

        return base.ShowDialog(owner);
    }
}