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
    /// <returns><see langword="true" /> when the project needed a fix and was rewritten.</returns>
    /// <exception cref="FileNotFoundException">Project file not found.</exception>
    /// <remarks>
    /// The file is saved only when a rule actually changed it: this runs on every build, and on a
    /// project that is already GTK-shaped an unconditional save would keep touching a file the IDE
    /// has open, which makes it ask to reload the project over and over.
    /// </remarks>
    public static bool FixProject(string projectFullPath)
    {
        if (!File.Exists(projectFullPath))
            throw new FileNotFoundException("Project file not found.", projectFullPath);

        var doc = XDocument.Load(projectFullPath);
        var changed = false;

        // Handle both SDK-style and legacy projects (namespace-safe)
        var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

        // Fix TargetFramework
        foreach (var tf in doc.Descendants(ns + "TargetFramework"))
        {
            if (!string.IsNullOrWhiteSpace(tf.Value))
            {
                changed |= Set(tf, tf.Value.Replace("-windows", ""));
            }
        }

        // Fix TargetFrameworks (semicolon-separated)
        foreach (var tfs in doc.Descendants(ns + "TargetFrameworks"))
        {
            if (!string.IsNullOrWhiteSpace(tfs.Value))
            {
                changed |= Set(tfs, string.Join(";",
                    tfs.Value
                        .Split(';').Where(i=>!string.IsNullOrEmpty(i))
                        .Select(v => v.Replace("-windows", ""))
                ));
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
                changed |= Set(uwf, "false");
            }
        }

        if (changed)
        {
            doc.Save(projectFullPath);
        }

        return changed;
    }

    /// <summary>Assigns <paramref name="value" /> to <paramref name="element" />, reporting whether it differed.</summary>
    private static bool Set(XElement element, string value)
    {
        if (element.Value == value)
            return false;

        element.Value = value;
        return true;
    }
}
