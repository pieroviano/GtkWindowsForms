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
/// <filterpriority>
/// 2
/// </filterpriority>
public sealed class OpenFileDialog : FileDialog
{
    public OpenFileDialog()
    {

    }

    public new bool Multiselect { get => base.Multiselect; set => base.Multiselect = value; }

    public bool ReadOnlyChecked { get; set; }

    public string SafeFileName => Path.GetFileName(FileName) ?? string.Empty;

    public string[] SafeFileNames
    {
        get
        {
            string[] fullPaths = FileNames;
            if (fullPaths is null || fullPaths.Length == 0)
            {
                return Array.Empty<string>();
            }

            string[] safePaths = new string[fullPaths.Length];
            for (int i = 0; i < safePaths.Length; ++i)
            {
                safePaths[i] = Path.GetFileName(fullPaths[i]);
            }

            return safePaths;
        }
    }

    public bool ShowReadOnly { get; set; }

    private new string Description => base.Description;

    public Stream OpenFile()
    {
        if (System.IO.File.Exists(base.FileName))
            return System.IO.File.OpenRead(base.FileName);
        else
            return null;

    }

    public override DialogResult ShowDialog(IWin32Window owner)
    {
        ActionType = Gtk.FileChooserAction.Open;

        return base.ShowDialog(owner);
    }

}
