using System.Xml.Linq;

namespace Net4x.ResxProcessor;

/// <summary>
/// Provides the entry point for the application that processes .resx resource files in a specified directory tree,
/// adding a &lt;data&gt; element with the relative file path if it does not already exist.
/// </summary>
/// <remarks>The application prompts for a root folder if none is specified via command-line arguments. It
/// validates the existence of the folder before proceeding. For each .resx file found, the application checks for a
/// &lt;data&gt; element named "this_FileName" and adds it if missing, storing the file's relative path. The process is
/// intended to be run in environments where multiple instances may be started; synchronization is handled by the
/// Processes class. The application returns 0 on success and 1 if the specified folder does not exist.</remarks>
public class Program
{
    /// <summary>
    /// Serves as the entry point for the application, processing resource files in the specified root folder.
    /// </summary>
    /// <remarks>The method expects the root folder to exist and contain .resx files to process. If the folder
    /// does not exist, an error message is displayed and the application exits with a non-zero code. All .resx files
    /// found in the root folder and its subdirectories are processed.</remarks>
    /// <param name="args">An array of command-line arguments. The first argument, if provided, specifies the root folder to process. If no
    /// arguments are supplied, the user is prompted to enter the root folder path.</param>
    /// <returns>An integer exit code. Returns 0 if processing completes successfully; returns 1 if the specified folder does not
    /// exist.</returns>
    public static int Main(string[] args)
    {
        return DoWork(args, new Processes());
    }

    internal static int DoWork(string[] args, Processes processes)
    {
        string rootFolder;

        if (args.Length > 0)
        {
            args[0] = args[0].Trim('"');
            rootFolder = Path.GetFullPath(args[0]);
        }
        else
        {
            Console.Write("Enter root folder: ");
            rootFolder = Path.GetFullPath(Console.ReadLine() ?? "");
        }

        if (!Directory.Exists(rootFolder))
        {
            Console.Error.WriteLine("Folder does not exist.");
            return 1;
        }

        processes.WaitSameProcessesStartedEarlier();

        var resxFiles = Directory.GetFiles(
            rootFolder,
            "*.resx",
            SearchOption.AllDirectories);

        foreach (var resxPath in resxFiles)
        {
            ProcessResx(rootFolder, resxPath);
        }

        return 0;
    }

    internal static void ProcessResx(string rootFolder, string resxPath)
    {
        var fileName = "this_FileName";
        var relativePath = Path.GetRelativePath(rootFolder, resxPath);

        var doc = XDocument.Load(resxPath);

        var root = doc.Root;
        if (root == null || root.Name.LocalName != "root")
        {
            Console.WriteLine($"Skipped (invalid resx): {relativePath}");
            return;
        }

        // Check if <data name="FileName"> already exists
        var existing = root
            .Elements("data")
            .FirstOrDefault(e =>
                string.Equals(
                    (string?)e.Attribute("name"),
                    fileName,
                    StringComparison.Ordinal));

        if (existing != null)
        {
            Console.WriteLine($"Skipped (exists): {relativePath}");
            return;
        }

        // Create new <data> element
        var dataElement =
            new XElement("data",
                new XAttribute("name", fileName),
                new XAttribute(XNamespace.Xml + "space", "preserve"),
                new XElement("value", relativePath));

        // Append at the end (resx tooling is order-tolerant)
        root.Add(dataElement);

        doc.Save(resxPath);

        Console.WriteLine($"Updated: {relativePath}");
    }
}