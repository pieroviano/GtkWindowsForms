using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Provides static methods and properties to manage an application, such as methods to start and stop an application, to process Windows messages, and properties to get information about an application. This class cannot be inherited.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
public sealed partial class Application
{

    private static readonly object internalSyncObject = new();

    private static string? _excutablepath;

    private static string? _startuppath;

    static Application()
    {
        Init();
    }

    /// <summary>
    /// Gets the application.
    /// </summary>
    /// <value>
    /// The application.
    /// </value>
    public static Gtk.Application? App
    {
        get;
        private set;
    }

    public static bool DontQuit { get; set; }

    /// <summary>
    /// Occurs when the application is about to shut down, after the message loop has terminated.
    /// </summary>
    public static event EventHandler? ApplicationExit;

    /// <summary>
    /// Occurs when the message loop on the current thread is about to terminate.
    /// </summary>
    public static event EventHandler? ThreadExit;

    /// <summary>
    /// Occurs when an exception escapes to the message loop - the last line of defence a Windows Forms
    /// application has, and the one most of them install.
    /// </summary>
    /// <remarks>
    /// Neither this event nor <see cref="M:System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode)" />
    /// existed, so an application that installs a handler did not compile, and - worse - there was no
    /// policy at all: an exception thrown by a click handler unwound through GtkSharp's signal
    /// marshalling with undefined behaviour.
    /// </remarks>
    public static event Threading.ThreadExceptionEventHandler? ThreadException;

    /// <summary>
    /// How exceptions reaching the message loop are treated. See
    /// <see cref="M:System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode)" />.
    /// </summary>
    private static UnhandledExceptionMode unhandledExceptionMode = UnhandledExceptionMode.Automatic;

    /// <summary>Set once the GLib unhandled-exception hook has been installed.</summary>
    private static bool exceptionHookInstalled;

    /// <summary>
    /// What to do with an exception that reached the message loop.
    /// </summary>
    internal enum UnhandledExceptionDisposition
    {
        /// <summary>Hand it to <see cref="E:System.Windows.Forms.Application.ThreadException" />.</summary>
        RaiseThreadException,

        /// <summary>No handler is attached: report it and keep the application alive.</summary>
        ReportAndContinue,

        /// <summary>Let it propagate and terminate the process.</summary>
        Rethrow,
    }

    /// <summary>
    /// The policy itself, kept separate from the GLib plumbing so it can be reasoned about - and tested -
    /// without a message loop.
    /// </summary>
    /// <param name="mode">The configured mode.</param>
    /// <param name="hasHandler">Whether anything is subscribed to <see cref="E:System.Windows.Forms.Application.ThreadException" />.</param>
    internal static UnhandledExceptionDisposition Decide(UnhandledExceptionMode mode, bool hasHandler)
    {
        switch (mode)
        {
            case UnhandledExceptionMode.ThrowException:
                return UnhandledExceptionDisposition.Rethrow;

            case UnhandledExceptionMode.CatchException:
                // Catch even with nobody listening - that is what distinguishes this mode from Automatic.
                return hasHandler
                    ? UnhandledExceptionDisposition.RaiseThreadException
                    : UnhandledExceptionDisposition.ReportAndContinue;

            case UnhandledExceptionMode.Automatic:
            default:
                return hasHandler
                    ? UnhandledExceptionDisposition.RaiseThreadException
                    : UnhandledExceptionDisposition.Rethrow;
        }
    }

    /// <summary>
    /// Sets how unhandled exceptions reaching the message loop are treated.
    /// </summary>
    /// <param name="mode">The mode to apply.</param>
    /// <exception cref="T:System.InvalidOperationException">
    /// A message loop is already running on this thread. Windows Forms requires the mode to be chosen
    /// before <see cref="M:System.Windows.Forms.Application.Run" />, so that the policy cannot change
    /// underneath a running application.
    /// </exception>
    public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
    {
        SetUnhandledExceptionMode(mode, true);
    }

