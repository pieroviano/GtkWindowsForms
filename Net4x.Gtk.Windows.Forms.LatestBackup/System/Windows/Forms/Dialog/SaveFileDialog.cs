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
/// <filterpriority>
/// 2
/// </filterpriority>
public sealed class SaveFileDialog : FileDialog
{
    public SaveFileDialog()
    {
    }

    public bool CheckWriteAccess
    {
        get => true;
        set { }
    }

    private new string Description => base.Description;

    public Stream OpenFile()
    {
        string filename = FileName;
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException("filename");
        }

        return new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
    }

    public override DialogResult ShowDialog(IWin32Window owner)
    {
        ActionType = Gtk.FileChooserAction.Save;

        return base.ShowDialog(owner);
    }

}
