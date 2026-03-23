using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gtk;

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
