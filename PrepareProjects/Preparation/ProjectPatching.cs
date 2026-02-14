using System.Text.RegularExpressions;
using System.Xml.Linq;
using Prepare.Common;

namespace PrepareProjects.Preparation;

public static class ProjectPatching
{
    public static string PatchProjectNamespace(string sourceProjectName,
        string? targetProjectName, string fileContent)
    {
        if (!sourceProjectName.StartsWith("GTKSystem."))
        {
            fileContent = fileContent.ReplaceIgnoreCase(sourceProjectName, targetProjectName);
        }
        else
        {
            fileContent = fileContent.ReplaceIgnoreCase(sourceProjectName,
                sourceProjectName.Replace("GTKSystem.", "GtkSystem."));
        }

        return fileContent;
    }

    public static string PatchProject(string targetProjectDirectory, string csprojPath, Action<string> CopyResources)
    {
        CopyContentAndResources(targetProjectDirectory);
        CopyPropsFile(targetProjectDirectory);
        CopyTargetsFile(targetProjectDirectory);

        PatchResourceManagerFile(targetProjectDirectory);

        CopyResources?.Invoke(targetProjectDirectory);

        UpdateNoneItems(csprojPath);
        UpdatePackageMetadata(csprojPath);
        UpdateCompileRemovals(csprojPath);
        ForceNetstandard(csprojPath);
        EnsureResxProcessorInNugetPackageAsTool(csprojPath);
        EnsureLocalizedResourcesItemsInProject(csprojPath);
        return csprojPath;
    }

    private static void AddNoneIfMissing(
        XElement itemGroup,
        string include,
        string link,
        XNamespace ns)
    {
        var exists = itemGroup
            .Elements(ns + "None")
            .Any(c => (string?)c.Attribute("Include") == include);

        if (!exists)
        {
            itemGroup.Add(
                new XElement(ns + "None",
                    new XAttribute("Include", include),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", link)));
        }
    }

    private static void EnsurePackageReference(
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


    private static void CopyContentAndResources(string projectDirectory)
    {
        var contentDir = Path.Combine(projectDirectory, "Content");
        Directory.CreateDirectory(contentDir);

        var resourcesDirectory = Path.GetFullPath(
            @"..\..\..\Resources");

        CopyIfExists(
            Path.Combine(resourcesDirectory, "System.ComponentModel.cs"),
            Path.Combine(contentDir, "System.ComponentModel.cs"));

        CopyIfExists(
            Path.Combine(resourcesDirectory, "System.Resources.ResourceManager.cs"),
            Path.Combine(contentDir, "System.Resources.ResourceManager.cs"));
    }

    private static void CopyIfExists(string sourcePath, string destinationPath)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                "Required resource file not found.",
                sourcePath);
        }

        File.Copy(sourcePath, destinationPath, true);
    }

    private static void CopyPropsFile(string targetDir)
    {
        var resourcesDir = Path.GetFullPath(
            @"..\..\..\Resources");

        var sourceProps = Path.Combine(
            resourcesDir,
            "Net4x.Gtk.Windows.Forms.props");

        var targetProps = Path.Combine(
            targetDir,
            "Net4x.Gtk.Windows.Forms.props");

        CopyIfExists(sourceProps, targetProps);
    }

    private static void CopyTargetsFile(string targetDir)
    {
        var resourcesDir = Path.GetFullPath(
            @"..\..\..\Resources");

        var sourceProps = Path.Combine(
            resourcesDir,
            "Net4x.Gtk.Windows.Forms.targets");

        var targetProps = Path.Combine(
            targetDir,
            "Net4x.Gtk.Windows.Forms.targets");

        CopyIfExists(sourceProps, targetProps);
    }

