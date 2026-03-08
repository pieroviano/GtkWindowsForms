using System.IO;
using System.Linq;
using System.Xml.Linq;
using StringComparison = System.StringComparison;
using StringSplitOptions = System.StringSplitOptions;

namespace Gtk.Windows.FormsDesigner;

/// <summary>
/// CsprojFixer is a utility class designed to modify .csproj files to ensure compatibility with the Windows Forms Designer in a GTK environment. It performs the following fixes:
/// </summary>
public static class CsprojFixer
{
    /// <summary>
    /// Fixes the project.
    /// </summary>
    /// <param name="projectFullPath">The project full path.</param>
    /// <exception cref="FileNotFoundException">Project file not found.</exception>
    public static void FixProject(string projectFullPath)
    {
        if (!File.Exists(projectFullPath))
            throw new FileNotFoundException("Project file not found.", projectFullPath);

        var doc = XDocument.Load(projectFullPath);

        // Handle both SDK-style and legacy projects (namespace-safe)
        var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

        // Fix TargetFramework
        foreach (var tf in doc.Descendants(ns + "TargetFramework"))
        {
            if (!string.IsNullOrWhiteSpace(tf.Value))
            {
                tf.Value = tf.Value.Replace("-windows", "");
            }
        }

        // Fix TargetFrameworks (semicolon-separated)
        foreach (var tfs in doc.Descendants(ns + "TargetFrameworks"))
        {
            if (!string.IsNullOrWhiteSpace(tfs.Value))
            {
                tfs.Value = string.Join(";",
                    tfs.Value
                        .Split(';').Where(i=>!string.IsNullOrEmpty(i))
                        .Select(v => v.Replace("-windows", ""))
                );
            }
        }

        // Fix UseWindowsForms
        foreach (var uwf in doc.Descendants(ns + "UseWindowsForms"))
        {
            var value = uwf.Value?.Trim();

            if (value != null &&
                (value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                 value.Equals("trur", StringComparison.OrdinalIgnoreCase)))
            {
                uwf.Value = "false";
            }
        }

        doc.Save(projectFullPath);
    }
}
