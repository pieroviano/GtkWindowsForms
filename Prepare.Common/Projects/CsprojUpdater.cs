using System.Xml.Linq;

namespace Prepare.Common.Projects;

public static class CsprojUpdater
{
    private static readonly string[] WarningsToAdd =
    {
        "CS0067",
        "CS0169",
        "CS0419",
        "CS0436"
    };

    public static void EnsureNoWarn(string csprojPath)
    {
        if (!File.Exists(csprojPath))
            throw new FileNotFoundException("csproj not found", csprojPath);

        var doc = XDocument.Load(csprojPath);
        var project = doc.Root;

        var firstPropertyGroup = project?
            .Elements("PropertyGroup")
            .FirstOrDefault();

        if (firstPropertyGroup == null)
            return;

        var noWarnElement = firstPropertyGroup.Element("NoWarn");

        if (noWarnElement == null)
        {
            var warnings = "$(NoWarn);" + string.Join(";", WarningsToAdd);
            firstPropertyGroup.Add(new XElement("NoWarn", warnings));
        }
        else
        {
            var existing = noWarnElement.Value
                .Split(';').Where(i=>!string.IsNullOrEmpty(i))
                .Select(x => x.Trim())
                .ToList();

            bool changed = false;

            foreach (var warning in WarningsToAdd)
            {
                if (!existing.Contains(warning))
                {
                    existing.Add(warning);
                    changed = true;
                }
            }

            if (changed)
            {
                if (!existing.Contains("$(NoWarn)"))
                    existing.Insert(0, "$(NoWarn)");

                noWarnElement.Value = string.Join(";", existing);
            }
        }

        doc.Save(csprojPath);
    }
}
