using Prepare.Common.Content;

namespace PrepareProjects.Preparation.Source;

public static class FixService
{
    public static void FixClasses(string csprojPath)
    {
        FixTableLayoutPanelReference(csprojPath);

        FixClass(csprojPath, Path.Combine(
            "GtkSystem",
            "Windows",
            "Forms",
            "GTKControls",
            "ControlBase",
            "TableLayoutPanelBase.cs"));

        FixClass(csprojPath, Path.Combine(
            "System",
            "Windows",
            "Forms",
            "TreeView.cs"));
    }

    private static void FixClass(string csprojPath, string relativePath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;
        var fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            return;
        }

        var content = File.ReadAllText(fullPath);

        if (!content.Contains("GTKControls"))
        {
            return;
        }

        content = content.Replace("GTKControls", "GtkControls");

        CsFile.WriteAllText(fullPath, content);
    }

    private static void FixTableLayoutPanelReference(string csprojPath)
    {
        var projectDirectory = Path.GetDirectoryName(csprojPath)!;

        var relativePath = Path.Combine("System", "Windows", "Forms", "TableLayoutPanel.cs");
        var fullPath = Path.Combine(projectDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            return;
        }

        var content = File.ReadAllText(fullPath);

        var modified = false;

        // 1️⃣ Replace GTKControls -> GtkControls
        if (content.Contains("GTKControls"))
        {
            content = content.Replace("GTKControls", "GtkControls");
            modified = true;
        }

        if (modified)
        {
            CsFile.WriteAllText(fullPath, content);
        }
    }
}
