using System.Windows.Forms;

namespace AutoClosingMessageBoxApp;

public static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static int Main()
    {
        try
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            global::System.Windows.Forms.Application.EnableVisualStyles();
            global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            global::System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.Run(new Form1());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
            return -1;
        }
        Environment.Exit(0);
        return 0;
    }
}