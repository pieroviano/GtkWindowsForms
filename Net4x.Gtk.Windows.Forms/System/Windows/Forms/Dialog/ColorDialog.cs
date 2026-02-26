/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a common dialog box that displays available colors along with controls that enable the user to define custom colors.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public class ColorDialog : CommonDialog
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ColorDialog" /> class.
        /// </summary>
        public ColorDialog() : base()
        {

        }
        /// <summary>
        /// Gets or sets a value indicating whether the user can use the dialog box to define custom colors.
        /// </summary>
        /// <returns>true if the user can define custom colors; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue(true)]
        public virtual bool AllowFullOpen { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays all available colors in the set of basic colors.
        /// </summary>
        /// <returns>true if the dialog box displays all available colors in the set of basic colors; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool AnyColor { get; set; }
        /// <summary>
        /// Gets or sets the color selected by the user.
        /// </summary>
        /// <returns>The color selected by the user. If a color is not selected, the default value is black.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color Color { get; set; }
        /// <summary>
        /// Gets or sets the set of custom colors shown in the dialog box.
        /// </summary>
        /// <returns>A set of custom colors shown by the dialog box. The default value is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] CustomColors { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the controls used to create custom colors are visible when the dialog box is opened
        /// </summary>
        /// <returns>true if the custom color controls are available when the dialog box is opened; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool FullOpen { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether a Help button appears in the color dialog box.
        /// </summary>
        /// <returns>true if the Help button is shown in the dialog box; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool ShowHelp { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box will restrict users to selecting solid colors only.
        /// </summary>
        /// <returns>true if users can select only solid colors; otherwise, false. The default value is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public virtual bool SolidColorOnly { get; set; }
        /// <summary>
        /// Gets the underlying window instance handle (HINSTANCE).
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that contains the HINSTANCE value of the window handle.
        /// </returns>
        protected virtual IntPtr Instance { get; }
        /// <summary>
        /// Gets values to initialize the
        /// <see cref="System.Windows.Forms.ColorDialog" />
        /// </summary>
        /// <returns>A bitwise combination of internal values that initializes the
        /// <see cref="System.Windows.Forms.ColorDialog" />
        /// </returns>
        protected virtual int Options { get; }
        /// <summary>
        /// Resets all options to their default values, the last selected color to black, and the custom colors to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public override void Reset()
        {
            Color = Color.Black;
        }
        protected override bool RunDialog(IWin32Window owner)
        {
            Gtk.ColorChooserDialog colorChooserDialog = null;
            if (owner != null && owner is Form ownerform)
            {
                colorChooserDialog = new Gtk.ColorChooserDialog(System.Properties.Resources.Gtk_ColorDialog_RunDialog_Choose_color, ownerform.self);
                colorChooserDialog.WindowPosition = Gtk.WindowPosition.CenterOnParent;
            }
            else
            {
                colorChooserDialog = new Gtk.ColorChooserDialog(System.Properties.Resources.Gtk_ColorDialog_RunDialog_Choose_color, null);
                colorChooserDialog.WindowPosition = Gtk.WindowPosition.Center;
            }
            colorChooserDialog.IconName = "image-x-generic";
            colorChooserDialog.KeepAbove = true;
            if (Color.Name != "0")
                colorChooserDialog.Rgba = new Gdk.RGBA() { Alpha = (double)Color.A / 255, Red = (double)Color.R / 255, Green = (double)Color.G / 255, Blue = (double)Color.B / 255 };
            if (FullOpen && AllowFullOpen)
                colorChooserDialog.Fullscreen();
            int res = colorChooserDialog.Run();
            Gdk.RGBA colorSelection = colorChooserDialog.Rgba;
            this.Color = Color.FromArgb((int)(colorSelection.Alpha * 255), (int)Math.Round(colorSelection.Red * 255, 0), (int)Math.Round(colorSelection.Green * 255, 0), (int)Math.Round(colorSelection.Blue * 255, 0));
            colorChooserDialog.Dispose();
            colorChooserDialog.Destroy();
            return res == -5;
        }
        /// <summary>
        /// Returns a string that represents the
        /// <see cref="System.Windows.Forms.ColorDialog" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that represents the current
        /// <see cref="System.Windows.Forms.ColorDialog" />
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override string ToString() { return this.Color.Name; }
    }
}
