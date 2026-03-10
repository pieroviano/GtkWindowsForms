/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

namespace System.Windows.Forms
{
    /// <summary>
    /// Prompts the user to select a folder. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public sealed class FolderBrowserDialog : FileDialog
    {
        public FolderBrowserDialog()
        {

        }

        public Environment.SpecialFolder RootFolder { get; set; } = Environment.SpecialFolder.Desktop;

        public string SelectedPath
        {
            get => SelectedDirectory;
            set => SelectedDirectory = value;
        }

        public bool SelectedPathNeedsCheck { get; set; }

        public string[] SelectedPaths => base.FileNames;

        public bool ShowNewFolderButton { get; set; }

        private new bool Multiselect => base.Multiselect;

        private new string Title => base.Title;

        public override void Reset()
        {
            RootFolder = Environment.SpecialFolder.Desktop;
            Description = string.Empty;
            SelectedPath = string.Empty;
            SelectedPathNeedsCheck = false;
            ShowNewFolderButton = true;
            base.Reset();
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            ActionType = Gtk.FileChooserAction.SelectFolder;

            return base.ShowDialog(owner);
        }

    }
}
