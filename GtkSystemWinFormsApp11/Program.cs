using System.Windows.Forms;

namespace GtkSystemWinFormsApp11
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            // ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            MessageBox.ReplaceInstance(2000);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}