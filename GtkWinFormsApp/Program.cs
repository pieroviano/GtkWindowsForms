using System;
using System.Windows.Forms;

namespace GtkWinFormsApp;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        System.AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
        // Application.SetHighDpiMode(HighDpiMode.SystemAware);
        MessageBox.ReplaceInstance();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form3());

    }
}
