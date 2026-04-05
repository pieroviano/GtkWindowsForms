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
public sealed class Application
{

    private static readonly object internalSyncObject = new();

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
    /// Gets the common application data path.
    /// </summary>
    /// <value>
    /// The common application data path.
    /// </value>
    public static string CommonAppDataPath
    {
        get
        {
            return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appDataDirectory);
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
            var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            if (module.ModuleName.ToLower() == "dotnet" || module.ModuleName.ToLower() == "dotnet.exe")
                return Assembly.GetEntryAssembly().Location;
            else
                return module.FileName;
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
            return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appDataDirectory);
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
    public static string StartupPath { get { return System.IO.Directory.GetCurrentDirectory(); } }

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
            return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataDirectory);
        }
    }

    private static string appDataDirectory
    {
        get
        {
            var assemblyFullName = Assembly.GetEntryAssembly().FullName.Split(',');
            var _namespace = assemblyFullName[0];
            var assembly = Assembly.GetExecutingAssembly().GetName();

            return Path.Combine(_namespace, assembly.Name, assembly.Version.ToString());
        }
    }

    /// <summary>
    /// Does the events.
    /// </summary>
    public static void DoEvents()
    {
        while (Gtk.Application.EventsPending())
            Gtk.Application.RunIteration(false);
    }

    /// <summary>
    /// Enables the visual styles.
    /// </summary>
    public static void EnableVisualStyles()
    {

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
    public static Gtk.Application Init()
    {
        if (App == null)
        {
            var css_style = @"

/* Define control styles*/

@define-color frame_color  alpha(@theme_fg_color, 0.2);
@define-color frame3d_color  alpha(@theme_fg_color, 0.2);

@define-color line_color #ECECEC;
@define-color separator_color1 #C6C5C4;
@define-color separator_color2 #D6D7D8;

.DefaultThemeStyle{padding: 0px 2px; border-style:solid;min-height:6px;min-width:6px;}
.DefaultThemeStyle entry{
   padding: 4px 5px; border-width: 1px; border-style: solid; border-color:@frame_color;
   background-color: @theme_base_color; color: @theme_text_color;
}
.DefaultThemeStyle entry.flat{
   padding: 4px 5px; border-width: 1px; border-style: solid; border-color:@frame_color;
   background-color: @theme_base_color; color: @theme_text_color;
}
.DefaultThemeStyle button{padding:4px 3px;}
.BorderNone{border-style:none;box-shadow:none;}

.BorderFixedSingle{border-width:0px;border-style:none;padding:1px;box-shadow: inset 0px 0px 0px 1px @frame_color;}
.BorderFixed3D{border-width:0px;border-style:none; padding:2px; box-shadow: inset 1px 1px 1px 2px @frame3d_color;}

.DataGridView {border-width:1px;margin:-3px;}
.GridViewCell-Button{ color:@theme_text_color; border:solid 1px @frame_color; background-color: shade(@theme_bg_color, 0.7);}
.GridViewCell-Button:hover{background-color: shade(@theme_bg_color, 0.8);}
.GridViewCell-Button:selected{ color:blue}

.LinkLabel{border-style:none;}
.TextBox{}
.ComboBox{padding:0px;}
.ComboBox entry{border-right-width:0px;  }
.ComboBox entry.flat{border-right-width:0px;  }
.ComboBox entry:focus{border-right-width:0px; box-shadow: inset 0px 0px 0px 1px #62a0ea;}
.ComboBox button{padding-top:0px;padding-bottom:0px;border-width: 1px 1px 1px 1px; border-style: solid; border-color:@frame_color;}

.DropDownList button{padding:0px;}
.SplitContainer{padding:0px;border:0px;box-shadow:none;}
/* When scrollbars are present， width and height less than60px有 exception information output */
.SplitterPanel{padding:0px;margin:0px;border:0px;box-shadow:none;min-width:60px;min-height:60px;}
.SplitterPanel .frame{padding:0px;margin:0px;border:0px;box-shadow:none;}
.SplitterPanel .flat{padding:0px;margin:0px;border:0px;box-shadow:none;}

.TableLayoutPanel {box-shadow: 1px 1px 1px 0px @frame_color;}
.TableLayoutPanel viewport.frame {box-shadow: inset 1px 1px 1px 0 @frame_color;}
.ListView{border:inset 1px @frame_color;}
.ListView .Label{background-color:transparent;} 
.ListView checkbutton {padding:0px;}
.ListView .Label{background-color:transparent;} 
.ListView flowboxchild {padding:0px;}
.ListView flowboxchild viewport{padding:2px 0px;}
.ListView .GridBorder viewport{box-shadow: inset -1px -1px #eeeeee;}
.ListViewHeader {background-color:@theme_bg_color; opacity:0.88;padding:0px;min-height:6px; }
.ListViewHeader button{box-shadow:inset -1px 0px @frame_color;border-width:0px; border-radius:0px; padding:0px 0px 5px 0px;min-height:20px;min-width:6px;margin:0px; }
.ListViewHeader button.frame{border-width:0px;padding:0px 0px 0px 0px;margin:0px; }
.ListView .GroupLine{border-top:inset 1px #6677bb;}
.ListView .GroupTitle{padding-left:5px;padding-right:5px; color:#6677bb; }
.ListView .GroupSubTitle{padding-left:5px;padding-right:5px; }
.StatusStrip{padding:0px; border-width:1px 0px 0px 0px; border-top:solid 1px @frame_color;}
.ToolStrip button{padding:0px;}

.NumericUpDown{border-width:1px;padding:2px; }
.NumericUpDown button.up{border-width:0px;padding:0px;}
.NumericUpDown button.down{border-width:0px;padding:0px;}
.NumericUpDown.horizontal entry{border-width:0px;padding:2px 3px; min-height:6px;min-width:6px;} 
.NumericUpDown.vertical entry{border-width:0px;padding:2px 3px; min-height:6px;min-width:6px;} 
.TrackBar {border-width:0px;box-shadow:none;}
.PrintPreviewBack{background-color:#cccccc; border-radius:0px;}
.Paper{box-shadow: 0px 0px 3px 1px #999999;background:#ffffff; border-radius:0px;}
.PropertyGrid {box-shadow:0px 0px 0px 1px @frame_color; background:#eeeeee;}
.PropertyGrid button{background:#eeeeee;}
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
            var iconpath = Path.Combine(appdirectory, "icon.png");
            if (File.Exists(iconpath))
                Gtk.Window.SetDefaultIconFromFile(iconpath);

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
                        if (nameValue.TryGetValue("ThemeCssPath", out var themecss))
                        {
                            if (File.Exists(themecss))
                            {
                                cssBuilder.AppendFormat("@import url(\"{0}\");", themecss).AppendLine();
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
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_css_file_path);
                setupthemecontent.AppendLine("ThemeCssPath=theme/mytheme/theme.css");
                setupthemecontent.AppendLine().AppendLine("[custom style]");
                setupthemecontent.AppendLine(System.Properties.Resources.Gtk_Application_Init_Freely_defined_style_files);
                setupthemecontent.AppendLine("StylePath=style.css");

                File.WriteAllText(themesetuppath, setupthemecontent.ToString(), Text.Encoding.UTF8);
            }

            css.LoadFromData(cssBuilder.ToString());
            Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, css, StyleProviderPriority.Application);
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
        mainForm.self.Destroyed += Control_Destroyed;
        mainForm.Show();
        Gtk.Application.Run();
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

    private static void Control_Destroyed(object sender, EventArgs e)
    {
        ExitThread();
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
            Gtk.Application.Quit();
        }
    }

    public static void Init(bool dontQuit)
    {
        Forms.Application.DontQuit = dontQuit;
        Application.Init();
    }
}