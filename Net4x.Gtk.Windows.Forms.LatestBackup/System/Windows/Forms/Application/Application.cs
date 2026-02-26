using Gtk;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides static methods and properties to manage an application, such as methods to start and stop an application, to process Windows messages, and properties to get information about an application. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public sealed class Application
    {
        static Application()
        {
            Init();
        }

        private static string appDataDirectory
        {
            get
            {
                string[] assemblyFullName = Assembly.GetEntryAssembly().FullName.Split(',');
                string _namespace = assemblyFullName[0];
                AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
                return Path.Combine(_namespace, assembly.Name, assembly.Version.ToString());
            }
        }
        /// <summary>
        /// Gets the path for the application data that is shared among all users.
        /// </summary>
        /// <returns>The path for the application data that is shared among all users.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static string CommonAppDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appDataDirectory);
            }
        }
        /// <summary>
        /// Gets the path for the application data of a user.
        /// </summary>
        /// <returns>The path for the application data of a user.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static string UserAppDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataDirectory);
            }
        }
        /// <summary>
        /// Gets the path for the application data of a local, non-roaming user.
        /// </summary>
        /// <returns>The path for the application data of a local, non-roaming user.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static string LocalUserAppDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appDataDirectory);
            }
        }
        /// <summary>
        /// Gets the path for the executable file that started the application, including the executable name.
        /// </summary>
        /// <returns>The path and executable name for the executable file that started the application.This path will be different depending on whether the Windows Forms application is deployed using ClickOnce. ClickOnce applications are stored in a per-user application cache in the C:\Documents and Settings\username directory. For more information, see Accessing Local and Remote Data in ClickOnce Applications.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static string ExecutablePath
        {
            get
            {
                System.Diagnostics.ProcessModule module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                if (module.ModuleName.ToLower() == "dotnet" || module.ModuleName.ToLower() == "dotnet.exe")
                    return Assembly.GetEntryAssembly().Location;
                else
                    return module.FileName;
            }
        }
        /// <summary>
        /// Gets the path for the executable file that started the application, not including the executable name.
        /// </summary>
        /// <returns>The path for the executable file that started the application.This path will be different depending on whether the Windows Forms application is deployed using ClickOnce. ClickOnce applications are stored in a per-user application cache in the C:\Documents and Settings\username directory. For more information, see Accessing Local and Remote Data in ClickOnce Applications.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static string StartupPath { get { return System.IO.Directory.GetCurrentDirectory(); } }


        private static readonly object internalSyncObject = new object();
        /// <summary>
        /// Gets or sets the culture information for the current thread.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Globalization.CultureInfo" /> representing the culture information for the current thread.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" />
        /// </PermissionSet>
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
        /// Gets or sets the current input language for the current thread.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.InputLanguage" /> representing the current input language for the current thread.
        /// </returns>
        /// <filterpriority>2
        /// </filterpriority>
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
        /// Gets a collection of open forms owned by the application.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.FormCollection" /> containing all the currently open forms owned by this application.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Window="AllWindows" />
        /// </PermissionSet>
        public static FormCollection OpenForms
        {
            get
            {
                FormCollection forms = new FormCollection();
                var windows = Gtk.Window.ListToplevels().Where(w => w.IsVisible == true);
                foreach (Gtk.Window w in windows)
                    if (w.Data.ContainsKey("Control") && w.Data["Control"] is Form form)
                        forms.Add(form);
                return forms;
            }
        }
        /// <summary>
        /// Processes all Windows messages currently in the message queue.
        /// </summary>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static void DoEvents()
        {
            while (Gtk.Application.EventsPending())
                Gtk.Application.RunIteration(false);
        }

        public static Gtk.Application App { get; private set; }
        public static Gtk.Application Init()
        {
            if (App == null)
            {
                string css_style = @"

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

                string appdirectory = "./";// StartupPath; // becauselinux the system commonly uses environment variable paths， will lead toDirectory/Environment the retrieved current directory is incorrect
                if (!File.Exists($"{appdirectory}/GtkSystem.Windows.Forms.dll"))
                {
                    appdirectory = Path.GetDirectoryName(ExecutablePath);
                }
                string resourcepath = Path.Combine(appdirectory, "Resources");
                string themepath = Path.Combine(appdirectory, "theme");
                string themesetuppath = Path.Combine(themepath, "setup.theme");
                if (!Directory.Exists(resourcepath))
                {
                    Directory.CreateDirectory(resourcepath);
                }
                if (!Directory.Exists(themepath))
                {
                    Directory.CreateDirectory(themepath);
                }

                Gtk.Application.Init();
                App = new Gtk.Application("GtkSystem.Windows.Forms", GLib.ApplicationFlags.None);
                App.Register(GLib.Cancellable.Current);
                App.Shutdown += App_Shutdown;
                var quitAction = new GLib.SimpleAction("quit", null);
                quitAction.Activated += QuitActivated;
                App.AddAction(quitAction);
                Gtk.Settings settings = Gtk.Settings.Default;
                settings.SplitCursor = true;
                settings.EnableAnimations = true;
                string iconpath = Path.Combine(appdirectory, "icon.png");
                if (File.Exists(iconpath))
                    Gtk.Window.SetDefaultIconFromFile(iconpath);

                Gtk.CssProvider css = new Gtk.CssProvider();
                StringBuilder cssBuilder = new StringBuilder();

                if (File.Exists(themesetuppath))
                {
                    string[] setuptheme = File.ReadAllLines(themesetuppath, Text.Encoding.UTF8);
                    Dictionary<string, string> nameValue = setuptheme.Where(w => w.Contains("=")).ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
                    nameValue.TryGetValue("UseDefaultStyle", out string usedef);
                    if (usedef != "false")
                        cssBuilder.AppendLine(css_style);

                    nameValue.TryGetValue("AutoTheme", out string autotheme);
                    if (autotheme == "false")
                    {
                        if (nameValue.TryGetValue("DefaultThemeName", out string themename))
                            Gtk.Settings.Default.ThemeName = themename;
                    }
                    if (nameValue.TryGetValue("UseCustomTheme", out string usetheme))
                    {
                        if (usetheme == "true")
                        {
                            if (nameValue.TryGetValue("ThemeFolder", out string themefolder))
                            {
                                if (Directory.Exists(themefolder))
                                {
                                    try
                                    {
                                        string _themefolder = Path.GetFullPath(themefolder);
                                        Environment.SetEnvironmentVariable("GTK_DATA_PREFIX", _themefolder);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                    finally
                                    {
                                        Gtk.Settings.Default.ThemeName = nameValue["Name"];
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(themefolder + System.Properties.Resources.Gtk_Application_Init_Directory_does_not_exist);
                                }
                            }
                            if (nameValue.TryGetValue("ThemeCssPath", out string themecss))
                            {
                                if (File.Exists(themecss))
                                {
                                    cssBuilder.AppendFormat("@import url(\"{0}\");", themecss).AppendLine();
                                }
                            }
                        }
                    }
                    if (nameValue.TryGetValue("UseCustomStyle", out string customstyle))
                    {
                        if (customstyle == "true" && nameValue.TryGetValue("StylePath", out string stylefile))
                        {
                            string stylepath = Path.Combine(themepath, stylefile);
                            if (File.Exists(stylepath))
                            {
                                string styletext = File.ReadAllText(stylepath, Text.Encoding.UTF8);
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

                    StringBuilder setupthemecontent = new StringBuilder();
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

        private static void QuitActivated(object sender, EventArgs e)
        {
            Gtk.Application.Quit();
        }
        private static void App_Shutdown(object sender, EventArgs e)
        {
            Console.WriteLine("App_Shutdown");
            Gtk.Application.Quit();
        }

        public static bool SetHighDpiMode(HighDpiMode highDpiMode)
        {
            return true;
        }
        /// <summary>
        /// Enables visual styles for the application.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static void EnableVisualStyles()
        {

        }
        /// <summary>
        /// Sets the application-wide default for the UseCompatibleTextRendering property defined on certain controls.
        /// </summary>
        /// <param name="defaultValue">The default value to use for new controls. If true, new controls that support UseCompatibleTextRendering use the GDI+ based
        /// <see cref="System.Drawing.Graphics" /> class for text rendering; if false, new controls use the GDI based
        /// <see cref="System.Windows.Forms.TextRenderer" /> class.
        /// </param>
        /// <exception cref="System.InvalidOperationException">You can only call this method before the first window is created by your Windows Forms application.
        /// </exception>
        public static void SetCompatibleTextRenderingDefault(bool defaultValue)
        {

        }
        /// <summary>
        /// Begins running a standard application message loop on the current thread, and makes the specified form visible.
        /// </summary>
        /// <param name="mainForm">A
        /// <see cref="System.Windows.Forms.Form" /> that represents the form to make visible.
        /// </param>
        /// <exception cref="System.InvalidOperationException">A main message loop is already running on the current thread.
        /// </exception>
        /// <filterpriority>2
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Security.Permissions.UIPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static void Run(Form mainForm)
        {
            mainForm.self.Destroyed += Control_Destroyed;
            mainForm.Show();
            Gtk.Application.Run();
        }
        private static void Control_Destroyed(object sender, EventArgs e)
        {
            ExitThread();
        }
        /// <summary>
        /// Informs all message pumps that they must terminate, and then closes all application windows after the messages have been processed.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static void Exit()
        {
            ExitThread();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void Exit(CancelEventArgs e)
        {
            lock (internalSyncObject)
            {
                try
                {
                    if (e == null)
                    {
                        Gtk.Application.Quit();
                    }
                    else
                    {
                        if (e.Cancel == false)
                        {
                            Gtk.Application.Quit();
                        }
                    }
                }
                finally
                {
                }
            }
        }
        /// <summary>
        /// Exits the message loop on the current thread and closes all windows on the thread.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static void ExitThread()
        {
            lock (internalSyncObject)
            {
                try
                {

                    Gtk.Application.Quit();
                }
                finally { }
            }
        }
    }

    public static class InitAppliction
    {

        private static Gtk.Application app = Application.Init();
        static InitAppliction()
        {

        }
    }
}
public sealed class ApplicationConfiguration
{
    public static void Initialize()
    {
        global::System.Windows.Forms.Application.EnableVisualStyles();
        global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        global::System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
    }
}
