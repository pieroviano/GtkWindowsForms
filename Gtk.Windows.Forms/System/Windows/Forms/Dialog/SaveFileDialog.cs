/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Prompts the user to select a location for saving a file. This class cannot be inherited.
/// </summary>
public class SaveFileDialog : FileDialog
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.SaveFileDialog" /> class.
    /// </summary>
    public SaveFileDialog()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether check write access.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if check write access; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool CheckWriteAccess
    {
        get => true;
        set { }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog asks for confirmation before an existing file is
    /// overwritten.
    /// </summary>
    /// <remarks>
    /// GTK's save chooser asks by itself, so this carries the caller's choice for code that reads it back.
    /// </remarks>
    [DefaultValue(true)]
    public bool OverwritePrompt { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the dialog asks for confirmation before a file that does not exist
    /// is created.
    /// </summary>
    [DefaultValue(false)]
    public bool CreatePrompt { get; set; }

    private new string? Description => base.Description;

    /// <summary>
    /// Opens the file.
    /// </summary>
    /// <returns>
    /// Stream.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// filename
    /// </exception>
    public Stream OpenFile()
    {
        var filename = FileName;
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException("filename");
        }

        return new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
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
        ActionType = Gtk.FileChooserAction.Save;

        return base.ShowDialog(owner);
    }
}