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
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.FolderBrowserDialog" /> class.
        /// </summary>
        public FolderBrowserDialog()
        {

        }
        /// <summary>
        /// Resets properties to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets or sets the root folder where the browsing starts from.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the
        /// <see cref="System.Environment.SpecialFolder" /> values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public Environment.SpecialFolder RootFolder { get; set; } = Environment.SpecialFolder.Desktop;
        /// <summary>
        /// Gets or sets the path selected by the user.
        /// </summary>
        /// <returns>The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an empty string ("").
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public string SelectedPath
        {
            get => SelectedDirectory;
            set => SelectedDirectory = value;
        }

        public string[] SelectedPaths => base.FileNames;

        private new bool Multiselect => base.Multiselect;

        private new string Title => base.Title;
        /// <summary>
        /// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box.
        /// </summary>
        /// <returns>true if the New Folder button is shown in the dialog box; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ShowNewFolderButton { get; set; }

        public bool SelectedPathNeedsCheck { get; set; }
        public override DialogResult ShowDialog(IWin32Window owner)
        {
            ActionType = Gtk.FileChooserAction.SelectFolder;
            return base.ShowDialog(owner);
        }
    }
}
