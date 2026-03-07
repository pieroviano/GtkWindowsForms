/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
namespace System.Windows.Forms
{
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

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows multiple files to be selected.
        /// </summary>
        /// <returns>true if the dialog box allows multiple files to be selected together or concurrently; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public new bool Multiselect { get => base.Multiselect; set => base.Multiselect = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the read-only check box is selected.
        /// </summary>
        /// <returns>true if the read-only check box is selected; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ReadOnlyChecked { get; set; }

        /// <summary>
        /// Gets the file name and extension for the file selected in the dialog box. The file name does not include the path.
        /// </summary>
        /// <returns>The file name and extension for the file selected in the dialog box. The file name does not include the path. The default value is an empty string.
        /// </returns>
        public string SafeFileName => Path.GetFileName(FileName) ?? string.Empty;

        /// <summary>
        /// Gets an array of file names and extensions for all the selected files in the dialog box. The file names do not include the path.
        /// </summary>
        /// <returns>An array of file names and extensions for all the selected files in the dialog box. The file names do not include the path. If no files are selected, an empty array is returned.
        /// </returns>
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

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box contains a read-only check box.
        /// </summary>
        /// <returns>true if the dialog box contains a read-only check box; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool ShowReadOnly { get; set; }

        private new string Description => base.Description;

        /// <summary>
        /// Opens the file selected by the user, with read-only permission. The file is specified by the
        /// <see cref="System.Windows.Forms.FileDialog.FileName" /> property.
        /// </summary>
        /// <returns>A
        /// <see cref="System.IO.Stream" /> that specifies the read-only file selected by the user.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The file name is null.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
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
}
