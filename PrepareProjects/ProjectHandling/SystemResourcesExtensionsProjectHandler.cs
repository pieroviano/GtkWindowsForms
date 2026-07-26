using System.Xml.Linq;
using Prepare.Common.IO;
using PrepareProjects.Preparation.Source;

namespace PrepareProjects.ProjectHandling;

public static class SystemResourcesExtensionsProjectHandler
{
    /// <summary>
    /// The explanation carried in the generated csproj. The long form lives in the packaged
    /// <c>README.md</c>; this is the note a reader of the project file needs.
    /// </summary>
    private const string EmptyAssemblyComment =
        """
         The project "Gtk.Resources.Extensions" generates an **intentionally empty** assembly whose `AssemblyName`
             *is* `System.Resources.Extensions`, so that it displaces Microsoft's assembly of that name in an
             application's output folder. It carries no code and is not meant to: the working implementation lives
             in the Forms library under `GTKSystem/Resources/Extensions/`, in namespace
             `GtkSystem.Resources.Extensions`, so nothing needs the displaced types. The sample apps reference the
             project for that reason. (See README.md, which ships in the package, for the consumer-facing version.
             Note that displacement only takes effect if this assembly wins MSBuild's conflict resolution against
             the `System.Resources.Extensions` NuGet package)
        """;

    /// <summary>The README overlaid into the project and packed into the NuGet package.</summary>
    private const string ReadmeResourceName = "SystemResourcesExtensions.README.md";

    /// <summary>
    /// Assembly version that wins MSBuild's conflict resolution against Microsoft's
    /// <c>System.Resources.Extensions</c> (4.0.0.0), which is what makes the displacement actually
    /// happen. Deliberately far above anything Microsoft is likely to ship; the NuGet package version
    /// is unaffected.
    /// </summary>
    private const string DisplacementAssemblyVersion = "99.0.0.0";

    /// <summary>
    /// Package metadata for <c>Gtk.Resources.Extensions</c>, in the order it is written. Upstream's
    /// csproj has none of it: without these the project builds a library nobody packs, and packing it by
    /// hand yields NuGet's placeholder author and description.
    /// </summary>
    private static readonly (string Name, string Value)[] PackagingProperties =
    {
        ("PackageId", "Gtk.Resources.Extensions"),
        ("PackageReadmeFile", "README.md"),
        ("Authors", "Piero Viano"),
        ("Company", "Piero Viano"),
        ("Copyright", "Copyright (c) Piero Viano. All right reserved."),
        // Without an explicit Version the SDK reads $(VersionSuffix) as a prerelease label and packs
        // 1.3.24-26204 instead of 1.3.24.26204, unlike every other package in the solution.
        ("Version", "$(VersionPrefix).$(VersionSuffix)"),
        ("Title", "Empty System.Resources.Extensions that displaces Microsoft's for Gtk.Windows.Forms"),
        ("Description",
            "Companion package of Gtk.Windows.Forms. Ships an intentionally empty assembly named " +
            "System.Resources.Extensions so that it displaces Microsoft's BinaryFormatter-based assembly of " +
            "that name in an application's output folder; the ResX reader Gtk.Windows.Forms actually uses " +
            "lives in that library, under GtkSystem.Resources.Extensions. See README.md."),
        ("PackageProjectUrl", "https://bitbucket.org/piero-viano/net4x.gtk.windows.forms"),
        ("RepositoryUrl", "https://bitbucket.org/piero-viano/net4x.gtk.windows.forms"),
        ("PackageLicenseFile", "LICENSE.txt"),
        ("PackageRequireLicenseAcceptance", "True"),
        ("PackAsTool", "False"),
        ("GeneratePackageOnBuild", "True"),
        // Packages/ is where the pipeline's publish step looks; bin\ would never be pushed.
        ("PackageOutputPath", "$(SolutionDir)Packages/"),
    };

    public static void Prepare(string net4xSystemResourcesExtensionsCsproj)
    {
        var targetDir = ProjectUtility.CopyProjectAndTranslateIt(
            "System.Resources.Extensions",
            "Gtk.Resources.Extensions", ProjectPatching.PatchProjectNamespace);

        var oldCsproj = Path.Combine(targetDir, "System.Resources.Extensions.csproj");
        var newCsproj = Path.Combine(targetDir, net4xSystemResourcesExtensionsCsproj);

        File.Move(oldCsproj, newCsproj, true);

        // CopyProjectAndTranslateIt empties the project directory first, so the README has to be
        // overlaid from Resources\ on every run rather than left in place.
        LogUtility.FileCopyAndLog(
            RepoPaths.At(Path.Combine("Resources", ReadmeResourceName)),
            Path.Combine(targetDir, "README.md"));

        UpdateCsproj(newCsproj);
    }

    /// <summary>
    /// Rewrites the csproj copied from upstream into the shipped one: netstandard2.0, no upstream item
    /// groups, the displacing <c>AssemblyName</c>, and the packaging metadata (package id + README).
    /// Everything here is re-emitted on each run because the project directory is wiped first.
    /// </summary>
    public static void UpdateCsproj(string csprojPath)
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

        // 5. The doc file follows the assembly name, not the project name, so it stays
        //    System.Resources.Extensions.xml rather than upstream's $(MSBuildProjectName).xml.
        SetProperty(firstPropertyGroup, ns, "DocumentationFile",
            @"$(MSBuildProjectDirectory)\System.Resources.Extensions.xml");

        // 6. Displacement sentinel. MSBuild's conflict resolution keeps the highest AssemblyVersion
        //    for a simple name; at the package version (1.3.24.x) this assembly lost to Microsoft's
        //    System.Resources.Extensions (4.0.0.0) and the displacement silently did not happen.
        SetProperty(firstPropertyGroup, ns, "AssemblyVersion", DisplacementAssemblyVersion);
        SetProperty(firstPropertyGroup, ns, "FileVersion", DisplacementAssemblyVersion);

        // 6. Packaging: the note explaining the empty assembly, the metadata that turns this into a
        //    publishable package, and the files packed alongside the assembly.
        if (!firstPropertyGroup.Nodes().OfType<XComment>().Any())
        {
            firstPropertyGroup.Add(new XComment(EmptyAssemblyComment));
        }

        foreach (var (name, value) in PackagingProperties)
        {
            SetProperty(firstPropertyGroup, ns, name, value);
        }

        doc.Root!.Add(
            new XElement(ns + "ItemGroup",
                PackedFile(ns, "README.md"),
                PackedFile(ns, @"..\gtksystem-windows-forms\Source\licenses\LICENSE.txt")));

        doc.Save(csprojPath);
    }

    /// <summary>A <c>None</c> item packed at the package root.</summary>
    private static XElement PackedFile(XNamespace ns, string include) =>
        new(ns + "None",
            new XAttribute("Include", include),
            new XAttribute("Pack", "true"),
            new XAttribute("PackagePath", @"\"));

    private static void SetProperty(XElement propertyGroup, XNamespace ns, string name, string value)
    {
        var existing = propertyGroup.Element(ns + name);

        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            propertyGroup.Add(new XElement(ns + name, value));
        }
    }
}