    private static void EnsureCompileRemove(
        XElement itemGroup,
        string removePath,
        XNamespace ns)
    {
        var exists = itemGroup
            .Elements(ns + "Compile")
            .Any(c => (string?)c.Attribute("Remove") == removePath);

        if (!exists)
        {
            itemGroup.Add(
                new XElement(ns + "Compile",
                    new XAttribute("Remove", removePath),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", @"content\")));
        }
    }

    private static void EnsureLocalizedResourcesItemsInProject(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Find last ItemGroup
        var itemGroup = doc
            .Root!
            .Elements(ns + "ItemGroup")
            .LastOrDefault();

        if (itemGroup == null)
        {
            return;
        }

        // ------------------------------------------------------------
        // Compile: Resources.Designer.cs
        // ------------------------------------------------------------
        var hasDesignerCompile = itemGroup
            .Elements(ns + "Compile")
            .Any(e => (string?)e.Attribute("Update") ==
                      @"Properties\Resources.Designer.cs");

        if (!hasDesignerCompile)
        {
            itemGroup.Add(
                new XElement(ns + "Compile",
                    new XAttribute("Update", @"Properties\Resources.Designer.cs"),
                    new XElement(ns + "DesignTime", "True"),
                    new XElement(ns + "AutoGen", "True"),
                    new XElement(ns + "DependentUpon", "Resources.resx")
                ));
        }

        // ------------------------------------------------------------
        // EmbeddedResource: localized resx files
        // ------------------------------------------------------------
        string[] cultures =
        {
            "it",
            "es",
            "de",
            "fr",
            "zh-Hans"
        };

        foreach (var culture in cultures)
        {
            var updatePath = $@"Properties\Resources.{culture}.resx";

            var exists = itemGroup
                .Elements(ns + "EmbeddedResource")
                .Any(e => (string?)e.Attribute("Update") == updatePath);

            if (exists)
            {
                continue;
            }

            itemGroup.Add(
                new XElement(ns + "EmbeddedResource",
                    new XAttribute("Update", updatePath),
                    new XElement(ns + "Generator", "ResXFileCodeGenerator"),
                    new XElement(ns + "DependentUpon", "Resources.resx")
                ));
        }

        doc.Save(csprojPath);
    }

    private static void EnsureResxProcessorInNugetPackageAsTool(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string include =
            @"..\Net4x.ResxProcessor\bin\$(Configuration)\net8.0\**\*.*";

        // Find the *last* ItemGroup
        var lastItemGroup = doc
            .Root!
            .Elements(ns + "ItemGroup")
            .LastOrDefault();

        if (lastItemGroup == null)
        {
            return;
        }

        var exists = lastItemGroup
            .Elements(ns + "None")
            .Any(n => (string?)n.Attribute("Include") == include);

        if (!exists)
        {
            lastItemGroup.Add(
                new XElement(ns + "None",
                    new XAttribute("Include", include),
                    new XAttribute(
                        "Link",
                        @"tools/%(RecursiveDir)%(Filename)%(Extension)"),
                    new XAttribute("Pack", "True"),
                    new XAttribute("PackagePath", "tools/")));
        }

        doc.Save(csprojPath);
    }

    private static void ForceNetstandard(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var tf = doc
            .Descendants(ns + "TargetFrameworks")
            .FirstOrDefault();

        if (tf != null)
        {
            tf.Value = "netstandard2.0";
        }

        doc.Save(csprojPath);
    }

    private static void PatchResourceManagerFile(string targetDir)
    {
        var filePath = Path.Combine(
            targetDir,
            @"GTKSystem\Resources\ResourceManager.cs");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "ResourceManager.cs not found.",
                filePath);
        }

        var text = File.ReadAllText(filePath);

        var regex = new Regex(
            @"filepath\s*=\s*[^;]+",
            RegexOptions.Multiline);

        var replacement =
            @"filepath = $""./{ReadResourceData(""this_FileName"")}""";

        var newText = regex.Replace(text, replacement, 1);

        if (!ReferenceEquals(text, newText) && text != newText)
        {
            File.WriteAllText(filePath, newText);
        }
    }

    private static void SetElementValue(XElement parent, XName name, string value)
    {
        var element = parent.Element(name);
        if (element != null)
        {
            element.Value = value;
        }
        else
        {
            parent.Add(new XElement(name, value));
        }
    }

