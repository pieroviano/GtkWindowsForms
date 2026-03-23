/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
namespace System.Windows.Forms;

/// <summary>
/// Prompts the user to open a file. This class cannot be inherited.
/// </summary>
public sealed class OpenFileDialog : FileDialog
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.OpenFileDialog" /> class.
    /// </summary>
    public OpenFileDialog()
    {

    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Windows.Forms.OpenFileDialog" /> is multiselect.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if multiselect; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public new bool Multiselect
    {
        get => base.Multiselect;
        set => base.Multiselect = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether read only checked.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if read only checked; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ReadOnlyChecked
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the name of the safe file.
    /// </summary>
    /// <value>
    /// The name of the safe file.
    /// </value>
    public string SafeFileName => Path.GetFileName(FileName) ?? string.Empty;

    /// <summary>
    /// Gets the safe file names.
    /// </summary>
    /// <value>
    /// The safe file names.
    /// </value>
    public string[] SafeFileNames
    {
        get
        {
            string[] fullPaths = FileNames;
            if (fullPaths is null || fullPaths.Length == 0)
            {
                return Array.Empty<string>();
            }

            var safePaths = new string[fullPaths.Length];
            for (int i = 0; i < safePaths.Length; ++i)
            {
                safePaths[i] = Path.GetFileName(fullPaths[i]);
            }

            return safePaths;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether show read only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show read only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ShowReadOnly
    {
        get;
        set;
    }

    private new string Description => base.Description;

    /// <summary>
    /// Opens the file.
    /// </summary>
    /// <returns>
    /// Stream.
    /// </returns>
    public Stream OpenFile()
    {
        if (System.IO.File.Exists(base.FileName))
            return System.IO.File.OpenRead(base.FileName);
        else
            return null;

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
    public override DialogResult ShowDialog(IWin32Window owner)
    {
        ActionType = Gtk.FileChooserAction.Open;

        return base.ShowDialog(owner);
    }
}