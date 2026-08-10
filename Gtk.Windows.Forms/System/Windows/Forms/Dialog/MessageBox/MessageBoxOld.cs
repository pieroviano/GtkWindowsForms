/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using GtkSystem.Windows.Forms.Resources;

namespace System.Windows.Forms;

/// <summary>
/// Class MessageBoxOld.
/// </summary>
public class MessageBoxOld
{

    private static Gtk.Window? ActiveWindow = null; // has caching significance

    /// <summary>
    /// Displays a message box with specified text, caption, and style with Help Button.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style and Help file Path .
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and keyword.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and keyword for a IWin32Window.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and HelpNavigator.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path and HelpNavigator for IWin32Window.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object for a IWin32Window.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style with Help Button.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
    {
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text and caption.
    /// </summary>
    public static DialogResult Show(string text, string caption)
    {
        return ShowCore(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text.
    /// </summary>
    public static DialogResult Show(string text)
    {
        return ShowCore(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon,
                                    MessageBoxDefaultButton defaultButton)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
    {
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text and caption.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text, string caption)
    {
        return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    /// Displays a message box with specified text.
    /// </summary>
    public static DialogResult Show(IWin32Window owner, string text)
    {
        return ShowCore(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    private static void Dia_Response(object o, Gtk.ResponseArgs args)
    {
        if (o is Dialog dia)
        {
            dia.PangoContext.Dispose();
            dia.Dispose();
            dia.Destroy();
        }
    }

    private static DialogResult ShowCore(IWin32Window? owner, string text, string caption,
                                 MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
                                 MessageBoxOptions options)
    {
        var result = DialogResult.None;
        try
        {

            result = ShowCore(owner, text, caption, buttons, icon, defaultButton, options, true);
        }
        finally
        {

        }

        return result;
    }

    private static DialogResult ShowCore(IWin32Window? owner, string? text, string caption,
                                         MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
                                         MessageBoxOptions options, bool showHelp)
    {
        var irun = 0;
        if (owner is System.Windows.Forms.Form control)
        {
            irun = ShowCore((Gtk.Window?)control.Widget, 0, text, caption, buttons, icon);
        }
        else
        {
            var window = Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive);
            if (window != null)
            {
                ActiveWindow = window;
                irun = ShowCore(window, 0, text, caption, buttons, icon);
            }
            else if (ActiveWindow != null)
            {
                irun = ShowCore(ActiveWindow, 0, text, caption, buttons, icon);
            }
            else
            {
                irun = ShowCore(null, 0, text, caption, buttons, icon);
            }
        }

        var resp = (Gtk.ResponseType)Enum.Parse(typeof(Gtk.ResponseType), irun.ToString());
        if (resp == Gtk.ResponseType.Yes)
            return DialogResult.Yes;
        else if (resp == Gtk.ResponseType.No)
            return DialogResult.No;
        else if (resp == Gtk.ResponseType.Ok)
            return DialogResult.OK;
        else if (resp == Gtk.ResponseType.Cancel)
            return DialogResult.Cancel;
        else if (resp == Gtk.ResponseType.Reject)
            return DialogResult.Abort;
        else if (resp == Gtk.ResponseType.Help)
            return DialogResult.Retry;
        else if (resp == Gtk.ResponseType.Close)
            return DialogResult.Ignore;
        else if (resp == Gtk.ResponseType.None)
            return DialogResult.None;
        else if (resp == Gtk.ResponseType.DeleteEvent)
            return DialogResult.None;
        else
            return DialogResult.None;
    }

    private static int ShowCore(Window? owner, int position, string? text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, params object[] args)
    {
        var dia = new Gtk.Dialog(caption, owner, Gtk.DialogFlags.DestroyWithParent)
        {
            // Gtk4: Window.KeepBelow and Gdk.WindowTypeHint were removed (WM-managed).
        };
        dia.StyleContext.AddClass("DefaultThemeStyle");
        dia.StyleContext.AddClass("MessageBox");
        dia.SetSizeRequest(300, 100);
        dia.SetDefaultSize(300, 100);
        dia.Response += Dia_Response;
        dia.ContentArea.Spacing = 10;
        dia.ContentArea.Hexpand = false;
        dia.ContentArea.Vexpand = false;
        dia.ContentArea.Halign = Align.Fill;
        dia.ContentArea.Valign = Align.Start;
        dia.Valign = Align.Fill;
        dia.Halign = Align.Fill;
        var msgbox = new Box(Gtk.Orientation.Horizontal, 10)
        {
            Valign = Align.Start,
            Halign = Align.Fill
        };

        if (icon == MessageBoxIcon.Question)
            msgbox.Append(new Gtk.Image(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-question.png"))));
        else if (icon == MessageBoxIcon.Warning || icon == MessageBoxIcon.Exclamation)
            msgbox.Append(new Gtk.Image(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-warning.png"))));
        else if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Asterisk)
            msgbox.Append(new Gtk.Image(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-information.png"))));
        else if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand)
            msgbox.Append(new Gtk.Image(new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("dialog-error.png"))));
        var content = new Gtk.Label(text)
        {
            MarginEnd = 30, Halign = Align.Fill,
            Valign = Align.Start
        };
        var maxwidth = 300;
        // Gtk4: Gdk.Display.NMonitors/GetMonitor/GetMonitorAtWindow removed; Monitor has only Geometry.
        if (Gdk.Display.Default.Monitors.NItems > 0)
        {
            var rectangle = ((Gdk.Monitor)Gdk.Display.Default.Monitors.GetObject(0)).Geometry;
            if (owner != null && owner.Surface is Gdk.Surface ownerSurface &&
                Gdk.Display.Default.GetMonitorAtSurface(ownerSurface) is Gdk.Monitor monitor)
                rectangle = monitor.Geometry;
            maxwidth = rectangle.Width / 2;
        }
        var pag = content.CreatePangoLayout(text);
        pag.GetPixelSize(out var width, out var height);
        if (width > maxwidth)
        {
            dia.SetSizeRequest(maxwidth, 100);
            dia.SetDefaultSize(maxwidth, 100);
            content.Wrap = true;
            content.Wrap = true;
            content.LineWrapMode = Pango.WrapMode.Word;
        }
        msgbox.Append(content);
        dia.ContentArea.Append(msgbox);

        // Gtk4-removed: dia.Icon = new Gdk.Pixbuf(AssemblyResources.CurrentAssembly, AssemblyResources.ToSystemUri("help-faq.png"));
        if (buttons == MessageBoxButtons.OK)
        {
            dia.AddButton(System.Properties.Resources.Gtk_FileDialog_RunDialog_OK, Gtk.ResponseType.Ok);
        }
        else if (buttons == MessageBoxButtons.OKCancel)
        {
            dia.AddButton(System.Properties.Resources.Gtk_FileDialog_RunDialog_OK, Gtk.ResponseType.Ok);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Cancel, Gtk.ResponseType.Cancel);
        }
        else if (buttons == MessageBoxButtons.YesNo)
        {
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Yes, Gtk.ResponseType.Yes);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_No, Gtk.ResponseType.No);
        }
        else if (buttons == MessageBoxButtons.YesNoCancel)
        {
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Yes, Gtk.ResponseType.Yes);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_No, Gtk.ResponseType.No);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Cancel, Gtk.ResponseType.Cancel);
        }
        else if (buttons == MessageBoxButtons.AbortRetryIgnore)
        {
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Reject, Gtk.ResponseType.Reject);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Help, Gtk.ResponseType.Help);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Close, Gtk.ResponseType.Close);
        }
        else if (buttons == MessageBoxButtons.RetryCancel)
        {
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Help, Gtk.ResponseType.Help);
            dia.AddButton(System.Properties.Resources.Gtk_MessageBox_ShowCore_Cancel, Gtk.ResponseType.Cancel);
        }
        dia.Show();

