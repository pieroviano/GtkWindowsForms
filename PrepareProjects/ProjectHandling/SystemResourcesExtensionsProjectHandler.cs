using System.Xml.Linq;
using Prepare.Common.IO;
using PrepareProjects.Preparation;

namespace PrepareProjects.ProjectHandling;

public static class SystemResourcesExtensionsProjectHandler
{
    public static void Prepare()
    {
        var targetDir = ProjectUtility.CopyProjectAndTranslateIt(
            "System.Resources.Extensions",
            "Net4x.System.Resources.Extensions", ProjectPatching.PatchProjectNamespace);

        var oldCsproj = Path.Combine(targetDir, "System.Resources.Extensions.csproj");
        var newCsproj = Path.Combine(targetDir, "Net4x.System.Resources.Extensions.csproj");

        File.Move(oldCsproj, newCsproj, true);

        UpdateCsproj(newCsproj);
    }

    private static void UpdateCsproj(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // 1. Set TargetFramework to netstandard2.0
        var targetFramework = doc
            .Descendants(ns + "TargetFramework")
            .FirstOrDefault();

        if (targetFramework != null)
        {
            targetFramework.Value = "netstandard2.0";
        }

        // 2. Remove ALL ItemGroup elements
        doc
            .Root!
            .Elements(ns + "ItemGroup")
            .Remove();

        // 3. Remove PropertyGroup elements with non-empty Condition
        doc
            .Root!
            .Elements(ns + "PropertyGroup")
            .Where(pg => !string.IsNullOrWhiteSpace((string?)pg.Attribute("Condition")))
            .Remove();

        // 4. Ensure AssemblyName in the first PropertyGroup
        var firstPropertyGroup = doc
            .Root!
            .Elements(ns + "PropertyGroup")
            .FirstOrDefault();

        if (firstPropertyGroup == null)
        {
            throw new InvalidOperationException("No PropertyGroup found in csproj.");
        }

        var assemblyName = firstPropertyGroup.Element(ns + "AssemblyName");

        if (assemblyName != null)
        {
            assemblyName.Value = "System.Resources.Extensions";
        }
        else
        {
            firstPropertyGroup.Add(
                new XElement(ns + "AssemblyName", "System.Resources.Extensions"));
        }

        doc.Save(csprojPath);
    }
}