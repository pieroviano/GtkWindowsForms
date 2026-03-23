namespace System.Windows.Forms
{

    /// <summary>
    /// InitAppliction is a static class that is used to initialize the application. It is used to set the application properties and to initialize the application. This class cannot be inherited.
    /// </summary>
    public static class InitAppliction
    {

        private static readonly Gtk.Application app = Application.Init();

        static InitAppliction()
        {

        }
    }
}