    /// <summary>
    /// Sets how unhandled exceptions reaching the message loop are treated.
    /// </summary>
    /// <param name="mode">The mode to apply.</param>
    /// <param name="threadScope">
    /// Accepted for source compatibility. This implementation keeps one process-wide policy, because the
    /// GTK main loop it hooks is itself process-wide.
    /// </param>
    public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope)
    {
        if (mode != UnhandledExceptionMode.Automatic &&
            mode != UnhandledExceptionMode.ThrowException &&
            mode != UnhandledExceptionMode.CatchException)
        {
            throw new global::System.ComponentModel.InvalidEnumArgumentException(
                nameof(mode), (int)mode, typeof(UnhandledExceptionMode));
        }

        lock (internalSyncObject)
        {
            if (MessageLoop)
            {
                throw new InvalidOperationException(
                    "The unhandled-exception mode must be set before the message loop starts.");
            }

            unhandledExceptionMode = mode;
        }
    }

    /// <summary>
    /// Raises <see cref="E:System.Windows.Forms.Application.ThreadException" /> for
    /// <paramref name="t" />, applying the configured
    /// <see cref="T:System.Windows.Forms.UnhandledExceptionMode" />.
    /// </summary>
    /// <param name="t">The exception that reached the message loop.</param>
    /// <returns>
    /// <see langword="true" /> if the exception was handled and the application should continue;
    /// <see langword="false" /> if it must be allowed to propagate.
    /// </returns>
    public static bool OnThreadException(Exception t)
    {
        if (t == null)
        {
            throw new ArgumentNullException(nameof(t));
        }

        var handler = ThreadException;
        switch (Decide(unhandledExceptionMode, handler != null))
        {
            case UnhandledExceptionDisposition.RaiseThreadException:
                try
                {
                    handler!(null, new Threading.ThreadExceptionEventArgs(t));
                }
                catch (Exception failure)
                {
                    // A ThreadException handler that throws would otherwise recurse straight back here.
                    GtkLogger.Error("The ThreadException handler threw.", failure);

                    return false;
                }

                return true;

            case UnhandledExceptionDisposition.ReportAndContinue:
                GtkLogger.Error("Unhandled exception reached the message loop.", t);

                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Routes exceptions escaping GtkSharp's signal marshalling - which is how an exception thrown by a
    /// click handler leaves managed code - through <see cref="M:System.Windows.Forms.Application.OnThreadException(System.Exception)" />.
    /// </summary>
    private static void InstallUnhandledExceptionHook()
    {
        if (exceptionHookInstalled)
        {
            return;
        }

        exceptionHookInstalled = true;
        GLib.ExceptionManager.UnhandledException += args =>
        {
            var exception = args.ExceptionObject as Exception
                            ?? new Exception(args.ExceptionObject?.ToString() ?? "Unhandled exception.");

            // ExitApplication true means "let this terminate the process", which is exactly the
            // Rethrow disposition; handled exceptions leave the application running.
            args.ExitApplication = !OnThreadException(exception);
        };
    }

    /// <summary>
    /// Gets a value indicating whether a message loop is running on this thread.
    /// </summary>
    public static bool MessageLoop { get; private set; }

    /// <summary>
    /// The context driving the message loop currently running on this thread, if any.
    /// </summary>
    internal static ApplicationContext? CurrentContext { get; private set; }

    /// <summary>
    /// Raises <see cref="E:System.Windows.Forms.Application.ApplicationExit" />.
    /// </summary>
    private static void OnApplicationExit(EventArgs e) => ApplicationExit?.Invoke(null, e);

    /// <summary>
    /// Raises <see cref="E:System.Windows.Forms.Application.ThreadExit" />.
    /// </summary>
    private static void OnThreadExit(EventArgs e) => ThreadExit?.Invoke(null, e);

    /// <summary>
    /// Gets the common application data path.
    /// </summary>
    /// <value>
    /// The common application data path.
    /// </value>
    public static string CommonAppDataPath
    {
        get
        {
            var dir = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appDataDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }

    /// <summary>
    /// Gets or sets the current culture.
    /// </summary>
    /// <value>
    /// The current culture.
    /// </value>
    public static CultureInfo CurrentCulture
    {
        get
        {
            return Thread.CurrentThread.CurrentCulture;
        }
        set
        {
            Thread.CurrentThread.CurrentCulture = value;
        }
    }

    /// <summary>
    /// Gets or sets the current input language.
    /// </summary>
    /// <value>
    /// The current input language.
    /// </value>
    public static InputLanguage CurrentInputLanguage
    {
        get
        {
            return InputLanguage.CurrentInputLanguage;
        }
        set
        {
            InputLanguage.CurrentInputLanguage = value;
        }
    }

    /// <summary>
    /// Gets the executable path.
    /// </summary>
    /// <value>
    /// The executable path.
    /// </value>
    public static string ExecutablePath
    {
        get
        {
            if (_excutablepath == null)
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length > 0)
                {
                    _excutablepath = args[0];
                }
                else
                {
#if NET6_0_OR_GREATER
                    _excutablepath = Assembly.GetEntryAssembly()?.Location;
                    if (string.IsNullOrWhiteSpace(_excutablepath))
                        _excutablepath = Environment.ProcessPath;
#else
                    _excutablepath = Assembly.GetEntryAssembly()?.Location;
                    if (string.IsNullOrWhiteSpace(_excutablepath))
                        _excutablepath = Path.Combine(AppContext.BaseDirectory, Assembly.GetEntryAssembly()?.GetName().Name!, ".dll");
#endif
                }
            }

            return _excutablepath!;
        }
    }

    /// <summary>
    /// Gets the local user application data path.
    /// </summary>
    /// <value>
    /// The local user application data path.
    /// </value>
    public static string LocalUserAppDataPath
    {
        get
        {
            var dir = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appDataDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }

    /// <summary>
    /// Gets the open forms.
    /// </summary>
    /// <value>
    /// The open forms.
    /// </value>
    public static FormCollection OpenForms
    {
        get
        {
            var forms = new FormCollection();
            var windows = Gtk.Window.ListToplevels().Where(w => w.IsVisible == true);
            foreach (var w in windows)
                if (w.Data.ContainsKey("Control") && w.Data["Control"] is Form form)
                    forms.Add(form);

            return forms;
        }
    }

    /// <summary>
    /// Gets the startup path.
    /// </summary>
    /// <value>
    /// The startup path.
    /// </value>
    public static string StartupPath
    {
        get
        {
            if (_startuppath == null)
            {
                var appstart = AppContext.BaseDirectory;
                if (string.IsNullOrWhiteSpace(appstart))
                    appstart = Directory.GetCurrentDirectory();
                else
                    Directory.SetCurrentDirectory(appstart);
                _startuppath = appstart;
            }

            return _startuppath;
        }
    }

    /// <summary>
    /// Gets the user application data path.
    /// </summary>
    /// <value>
    /// The user application data path.
    /// </value>
    public static string UserAppDataPath
    {
        get
        {
            var dir = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }
    }

    private static string appDataDirectory
    {
        get
        {
            var assembly = Assembly.GetEntryAssembly().GetName();
            var assemblyFullName = assembly.FullName.Split(',');
            var _namespace = assemblyFullName[0];

            return Path.Combine(_namespace, assembly.Name, assembly.Version.ToString());
        }
    }

    /// <summary>
    /// Does the events.
    /// </summary>
    public static void DoEvents()
    {
        var context = GLib.MainContext.Default;
        while (context.HasPendingEvents)
            context.RunIteration(false);

        while (Gtk.Application.EventsPending())
            Gtk.Application.RunIteration(false);
    }

    /// <summary>
    /// Enables visual styles for the application.
    /// </summary>
    /// <remarks>
    /// GTK themes the widgets itself, so there is no per-process theming switch to flip; what this does
    /// record is the answer <see cref="P:System.Windows.Forms.Application.RenderWithVisualStyles" /> gives,
    /// which owner-drawing code branches on.
    /// </remarks>
    public static void EnableVisualStyles()
    {
        _useVisualStyles = true;
    }

    /// <summary>
    /// Exits this instance.
    /// </summary>
    public static void Exit()
    {
        ExitThread();
    }

    /// <summary>
    /// Exits this instance.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static void Exit(CancelEventArgs e)
    {
        lock (internalSyncObject)
        {
            try
            {
                if (e == null)
                {
                    Quit();
                }
                else
                {
                    if (e.Cancel == false)
                    {
                        Quit();
                    }
                }
            }
            finally
            {
                GtkLogger.Info("Application quitting");
            }
        }
    }

    /// <summary>
    /// Exits the thread.
    /// </summary>
    public static void ExitThread()
    {
        lock (internalSyncObject)
        {
            try
            {
                Quit();
            }
            finally
            {
                GtkLogger.Info("Application quitting");
            }
        }
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns>
    /// </returns>
    /// <summary>
    /// Rules that have to beat the theme's own, so they are loaded through a separate provider at
    /// <see cref="Gtk.StyleProviderPriority.User" /> rather than with the bulk stylesheet.
    /// </summary>
    /// <remarks>
    /// The menu-row hover rules live here because the theme's own <c>button:hover</c> would otherwise win.
    /// Note how the two halves are scoped: the background goes on the row's button, the foreground only on
    /// <c>&gt;box</c>, its caption row. A drop-down popover is parented to the button, so an inheritable
    /// colour on the button itself cascades into the submenu it opens and repaints every caption there in
    /// the selected foreground - white on white, which reads as an empty menu.
    /// </remarks>
    internal static string UserPriorityCss =>
        ".ToolStripButtonFlat,.ToolStripButtonFlat>button{border:none;border-width:0px;border-style:none;outline:none;background:none;background-image:none;box-shadow:none;border-radius:0px;padding:1px 4px;min-height:6px;min-width:6px;}\n"
        + ".ToolStripButtonFlat:hover,.ToolStripButtonFlat>button:hover{background-color:shade(@theme_bg_color,0.9);}\n"
        + ".ToolStripButtonFlat:active,.ToolStripButtonFlat:checked,.ToolStripButtonFlat>button:active,.ToolStripButtonFlat>button:checked{background-color:shade(@theme_bg_color,0.82);}\n"
        // Gtk3 drew the hovered menu row in the theme's selection colours, courtesy of GtkMenuItem. Gtk4 has
        // no menu-item widget, so a drop-down row was left with the theme's plain button hover and the menu
        // gave no feedback about which entry the pointer was on. Menu-bar entries keep the strip's own
        // hover, which is why the rules exclude them.
        + ".MenuItemButton:not(.MenuBarItem):hover{background-color:@theme_selected_bg_color;}\n"
        + ".MenuItemButton:not(.MenuBarItem):hover>box label{color:@theme_selected_fg_color;}\n"
        // A ToolStripTextBox is a bare GtkEntry, and Adwaita gives an entry a 34px minimum height plus
        // padding. A size request cannot pull it below that - a request is a minimum - so the entry ended up
        // 42px tall, taller than the strip it lives in, and being the tallest item it inflated the whole
        // toolstrip past its designed height and pushed the trailing items off the right edge. Collapsing
        // the theme's minimum lets the item's designed Size govern, as it does in WinForms.
        + ".ToolStripEntry,.ToolStripEntry>text{min-height:0;padding-top:0;padding-bottom:0;}\n"
        // A WinForms group box notches its caption out of the top border. Gtk3 drew that with
        // gtk_render_frame_gap, a primitive Gtk4 deleted, so GroupBoxBase.OnSnapshot draws the notched
        // border itself and the theme's own uninterrupted rectangle has to go. Making it transparent is not
        // enough, and the reason is worth recording: Gtk4 gives a widget's own drawing a context whose
        // origin is its CSS *content box* and clips it there. Adwaita's 1px frame border therefore shifted
        // the shim's border a pixel in and swallowed its right and bottom edges outright - measured on the
        // live window, the two remaining edges stopped exactly at the content box. Taking the border and
        // the padding to zero makes the content box the widget box, which is the only geometry in which a
        // border drawn at the widget's own edges survives. Nothing touches the background, which is how
        // GroupBox.BackColor reaches the widget.
        + ".GroupBox{border-style:none;border-width:0;border-radius:0;box-shadow:none;padding:0;}";

    public static Gtk.Application Init()
    {
        if (App == null)
        {
            var css_style = @"
@define-color frame_color  alpha(@theme_fg_color, 0.3);
@define-color frame3d_color  alpha(@theme_fg_color, 0.2);
@define-color frame3d_color5  alpha(@theme_fg_color, 0.4);
@define-color line_color #ECECEC;
@define-color separator_color1 #C6C5C4;
@define-color separator_color2 #D6D7D8;

.DefaultThemeStyle{padding: 0px; border-style:solid;min-height:2px;min-width:2px;}
.DefaultThemeStyle entry{
   padding: 4px 5px; border-width: 1px; border-style: solid; border-color:@frame_color;
   background-color: @theme_base_color; color: @theme_text_color;
}
.DefaultThemeStyle entry.flat{
   padding: 4px 5px; border-width: 1px; border-style: solid; border-color:@frame_color;
   background-color: @theme_base_color; color: @theme_text_color;
}
.DefaultThemeStyle button{padding:4px 3px;}
.titlebar{border-radius: 0px; border-bottom: solid 1px #cfcfcf;}
.ButtonNone{padding:0px;margin:0px;border-radius:0px;border-style:none;box-shadow:none;}
.ButtonNone.check,.ButtonNone check{padding-top:0px;padding-bottom:0px;margin-top:0px;margin-bottom:0px;}
.BorderNone{border-style:none;box-shadow:none;}
.BorderFixedSingle{border-width:0px;border-style:none;padding:1px;box-shadow: 0px 0px 0px 1px @frame_color;}
.BorderFixed3D{border-width:0px;border-style:none; padding:2px 1px 1px 2px;box-shadow: 0px 0px 0px 1px @frame3d_color, inset 2px 2px 0px 0px @frame3d_color5;}

.DataGridView {margin:0px;}
.GridViewCell-Button{ color:@theme_text_color; border:solid 1px @frame_color; background-color: shade(@theme_bg_color, 0.7);}
.GridViewCell-Button:hover{background-color: shade(@theme_bg_color, 0.8);}
.GridViewCell-Button:selected{ color:blue;}

.LinkLabel{border-style:none;}
.TextBox{}
.ComboBox{padding:0px;}
.ComboBox entry{border-right-width:0px;  }
.ComboBox entry.flat{border-right-width:0px;  }
.ComboBox entry:focus{border-right-width:0px; box-shadow: inset 0px 0px 0px 1px #62a0ea;}
.ComboBox button{padding-top:1px;padding-bottom:1px;border-width: 1px 1px 1px 1px; border-style: solid; border-color:@frame_color;}

.DropDownList button{padding:2px;}
.SplitContainer{padding:0px;border:0px;box-shadow:none;}
.SplitterPanel{padding:0px;margin:0px;border:0px;box-shadow:none;min-width:20px;min-height:20px;}
.SplitterPanel .frame{padding:0px;margin:0px;border:0px;box-shadow:none;}
.SplitterPanel .flat{padding:0px;margin:0px;border:0px;box-shadow:none;}

.TableLayoutPanel {box-shadow: 0px 0px 0px 1px @frame_color;padding:0px;}
.TableCellBorder grid>viewport.frame {box-shadow: 1px 1px 1px 0 @frame_color;border-width:0px;}
.ListView{border:inset 1px @frame_color;}
.ListView .Label{background-color:transparent;} 
.ListView checkbutton {padding:0px;}
.ListView .Label{background-color:transparent;} 
.ListView flowboxchild {padding:0px;}
.ListView flowboxchild viewport{padding:2px 0px;}
.ListView .GridBorder viewport{box-shadow: inset -1px -1px #eeeeee;}
.ListViewHeader {background-color:@theme_bg_color; padding:0px;min-height:6px; }
.ListViewHeader button{box-shadow:inset -1px 0px @frame_color;border-width:0px; border-radius:0px; padding:0px 0px 5px 0px;min-height:20px;min-width:6px;margin:0px; }
.ListViewHeader button.frame{border-width:0px;padding:0px 0px 0px 0px;margin:0px; }
.ListView .GroupLine{border-top:inset 1px #6677bb;}
.ListView .GroupTitle{padding-left:5px;padding-right:5px; color:#6677bb; }
.ListView .GroupSubTitle{padding-left:5px;padding-right:5px; }
.GroupBox>border{border: solid 1px rgba(76, 66, 66, 0.3);}

.ToolStrip,.MenuStrip{padding:0px;border-radius:0px;min-height:6px;min-width:6px;}
.ToolStrip>toolbutton,.ToolStrip>toolbutton label{padding:0px;margin:0px;}
.MenuStrip>menuitem,.MenuStrip>menuitem label{padding:0px;margin:0px;}
.ToolStrip button,.ToolStrip menubutton,.ToolStrip menubutton>button,.MenuStrip button,.MenuStrip menubutton,.MenuStrip menubutton>button{padding-top:0px;padding-bottom:0px; min-height:6px;min-width:6px;border-radius:0px;border-width:0px;border-style:none;outline:none;background:none;background-image:none;box-shadow:none;}
.ToolStrip button:hover,.ToolStrip menubutton:hover,.MenuStrip button:hover,.MenuStrip menubutton:hover{background-color:shade(@theme_bg_color,0.9);}
.ToolStrip button:active,.ToolStrip button:checked,.ToolStrip menubutton:active,.ToolStrip menubutton:checked,.MenuStrip button:active,.MenuStrip button:checked,.MenuStrip menubutton:active,.MenuStrip menubutton:checked{background-color:shade(@theme_bg_color,0.82);}
.ToolStripButtonFlat,.ToolStripButtonFlat>button{border:none;border-width:0px;border-style:none;outline:none;background:none;background-image:none;box-shadow:none;border-radius:0px;padding:1px 4px;min-height:6px;min-width:6px;}
.ToolStripButtonFlat:hover,.ToolStripButtonFlat>button:hover{background-color:shade(@theme_bg_color,0.9);}
.ToolStripButtonFlat:active,.ToolStripButtonFlat:checked,.ToolStripButtonFlat>button:active,.ToolStripButtonFlat>button:checked{background-color:shade(@theme_bg_color,0.82);}
.ToolStrip combox,.MenuStrip combox{padding-top:0px;padding-bottom:0px; min-height:6px;min-width:6px;border-radius:0px;border-width:0px;}
.ToolStrip entry,.MenuStrip entry{padding-top:0px;padding-bottom:0px; min-height:6px;min-width:6px;border-radius:0px;border-width:1px;}
.ToolStrip levelbar,.MenuStrip levelbar{padding:0px;min-height:6px;min-width:6px;border-radius:0px;border-width:0px;}
.StatusStrip {border-top:solid 1px @frame_color;}
.MenuItemButton {background-color:transparent;background:none; border:none; border-radius:0px;border-width:0px; padding:0px;box-shadow:none; min-width:1px;min-height:1px;}
.ToolStrip checkbutton,.MenuStrip checkbutton{padding:0px; }
/* Gtk3 hid the CheckButton indicator with checkbutton.DrawIndicator=false; Gtk4 removed that property and
   the old margin-left:-20px clip no longer hides Gtk4's separate 'check' node, so menu items rendered as
   checkboxes. Collapse the indicator node instead: always for top-level menu-bar items (.MenuBarItem), and
   for ordinary menu items only while unchecked (keeping the checkmark that a Checked item must show). */
.MenuItemButton:not(:checked):not(:indeterminate) check{min-width:0;min-height:0;-gtk-icon-size:0;margin:0;padding:0;border-width:0;background:none;box-shadow:none;opacity:0;}
.MenuBarItem check{min-width:0;min-height:0;-gtk-icon-size:0;margin:0;padding:0;border-width:0;background:none;box-shadow:none;opacity:0;}
/* The 'opens a submenu' arrow. Gtk3 got this from GtkMenuItem's theming; Gtk4 has no menu-item widget, so
   StripMenuItem draws it and this keeps it clear of the caption and slightly dimmed, as themes do. */
.SubmenuArrow{margin-left:12px;opacity:0.65;}
/* The item whose drop-down is currently open reads as selected. A GtkMenuItem did this itself on Gtk3;
   with no menu-item widget on Gtk4 the entry stayed unpainted, so it was impossible to tell which one the
   open submenu belonged to. Uses the theme's selection colours rather than a hardcoded blue.
   The background sits on the button and the foreground on the caption row, never both on the button: the
   drop-down popover is parented to the button, so an inheritable colour there cascades into the menu it
   opens and paints its captions white on white - the drop-down then looks empty. */
.MenuItemOpen,.MenuItemOpen:hover{background-color:@theme_selected_bg_color;}
.MenuItemOpenRow,.MenuItemOpenRow label{color:@theme_selected_fg_color;}
.ToolStrip menu button image,.MenuStrip menu button image{margin-left:-22px;}
.ToolStrip menu button image+label,.MenuStrip menu button image+label{margin-left:-2px; }
.ToolStrip>separator,.MenuStrip>separator{padding:0px;margin:2px;}

.ContextMenuStrip button image{margin-left:-22px; }
.ContextMenuStrip button image+label{margin-left:-2px; }
.ContextMenuStrip checkbutton{margin-left:-22px;}
.NumericUpDown{padding:1px; min-height:6px;min-width:6px; }
.NumericUpDown button.up{padding:0px;}
.NumericUpDown button.down{padding:0px;}
.NumericUpDown.horizontal entry{margin:1px;border-width:0px;padding:2px 3px; min-height:6px;min-width:6px;} 
.NumericUpDown.vertical entry{margin:1px;border-width:0px;padding:2px 3px; min-height:6px;min-width:6px;} 
.TrackBar {border-width:0px;box-shadow:none;}
.TrackBar scale trough{min-width:5px;min-height:5px;border-radius:2px;}
.TrackBar scale slider{min-width:16px;min-height:16px;margin:-6px;}
.PrintPreviewBack{background-color:#cccccc; border-radius:0px;}
.Paper{box-shadow: 0px 0px 3px 1px #999999;background:#ffffff; border-radius:0px;}
.PropertyGrid {box-shadow:0px 0px 0px 1px @frame_color; background:#eeeeee;}
.PropertyGrid button{background:#eeeeee;}
.PictureBox {padding:0px;}
tooltip { background-color: #ffffff; color: #555555; border: 1px solid #c9c9c9; box-shadow: 3px 3px 2px #787878;}
tooltip window,tooltip.background {background-color: #ffffff;}
tooltip label {padding: 0px 10px; color: #555555;}
.MDIForm{border-style:solid;border-width:0px 2px 2px 2px;border-color:#cccccc;}
.MDIPanel{background-color:#c0c0c0; box-shadow: inset 3px 3px 0px 0px #787878;}
";

            var appdirectory = "./";// StartupPath; // becauselinux the system commonly uses environment variable paths， will lead toDirectory/Environment the retrieved current directory is incorrect
            if (!File.Exists($"{appdirectory}/{Assembly.GetExecutingAssembly().GetName().Name}.dll"))
            {
                appdirectory = Path.GetDirectoryName(ExecutablePath);
            }
            var resourcepath = Path.Combine(appdirectory, "Resources");
            var themepath = Path.Combine(appdirectory, "theme");
            var themesetuppath = Path.Combine(themepath, "setup.theme");
            if (!Directory.Exists(resourcepath))
            {
                Directory.CreateDirectory(resourcepath);
            }
            if (!Directory.Exists(themepath))
            {
                Directory.CreateDirectory(themepath);
            }

            Gtk.Application.Init();
            App = new Gtk.Application(Assembly.GetExecutingAssembly().GetName().Name, GLib.ApplicationFlags.None);
            App.Register(GLib.Cancellable.Current);
            App.Shutdown += App_Shutdown;
            var quitAction = new GLib.SimpleAction("quit", null);
            quitAction.Activated += QuitActivated;
            App.AddAction(quitAction);
            var settings = Gtk.Settings.Default;
            settings.SplitCursor = true;
            settings.EnableAnimations = true;
            // Gtk4 removed the "gtk-enable-tooltips" GtkSettings property (tooltips are always enabled and
            // controlled per-widget). The GtkSharp C# property still exists but setting it raises a runtime
            // GLib-GObject-CRITICAL ('has no property named gtk-enable-tooltips'), so it is omitted.
            var iconpath = Path.Combine(appdirectory, "icon.png");
            // Gtk4 removed the file-based default window icon (SetDefaultIconFromFile); a default icon is now
            // resolved by name from the icon theme. Add the app directory to the theme's search path and set
            // "icon" as the default window icon-name, so an icon.png shipped beside the app is picked up
            // (best-effort; reliable resolution expects a hicolor layout under the search path).
            if (File.Exists(iconpath))
            {
                try
                {
                    var display = Gdk.Display.Default;
                    if (display != null)
                    {
                        Gtk.IconTheme.GetForDisplay(display).AddSearchPath(appdirectory);
                        Gtk.Window.DefaultIconName = "icon";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug("Unable to register the default window icon", ex);
                }
            }

            var css = new Gtk.CssProvider();
            var cssBuilder=new StringBuilder();

            if (File.Exists(themesetuppath))
            {
                var setuptheme = File.ReadAllLines(themesetuppath, Text.Encoding.UTF8);
                var nameValue = setuptheme.Where(w=>w.Contains("=")).ToDictionary(k => k.Split('=')[0],v=>v.Split('=')[1]);
                nameValue.TryGetValue("UseDefaultStyle", out var usedef);
                if (usedef != "false")
                    cssBuilder.AppendLine(css_style);

                nameValue.TryGetValue("AutoTheme", out var autotheme);
                if (autotheme == "false")
                {
                    if (nameValue.TryGetValue("DefaultThemeName", out var themename))
                        Gtk.Settings.Default.ThemeName = themename;
                }
                if (nameValue.TryGetValue("UseCustomTheme", out var usetheme))
                {
                    if (usetheme == "true")
                    {
                        if (nameValue.TryGetValue("ThemeFolder", out var themefolder))
                        {
                            if (Directory.Exists(themefolder))
                            {
                                try
                                {
                                    var _themefolder = Path.GetFullPath(themefolder);
                                    Environment.SetEnvironmentVariable("GTK_DATA_PREFIX", _themefolder);
                                }
                                catch (Exception ex)
                                {
                                    GtkLogger.Info(ex.Message);
                                }
                                finally
                                {
                                    Gtk.Settings.Default.ThemeName = nameValue["Name"];
                                }
                            }
                            else
                            {
                                GtkLogger.Info(themefolder + System.Properties.Resources.Gtk_Application_Init_Directory_does_not_exist);
                            }
                        }
                    }
                }
                if (nameValue.TryGetValue("UseCustomStyle", out var customstyle))
                {
                    if (customstyle == "true" && nameValue.TryGetValue("StylePath", out var stylefile))
                    {
                        var stylepath = Path.Combine(themepath, stylefile);
                        if (File.Exists(stylepath))
                        {
                            var styletext = File.ReadAllText(stylepath, Text.Encoding.UTF8);
                            cssBuilder.AppendLine(styletext);
                        }
                        else
                        {
                            File.WriteAllText(stylepath, System.Properties.Resources.Gtk_Application_Init_Here_you_can_customize_or_adjust_the_control_style, Text.Encoding.UTF8);
                        }
                    }
                }
            }
            else
            {
                cssBuilder.AppendLine(css_style);

                var setupthemecontent = new StringBuilder();
                setupthemecontent.AppendLine("[setup]");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Whether_to_follow_the_system_theme_by_default);
                setupthemecontent.AppendLine("AutoTheme=true");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Whether_to_apply_built_in_styles);
                setupthemecontent.AppendLine("UseDefaultStyle=true");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Specify_the_theme_valid_when_AutoTheme_is_false);
                setupthemecontent.AppendLine("DefaultThemeName=Default");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Whether_to_use_a_custom_theme_corresponding_to_custom_theme);
                setupthemecontent.AppendLine("UseCustomTheme=false");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Whether_to_use_custom_style_corresponding_to_custom_style);
                setupthemecontent.AppendLine("UseCustomStyle=true");
                setupthemecontent.AppendLine().AppendLine("[custom theme]");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Custom_theme_name);
                setupthemecontent.AppendLine("Name=mytheme");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_The_folder_where_the_theme_files_are_located);
                setupthemecontent.AppendLine("ThemeFolder=theme");
                setupthemecontent.AppendLine().AppendLine("[custom style]");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Freely_defined_style_files);
                setupthemecontent.AppendLine("StylePath=style.css");

                File.WriteAllText(themesetuppath, setupthemecontent.ToString(), Text.Encoding.UTF8);
            }

            // Gtk4 raises no diagnostic for a stylesheet it cannot make sense of beyond this signal, so
            // surface it - a user theme that stops applying is otherwise completely invisible.
            css.ParsingError += (_, args) => GtkLogger.Debug($"CSS parsing error in {args.Section}");
            // A stylesheet loaded from a string has no base directory on Gtk4, and its
            // gtk_css_parser_resolve_url() then silently drops every relative url() - images and, worse,
            // @import - with no parsing error and nothing on stderr. Gtk3 resolved those against the
            // process working directory, so a user theme that shipped relative references simply stopped
            // applying. Make them absolute first: the working directory keeps the Gtk3 contract, and the
            // application directory covers the case Gtk3 got wrong, an app launched from elsewhere.
            // Imports are inlined rather than left to Gtk, which would open the file itself and drop the
            // rules around any byte that is not valid UTF-8 - silently. See CssUri.InlineImports.
            var userCss = CssUri.InlineImports(cssBuilder.ToString(), Directory.GetCurrentDirectory(), appdirectory);
            css.LoadFromData(CssUri.ResolveUrls(userCss, Directory.GetCurrentDirectory(), appdirectory));
            Gtk.StyleContext.AddProviderForDisplay(Gdk.Display.Default, css, (uint)StyleProviderPriority.Application);

            // Gtk4: toolbar/menu-strip buttons keep the theme's rounded frame (box-shadow/border) even at
            // Application priority, so force the flat, borderless look via a dedicated User-priority provider
            // (the same priority the per-item style overrides use, which is proven to take effect here).
            var toolStripFlat = new Gtk.CssProvider();
            toolStripFlat.LoadFromData(UserPriorityCss);
            Gtk.StyleContext.AddProviderForDisplay(Gdk.Display.Default, toolStripFlat, (uint)StyleProviderPriority.User);
        }

        return App;
    }

    /// <summary>
    /// Runs the specified main form.
    /// </summary>
    /// <param name="mainForm">
    /// The main form.
    /// </param>
    public static void Run(Form mainForm)
    {
        Run(new ApplicationContext(mainForm));
    }

    /// <summary>
    /// Begins running a standard application message loop on the current thread, without a form.
    /// </summary>
    /// <remarks>
    /// The loop runs until <see cref="M:System.Windows.Forms.Application.Exit" /> or
    /// <see cref="M:System.Windows.Forms.Application.ExitThread" /> is called. This overload did not exist,
    /// so an application that creates its windows itself had no way to start a message loop.
    /// </remarks>
    public static void Run()
    {
        Run(new ApplicationContext());
    }

    /// <summary>
    /// Begins running a standard application message loop on the current thread, with the specified
    /// <see cref="T:System.Windows.Forms.ApplicationContext" />.
    /// </summary>
    /// <param name="context">
    /// The context under which to run the loop. Its
    /// <see cref="P:System.Windows.Forms.ApplicationContext.MainForm" /> is shown if it has one, and the
    /// loop exits when the context raises
    /// <see cref="E:System.Windows.Forms.ApplicationContext.ThreadExit" />.
    /// </param>
    /// <remarks>
    /// <see cref="T:System.Windows.Forms.ApplicationContext" /> existed as a type but nothing consumed it,
    /// so the multi-form / no-main-form startup pattern had no entry point at all.
    /// </remarks>
    public static void Run(ApplicationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        lock (internalSyncObject)
        {
            if (MessageLoop)
            {
                // WinForms permits only one message loop per thread.
                throw new InvalidOperationException(
                    "Starting a second message loop on a thread that is already running one is not supported.");
            }
        }

        // The thread that runs the message loop is the UI thread Control.InvokeRequired compares against.
        Control.CaptureUiThread();

        // From here on, an exception escaping a handler has somewhere defined to go.
        InstallUnhandledExceptionHook();

        var previousContext = CurrentContext;
        CurrentContext = context;

        // Set when the context asks to exit. Quit() is a no-op while no loop is running, so an exit that
        // arrives before the loop starts has to be remembered rather than issued and lost.
        var exitRequested = false;

        void OnContextThreadExit(object? sender, EventArgs e)
        {
            exitRequested = true;
            Quit();
        }

        context.ThreadExit += OnContextThreadExit;

        try
        {
            var mainForm = context.MainForm;
            if (mainForm != null)
            {
                ShowMainForm(mainForm);
            }

            MessageLoop = true;

            // Showing the main form runs user code - Load, and anything it calls - and that code is allowed
            // to close the form again straight away (an app that only puts up a message box from Load, then
            // closes, is the canonical case). The close then reaches ExitThread before there is a loop to
            // quit, so entering one here would block forever on a quit that already happened. WinForms
            // returns from Run in exactly this situation.
            if (!exitRequested)
            {
                Gtk.Application.Run();
            }
        }
        finally
        {
            MessageLoop = false;
            context.ThreadExit -= OnContextThreadExit;
            CurrentContext = previousContext;

            OnThreadExit(EventArgs.Empty);

            // WinForms disposes the context once the loop it drove has terminated, which in turn disposes
            // the main form.
            context.Dispose();

            OnApplicationExit(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Shows the context's main form, applying the same taskbar handling
    /// <see cref="M:System.Windows.Forms.Application.Run(System.Windows.Forms.Form)" /> has always used.
    /// </summary>
    private static void ShowMainForm(Form mainForm)
    {
        if (mainForm.ShowInTaskbar == false)
        {
            var pw = new Gtk.Window();
            pw.Decorated = false;
            pw.WidthRequest = 1;
            pw.HeightRequest = 1;
            mainForm.self.TransientFor = pw;
            pw.Shown += Pw_Shown;
            pw.Show();
        }

        mainForm.Show();
    }

    /// <summary>
    /// Sets the compatible text rendering default.
    /// </summary>
    /// <param name="defaultValue">
    /// if set to
    /// <c>
    /// true
    /// </c>default value.
    /// </param>
    public static void SetCompatibleTextRenderingDefault(bool defaultValue)
    {

    }

    /// <summary>
    /// Sets the high dpi mode.
    /// </summary>
    /// <param name="highDpiMode">
    /// The high dpi mode.
    /// </param>
    /// <returns>
    /// </returns>
    public static bool SetHighDpiMode(HighDpiMode highDpiMode)
    {
        return true;
    }

    private static void App_Shutdown(object sender, EventArgs e)
    {
        GtkLogger.Info("App_Shutdown");
        Quit();
    }

    // Control_Destroyed was removed: Run now routes every overload through ApplicationContext, which ends
    // the loop off the main form's HandleDestroyed event instead.

    private static void Pw_Shown(object? sender, EventArgs e)
    {
        var pw = (Gtk.Window)sender!;
        pw.Hide();
    }

    private static void QuitActivated(object sender, EventArgs e)
    {
        Quit();
    }

    public static void Quit()
    {
        if (!DontQuit)
        {
#if DEBUG_QUIT
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            // Only quit when a GTK main loop is actually running. Calling gtk_main_quit with no active
            // loop trips a fatal Gtk-CRITICAL assertion (main_loops != NULL) - e.g. Application.Exit()
            // invoked with no prior Application.Run(), as in unit tests. Real WinForms treats Exit with no
            // running message loop as a no-op, so mirror that here.
            // Gtk4: no Gtk.Main; quit the GtkApplication instance if one exists (no-op otherwise).
            Gtk.Application.Quit();
        }
    }

    public static void Init(bool dontQuit)
    {
        Forms.Application.DontQuit = dontQuit;
        Application.Init();
    }
}