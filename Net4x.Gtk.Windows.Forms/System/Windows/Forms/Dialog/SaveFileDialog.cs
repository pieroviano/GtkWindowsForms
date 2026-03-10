/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
namespace System.Windows.Forms;

/// <summary>
/// Prompts the user to select a location for saving a file. This class cannot be inherited.
/// </summary>
public sealed class SaveFileDialog : FileDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveFileDialog"/> class.
    /// </summary>
    public SaveFileDialog()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether [check write access].
    /// </summary>
    /// <value><c>true</c> if [check write access]; otherwise, <c>false</c>.</value>
    public bool CheckWriteAccess
    {
        get => true;
        set { }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    private new string Description => base.Description;

    /// <summary>
    /// Opens the file.
    /// </summary>
    /// <returns>Stream.</returns>
    /// <exception cref="System.ArgumentNullException">filename</exception>
    public Stream OpenFile()
    {
        string filename = FileName;
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException("filename");
        }

        return new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <returns>DialogResult.</returns>
    public override DialogResult ShowDialog(IWin32Window owner)
    {
        ActionType = Gtk.FileChooserAction.Save;

        return base.ShowDialog(owner);
    }

}
