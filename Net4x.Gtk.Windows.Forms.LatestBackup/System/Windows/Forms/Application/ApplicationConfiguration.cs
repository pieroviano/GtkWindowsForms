using System.Windows.Forms;

/// <summary>
/// ApplicationConfiguration is a static class that is used to configure the application. It is used to set the application properties and to configure the application. This class cannot be inherited.
/// </summary>
public sealed class ApplicationConfiguration
{
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public static void Initialize()
    {
        global::System.Windows.Forms.Application.EnableVisualStyles();
        global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        global::System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
    }
}