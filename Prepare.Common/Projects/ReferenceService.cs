using System.Xml.Linq;

namespace Prepare.Common.Projects;

public static class ReferenceService
{
    public static void EnsurePackageReference(
        string csprojPath,
        string packageId,
        string version)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Find an ItemGroup that already has PackageReference(s)
        var itemGroup = doc
            .Root!
            .Elements(ns + "ItemGroup")
            .FirstOrDefault(ig => ig.Elements(ns + "PackageReference").Any());

        // If none exists, create a new ItemGroup
        if (itemGroup == null)
        {
            itemGroup = new XElement(ns + "ItemGroup");
            doc.Root!.Add(itemGroup);
        }

        var exists = itemGroup
            .Elements(ns + "PackageReference")
            .Any(pr => (string?)pr.Attribute("Include") == packageId);

        if (!exists)
        {
            itemGroup.Add(
                new XElement(ns + "PackageReference",
                    new XAttribute("Include", packageId),
                    new XAttribute("Version", version)));
        }

        doc.Save(csprojPath);
    }
    
    public static void EnsureProjectReference(
        string csprojPath,
        string path)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Find an ItemGroup that already has PackageReference(s)
        var itemGroup = doc
            .Root!
            .Elements(ns + "ItemGroup")
            .FirstOrDefault(ig => ig.Elements(ns + "ProjectReference").Any());

        // If none exists, create a new ItemGroup
        if (itemGroup == null)
        {
            itemGroup = new XElement(ns + "ItemGroup");
            doc.Root!.Add(itemGroup);
        }

        var exists = itemGroup
            .Elements(ns + "ProjectReference")
            .Any(pr => (string?)pr.Attribute("Include") == path);

        if (!exists)
        {
            itemGroup.Add(
                new XElement(ns + "ProjectReference",
                    new XAttribute("Include", path)));
        }

        doc.Save(csprojPath);
    }
}