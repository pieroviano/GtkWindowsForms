/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a common dialog box that displays available colors along with controls that enable the user to define custom colors.
/// </summary>
public class ColorDialog : CommonDialog
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ColorDialog" /> class.
    /// </summary>
    public ColorDialog() : base()
    {

    }

    /// <summary>
    /// Gets or sets a value indicating whether allow full open.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow full open; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public virtual bool AllowFullOpen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether any color.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if any color; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool AnyColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color Color
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the custom colors.
    /// </summary>
    /// <value>
    /// The custom colors.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int[] CustomColors
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether full open.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if full open; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool FullOpen
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show help.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show help; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool ShowHelp
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether solid color only.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if solid color only; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public virtual bool SolidColorOnly
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    protected virtual IntPtr Instance { get; }

    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <value>
    /// The options.
    /// </value>
    protected virtual int Options { get; }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public override void Reset()
    {
        Color = Color.Black;
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() { return this.Color.Name; }

    /// <summary>
    /// Runs the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    protected override bool RunDialog(IWin32Window? owner)
    {
        Gtk.ColorChooserDialog colorChooserDialog = null;
        if (owner != null && owner is Form ownerform)
        {
            colorChooserDialog = new Gtk.ColorChooserDialog(System.Properties.Resources.Gtk_ColorDialog_RunDialog_Choose_color, ownerform.self)
            {
                WindowPosition = Gtk.WindowPosition.CenterOnParent
            };
        }
        else
        {
            colorChooserDialog = new Gtk.ColorChooserDialog(System.Properties.Resources.Gtk_ColorDialog_RunDialog_Choose_color, null)
            {
                WindowPosition = Gtk.WindowPosition.Center
            };
        }
        colorChooserDialog.IconName = "image-x-generic";
        colorChooserDialog.KeepAbove = true;
        if (Color.Name != "0")
            colorChooserDialog.Rgba = new Gdk.RGBA() { Alpha = (double)Color.A / 255, Red = (double)Color.R / 255, Green = (double)Color.G / 255, Blue = (double)Color.B / 255 };
        if (FullOpen && AllowFullOpen)
            colorChooserDialog.Fullscreen();
        var res = colorChooserDialog.Run();
        var colorSelection = colorChooserDialog.Rgba;
        this.Color = Color.FromArgb((int)(colorSelection.Alpha * 255), (int)Math.Round(colorSelection.Red * 255, 0), (int)Math.Round(colorSelection.Green * 255, 0), (int)Math.Round(colorSelection.Blue * 255, 0));
        colorChooserDialog.Dispose();
        colorChooserDialog.Destroy();

        return res == -5;
    }
}