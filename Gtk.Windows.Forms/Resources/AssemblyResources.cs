using System.Reflection;

namespace GtkSystem.Windows.Forms.Resources;

public class AssemblyResources
{

    public static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();

    public static string ToCursorsUri(string filename) => $"Resources.Cursors.{filename}";

    public static string ToResourcesUri(string subdirctory, string filename) => $"Resources.{subdirctory}.{filename}";

    public static string ToSystemUri(string filename) => $"Resources.System.{filename}";

}