    private static void UpdateCompileRemovals(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Find the first ItemGroup that already contains a Compile with Remove=""
        var itemGroup = doc
            .Root!
            .Elements(ns + "ItemGroup")
            .FirstOrDefault(ig =>
                ig.Elements(ns + "Compile")
                    .Any(c => c.Attribute("Remove") != null));

        if (itemGroup == null)
        {
            return;
        }

        // --- Ensure Compile Remove entries ---
        EnsureCompileRemove(
            itemGroup,
            @"Content\System.ComponentModel.cs",
            ns);

        EnsureCompileRemove(
            itemGroup,
            @"Content\System.Resources.ResourceManager.cs",
            ns);

        // --- Ensure Compile Update for Resources.Designer.cs ---
        var hasResourcesUpdate = itemGroup
            .Elements(ns + "Compile")
            .Any(c => (string?)c.Attribute("Update") == @"Properties\Resources.Designer.cs");

        if (!hasResourcesUpdate)
        {
            itemGroup.Add(
                new XElement(ns + "Compile",
                    new XAttribute("Update", @"Properties\Resources.Designer.cs"),
                    new XElement(ns + "DesignTime", "True"),
                    new XElement(ns + "AutoGen", "True"),
                    new XElement(ns + "DependentUpon", "Resources.resx")
                ));
        }

        // --- Ensure Content items ---
        AddNoneIfMissing(
            itemGroup,
            @"Content\System.ComponentModel.cs",
            @"content\System.ComponentModel.cs",
            ns);

        AddNoneIfMissing(
            itemGroup,
            @"Content\System.Resources.ResourceManager.cs",
            @"content\System.Resources.ResourceManager.cs",
            ns);

        // --- Ensure props file is packed into build folder ---
        var hasPropsNone = itemGroup
            .Elements(ns + "None")
            .Any(n => (string?)n.Attribute("Include") ==
                      "Net4x.Gtk.Windows.Forms.props");

        if (!hasPropsNone)
        {
            itemGroup.Add(
                new XElement(ns + "None",
                    new XAttribute(
                        "Include",
                        "Net4x.Gtk.Windows.Forms.props"),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", @"\build\")));
        }

        // --- Ensure props file is packed into build folder ---
        var hasTargetsNone = itemGroup
            .Elements(ns + "None")
            .Any(n => (string?)n.Attribute("Include") ==
                      "Net4x.Gtk.Windows.Forms.targets");

        if (!hasTargetsNone)
        {
            itemGroup.Add(
                new XElement(ns + "None",
                    new XAttribute(
                        "Include",
                        "Net4x.Gtk.Windows.Forms.targets"),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", @"\build\")));
        }

        doc.Save(csprojPath);
    }

    private static void UpdateNoneItems(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string oldLicenseInclude = @"..\licenses\LICENSE.txt";
        const string newLicenseInclude =
            @"..\gtksystem-windows-forms\Source\licenses\LICENSE.txt";

        const string oldReadmeInclude = @"..\..\README.md";
        const string newReadmeInclude =
            @"..\gtksystem-windows-forms\README.md";

        // --- Update LICENSE.txt ---
        var licenseNone = doc
            .Descendants(ns + "None")
            .FirstOrDefault(e => (string?)e.Attribute("Include") == oldLicenseInclude);

        if (licenseNone != null)
        {
            licenseNone.SetAttributeValue("Include", newLicenseInclude);
        }

        // --- Update README.md ---
        var readmeNone = doc
            .Descendants(ns + "None")
            .FirstOrDefault(e => (string?)e.Attribute("Include") == oldReadmeInclude);

        if (readmeNone == null)
        {
            throw new InvalidOperationException(
                "Expected README.md <None> item not found in csproj.");
        }

        readmeNone.SetAttributeValue("Include", newReadmeInclude);

        doc.Save(csprojPath);
    }

    private static void UpdatePackageMetadata(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var propertyGroup = doc
            .Root!
            .Elements(ns + "PropertyGroup")
            .FirstOrDefault();

        if (propertyGroup == null)
        {
            throw new InvalidOperationException("No PropertyGroup found in csproj.");
        }

        // --- Packaging / build settings ---
        SetElementValue(
            propertyGroup,
            ns + "PackageOutputPath",
            @"$(SolutionDir)Packages/");

        SetElementValue(
            propertyGroup,
            ns + "GeneratePackageOnBuild",
            "True");

        SetElementValue(
            propertyGroup,
            ns + "LangVersion",
            "latest");

        SetElementValue(
            propertyGroup,
            ns + "EnableUnsafeBinaryFormatterSerialization",
            "true");

        SetElementValue(
            propertyGroup,
            ns + "SignAssembly",
            "true");

        SetElementValue(
            propertyGroup,
            ns + "AssemblyOriginatorKeyFile",
            @"..\GtkForms.snk");

        SetElementValue(
            propertyGroup,
            ns + "PackageProjectUrl",
            "https://bitbucket.org/piero-viano/net4x.gtk.windows.forms");

        SetElementValue(
            propertyGroup,
            ns + "PackageReleaseNotes",
            @"
C# interface cross-platform development component gtk-windows-forms
".Trim());

        SetElementValue(
            propertyGroup,
            ns + "Title",
            "GTK cross-platform winform interface component, compiled once, can run on windows, linux, macos platforms");

        SetElementValue(
            propertyGroup,
            ns + "Description",
            @"
It is a cross-platform interface component developed based on GTK components and compatible with the native C# control winform interface.
Use this component GtkSystem.Drawing to compile once and run across platforms windows\linux\macos.
Update platform:
https://pieroviano@bitbucket.org/piero-viano/net4x.gtk.windows.forms.git

Based on GTKSystem.Windows.Forms:
 https://www.gtkapp.com；
 https://gitee.com/easywebfactory/gtksystem-windows-forms；
 https://github.com/easywebfactory/gtksystem-windows-forms；
".Trim());

        doc.Save(csprojPath);
    }
}