        return dia.RunDialog();
    }

    private static int ShowMessageDialogCore(Gtk.Window owner, int position, string text, string caption, MessageBoxButtons buttons, params object[] icon)
    {
        var buttonsType = Gtk.ButtonsType.Close;
        if (buttons == MessageBoxButtons.OK)
            buttonsType = Gtk.ButtonsType.Ok;
        else if (buttons == MessageBoxButtons.OKCancel)
            buttonsType = Gtk.ButtonsType.OkCancel;
        else if (buttons == MessageBoxButtons.YesNo)
            buttonsType = Gtk.ButtonsType.YesNo;
        else if (buttons == MessageBoxButtons.YesNoCancel)
            buttonsType = Gtk.ButtonsType.YesNo;
        else if (buttons == MessageBoxButtons.AbortRetryIgnore)
            buttonsType = Gtk.ButtonsType.OkCancel;
        else if (buttons == MessageBoxButtons.RetryCancel)
            buttonsType = Gtk.ButtonsType.OkCancel;

        var dia = new Gtk.MessageDialog(owner, Gtk.DialogFlags.DestroyWithParent, Gtk.MessageType.Info, buttonsType, text);
        dia.StyleContext.AddClass("DefaultThemeStyle");
        dia.StyleContext.AddClass("MessageBox");
        // Gtk4-removed: dia.KeepBelow = false;
        dia.Title = caption;
        dia.Response += Dia_Response;

        return dia.RunDialog();
    }
}