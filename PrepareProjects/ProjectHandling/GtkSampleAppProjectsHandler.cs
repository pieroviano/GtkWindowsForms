using System.Reflection;
using System.Xml.Linq;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Languages;
using Prepare.Common.Logging;
using PrepareProjects.Preparation.Source;

namespace PrepareProjects.ProjectHandling;

public static class GtkSampleAppProjectsHandler
{
    public static async Task<string> PrepareAsync(string? sourceProjectName, string? targetProjectName, string version)
    {
        var sourceDir = Path.GetFullPath(
            @$"..\..\..\..\gtksystem-windows-forms\Samples\{sourceProjectName}");

        var targetDir = Path.GetFullPath(
            @$"..\..\..\..\{targetProjectName}");

        ProjectUtility.PrepareEmptyDirectory(targetDir);
        ProjectUtility.CopyDirectory(sourceDir, targetDir);

        ReplaceTestDataJson(targetDir);

        DeleteIfExists(Path.Combine(targetDir, "工程说明.txt"));
        CopyIfExists(Path.Combine(targetDir, "Resources", "010.jpg"), Path.Combine(targetDir, "Resources", "img11.jpg"));

        ProjectUtility.CopyLocalizationResourcesFiles(Path.Combine(targetDir, "Properties"),
            "GtkWinFormsAppResources.xml", "GtkWinFormsAppResources.zh-Hans.xml", "TestData1.json", "TestData1.en.json");

        var csprojPath = RenameProjectFiles(targetDir, sourceProjectName, targetProjectName);
        await ProjectUtility.EnsureLatestGtkSharpAsync(csprojPath);

        EnsureStrongNameSigning(csprojPath);
        EnsureResxEmbeddedResources(csprojPath);
        EnsureResources(csprojPath);

        UpdateLinkedCompileItems(csprojPath);
        UpdateProjectReferences(csprojPath, sourceProjectName, targetProjectName);
        UpdateNoneItems(csprojPath);

        EnsureResourcesDesignerCompileItem(csprojPath);
        EnsurePrepareBuildTarget(csprojPath);

        CopyAndFormatGtkWinFormsDesigner(csprojPath, targetDir);

        EnsureGtkWinFormsResourcesDesignerCompileItem(csprojPath);
        EnsureGtkWinFormsResourcesEmbedded(csprojPath);
        EnsureResourcesResxDesigner(csprojPath);
        EnsureJsonNoneItems(csprojPath);

        ProjectTranslation.TranslateFiles(sourceProjectName, targetProjectName, targetDir,
            ProjectPatching.PatchProjectNamespace, "GtkWinFormsAppResources", "samplesTranslations");
        var path = Path.Combine(targetDir, "Form1.cs");
        if (File.Exists(path))
        {
            var text = await File.ReadAllTextAsync(path);
            text = text.Replace("1、数据集列表数据源", "1. Dataset list data source").Replace("2、datatable数据源", "2. Datatable data source");
            await CsFile.WriteAllTextAsync(path, text);
        }
        path = Path.Combine(targetDir, "Form3.cs");
        if (File.Exists(path))
        {
            var text = await File.ReadAllTextAsync(path);
            text = text.Replace("打印", " Print");
            await CsFile.WriteAllTextAsync(path, text);
        }

        ConsoleColor.White.Write("Copy completed, project updated.", true);
#pragma warning disable CS8604 // Possible null reference argument.
        return $"{Path.Combine(targetDir, targetProjectName)}.csproj";
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void ReplaceTestDataJson(string targetDir)
    {
        foreach (var testData in new String[] { "TestData1.json", "TestData1.en.json" })
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var sourceFileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\..\Resources", testData));
            var destFileName = Path.Combine(targetDir, testData);
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            LogUtility.FileCopyAndLog(sourceFileName, destFileName);
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }

    private static void CopyAndFormatGtkWinFormsDesigner(
        string csprojPath,
        string targetDir)
    {
        var projectName =
            Path.GetFileNameWithoutExtension(csprojPath);

        var resourcesDir = Path.GetFullPath(
            @"..\..\..\..\Resources");

        var sourcePath = Path.Combine(
            resourcesDir,
            "GtkWinFormsAppResources.Designer.cs");

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                "Required Designer file not found.",
                sourcePath);
        }

        var targetPath = Path.Combine(
            targetDir,
            "Properties",
            "GtkWinFormsAppResources.Designer.cs");

        Directory.CreateDirectory(
            Path.GetDirectoryName(targetPath)!);

        var template = File.ReadAllText(sourcePath);
        template = template.Replace("{0}", "$Dollar0$Dollar").Replace("{", "{{").Replace("}", "}}").Replace("$Dollar0$Dollar", "{0}");

        var formattedContent =
            string.Format(template, projectName);

        CsFile.WriteAllText(targetPath, formattedContent);
    }

    private static void EnsureResourcesResxDesigner(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string updateValue = @"Properties\Resources.resx";

        // Check if it already exists
        var existing = doc
            .Descendants(ns + "EmbeddedResource")
            .FirstOrDefault(e =>
                (string?)e.Attribute("Update") == updateValue &&
                e.Element(ns + "Generator")?.Value == "ResXFileCodeGenerator" &&
                e.Element(ns + "LastGenOutput")?.Value == "Resources.Designer.cs");

        if (existing != null)
            return;

        var itemGroup =
            new XElement(ns + "ItemGroup",
                new XElement(ns + "EmbeddedResource",
                    new XAttribute("Update", updateValue),
                    new XElement(ns + "Generator", "ResXFileCodeGenerator"),
                    new XElement(ns + "LastGenOutput", "Resources.Designer.cs")
                )
            );

        // Append at the very end of the root element
        doc.Root!.Add(itemGroup);

        doc.Save(csprojPath);
    }


    private static void EnsureGtkWinFormsResourcesEmbedded(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string mainResx =
            @"Properties\GtkWinFormsAppResources.resx";
        const string zhHansResx =
            @"Properties\GtkWinFormsAppResources.zh-Hans.resx";

        // If the main resource is already configured, assume the group exists
        var exists = doc
            .Descendants(ns + "EmbeddedResource")
            .Any(e => (string?)e.Attribute("Update") == mainResx);

        if (exists)
        {
            return;
        }

        var itemGroup =
            new XElement(ns + "ItemGroup",

                new XElement(ns + "EmbeddedResource",
                    new XAttribute("Update", mainResx),
                    new XElement(ns + "Generator", "ResXFileCodeGenerator"),
                    new XElement(ns + "LastGenOutput",
                        "GtkWinFormsAppResources.Designer.cs")
                ),

                new XElement(ns + "EmbeddedResource",
                    new XAttribute("Update", zhHansResx),
                    new XElement(ns + "Generator", "ResXFileCodeGenerator"),
                    new XElement(ns + "DependentUpon",
                        "GtkWinFormsAppResources.resx")
                )
            );

        // Append at the very end of the project
        doc.Root!.Add(itemGroup);

        doc.Save(csprojPath);
    }

    private static void CopyIfExists(string path, string target)
    {
        if (File.Exists(path))
        {
            LogUtility.FileCopyAndLog(path, target);
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void EnsurePrepareBuildTarget(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string targetName = "PrepareBuild";

        // Check if the Target already exists
        var existingTarget = doc
            .Root!
            .Elements(ns + "Target")
            .FirstOrDefault(t => (string?)t.Attribute("Name") == targetName);

        if (existingTarget != null)
        {
            return;
        }

        var target = new XElement(ns + "Target",
            new XAttribute("Name", targetName),
            new XAttribute("BeforeTargets", "BeforeBuild"),
            new XElement(ns + "Message",
                new XAttribute("Importance", "High"),
                new XAttribute(
                    "Text",
                    "\"$(MSBuildThisFileDirectory)..\\Net4x.ResxProcessor\\bin\\$(Configuration)\\net8.0\\Net4x.XUnit.Copy.exe\" $(TargetFramework) \"$(TargetDir)\" \"$(MSBuildProjectFullPath)\"")),
            new XElement(ns + "Exec",
                new XAttribute(
                    "Command",
                    "\"$(MSBuildThisFileDirectory)..\\Net4x.ResxProcessor\\bin\\$(Configuration)\\net8.0\\Net4x.ResxProcessor.exe\" \"$(MSBuildProjectDirectory)\""))
        );

        // Append at the very end of the root element
        doc.Root!.Add(target);

        doc.Save(csprojPath);
    }

    private static void EnsureResourcesDesignerCompileItem(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string updatePath = @"Properties\Resources.Designer.cs";

        // Check if it already exists anywhere
        var exists = doc
            .Descendants(ns + "Compile")
            .Any(c => (string?)c.Attribute("Update") == updatePath);

        if (exists)
        {
            return;
        }

        var itemGroup =
            new XElement(ns + "ItemGroup",
                new XElement(ns + "Compile",
                    new XAttribute("Update", updatePath),
                    new XElement(ns + "DesignTime", "True"),
                    new XElement(ns + "AutoGen", "True"),
                    new XElement(ns + "DependentUpon", "Resources.resx")
                )
            );

        // Add as the last child of the project root
        doc.Root!.Add(itemGroup);

        doc.Save(csprojPath);
    }

    private static void EnsureResources(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string updateValue = @"Resources\*.*";

        // Check if it already exists
        var existing = doc
            .Descendants(ns + "None")
            .FirstOrDefault(e =>
                (string?)e.Attribute("Update") == updateValue &&
                e.Element(ns + "CopyToOutputDirectory")?.Value == "PreserveNewest");

        if (existing != null)
        {
            return;
        }

        var itemGroup = new XElement(ns + "ItemGroup",
            new XElement(ns + "None",
                new XAttribute("Update", updateValue),
                new XElement(ns + "CopyToOutputDirectory", "PreserveNewest")));

        doc.Root!.Add(itemGroup);
        doc.Save(csprojPath);
    }

    private static void EnsureResxEmbeddedResources(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string updateValue = @"**\*.resx";

        // Check if it already exists
        var existing = doc
            .Descendants(ns + "EmbeddedResource")
            .FirstOrDefault(e =>
                (string?)e.Attribute("Update") == updateValue &&
                e.Element(ns + "CopyToOutputDirectory")?.Value == "PreserveNewest");

        if (existing != null)
        {
            return;
        }

        var itemGroup = new XElement(ns + "ItemGroup",
            new XElement(ns + "EmbeddedResource",
                new XAttribute("Update", updateValue),
                new XElement(ns + "CopyToOutputDirectory", "PreserveNewest")));

        doc.Root!.Add(itemGroup);
        doc.Save(csprojPath);
    }

    private static void EnsureStrongNameSigning(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var firstPropertyGroup = doc
            .Root!
            .Elements(ns + "PropertyGroup")
            .FirstOrDefault();

        if (firstPropertyGroup == null)
        {
            return;
        }

        if (!firstPropertyGroup.Elements(ns + "SignAssembly").Any())
        {
            firstPropertyGroup.Add(
                new XElement(ns + "SignAssembly", "true"));
        }

        if (!firstPropertyGroup.Elements(ns + "AssemblyOriginatorKeyFile").Any())
        {
            firstPropertyGroup.Add(
                new XElement(
                    ns + "AssemblyOriginatorKeyFile",
                    @"..\GtkForms.snk"));
        }

        doc.Save(csprojPath);
    }

    private static string GetFullProjectReferencePath(
        string projectDir,
        string include)
    {
        return Path.GetFullPath(
            Path.Combine(projectDir, include.Trim()));
    }

    private static string NormalizeVersion(string version)
    {
        var parts = version.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
        {
            return version;
        }

        return string.Join('.', parts.Take(3)) + ".*";
    }

    private static void RenameIfExists(string sourcePath, string targetPath)
    {
        if (File.Exists(sourcePath))
        {
            File.Move(sourcePath, targetPath, true);
        }
    }

    private static string RenameProjectFiles(string targetDir, string? sourceProjectName, string? targetProjectName)
    {
        var oldCsproj = Path.Combine(targetDir, $"{sourceProjectName}.csproj");
        var newCsproj = Path.Combine(targetDir, $"{targetProjectName}.csproj");

        RenameIfExists(oldCsproj, newCsproj);

        RenameIfExists(
            Path.Combine(targetDir, $"{sourceProjectName}.csproj.user"),
            Path.Combine(targetDir, $"{targetProjectName}.csproj.user"));

        return newCsproj;
    }

    private static void UpdateLinkedCompileItems(string csprojPath)
    {
        var projectDir = Path.GetDirectoryName(csprojPath)!;

        // --- Delete local files if they exist ---
        DeleteIfExists(Path.Combine(projectDir, "System.ComponentModel.cs"));
        DeleteIfExists(Path.Combine(projectDir, "System.Resources.ResourceManager.cs"));

        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Check if Compile links already exist
        var hasComponentModel = doc
            .Descendants(ns + "Compile")
            .Any(e =>
                (string?)e.Attribute("Include") ==
                @"..\Net4x.Gtk.Windows.Forms\Content\System.ComponentModel.cs");

        var hasResourceManager = doc
            .Descendants(ns + "Compile")
            .Any(e =>
                (string?)e.Attribute("Include") ==
                @"..\Net4x.Gtk.Windows.Forms\Content\System.Resources.ResourceManager.cs");

        if (!hasComponentModel || !hasResourceManager)
        {
            var itemGroup = new XElement(ns + "ItemGroup");

            if (!hasComponentModel)
            {
                itemGroup.Add(
                    new XElement(ns + "Compile",
                        new XAttribute(
                            "Include",
                            @"..\Net4x.Gtk.Windows.Forms\Content\System.ComponentModel.cs"),
                        new XAttribute("Link", "System.ComponentModel.cs")));
            }

            if (!hasResourceManager)
            {
                itemGroup.Add(
                    new XElement(ns + "Compile",
                        new XAttribute(
                            "Include",
                            @"..\Net4x.Gtk.Windows.Forms\Content\System.Resources.ResourceManager.cs"),
                        new XAttribute("Link", "System.Resources.ResourceManager.cs")));
            }

            doc.Root!.Add(itemGroup);
        }

        doc.Save(csprojPath);
    }

    private static void UpdateNoneItems(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string readmeEnInclude =
            @"..\PrepareProjects\Resources\ProjectDescription.txt";

        var readmeNone = doc
            .Descendants(ns + "Content")
            .FirstOrDefault(e => (string?)e.Attribute("Include") == "TestData1.json");

        if (readmeNone != null)
        {
            readmeNone.AddAfterSelf(
                new XElement(ns + "Content",
                    new XAttribute("Include", readmeEnInclude),
                    new XAttribute("Link", "ProjectDescription.txt")));

            doc.Save(csprojPath);
        }
    }

    private static void UpdateProjectReferences(
        string csprojPath,
        string? sourceProjectName,
        string? targetProjectName)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var oldInclude =
            @$"..\..\Source\GTKSystem.Windows.Forms\{sourceProjectName}.csproj";

        var newInclude =
            @"..\Net4x.Gtk.Windows.Forms\Net4x.Gtk.Windows.Forms.csproj";

        const string designerPackageId = "GTKSystem.Windows.FormsDesigner";
        const string gtkPackageId = "GTKSystem.Windows.Forms";

        const string resourcesExtensionsInclude =
            @"..\Net4x.System.Resources.Extensions\Net4x.System.Resources.Extensions.csproj";

        // ------------------------------------------------------------
        // 1. REMOVE unwanted PackageReferences
        // ------------------------------------------------------------
        doc.Descendants(ns + "PackageReference")
            .Where(pr =>
                (string?)pr.Attribute("Include") == designerPackageId ||
                (string?)pr.Attribute("Include") == gtkPackageId)
            .Remove();

        // ------------------------------------------------------------
        // 2. COLLECT all ProjectReferences
        // ------------------------------------------------------------
        var allProjectReferences = doc
            .Descendants(ns + "ProjectReference")
            .ToList();

        // Update old include → new include
        foreach (var pr in allProjectReferences)
        {
            if ((string?)pr.Attribute("Include") == oldInclude)
            {
                pr.SetAttributeValue("Include", newInclude);
            }
        }

        // Ensure required references exist
        if (!allProjectReferences.Any(pr => (string?)pr.Attribute("Include") == newInclude))
        {
            allProjectReferences.Add(
                new XElement(ns + "ProjectReference",
                    new XAttribute("Include", newInclude)));
        }

        if (!allProjectReferences.Any(pr =>
                (string?)pr.Attribute("Include") == resourcesExtensionsInclude))
        {
            allProjectReferences.Add(
                new XElement(ns + "ProjectReference",
                    new XAttribute("Include", resourcesExtensionsInclude)));
        }

        var elements = allProjectReferences
            .Where(pr =>
            {
                var include = (string?)pr.Attribute("Include");
                var endsWith = include != null &&
                               include.EndsWith(
                                   "GTKSystem.Windows.Forms.csproj",
                                   StringComparison.OrdinalIgnoreCase);
                return endsWith;
            })
            .ToList();
        foreach (var el in elements)
        {
            allProjectReferences.Remove(el);
        }

        // ------------------------------------------------------------
        // 3. REMOVE duplicate ProjectReferences (semantic path compare)
        // ------------------------------------------------------------
        var projectDir = Path.GetDirectoryName(csprojPath)!;

        // Normalize, group, pick shortest Include
        var normalizedProjectReferences =
            allProjectReferences
                .Select(pr =>
                {
                    var include = (string)pr.Attribute("Include")!;
                    var fullPath = GetFullProjectReferencePath(projectDir, include);
                    return new { Include = include, FullPath = fullPath };
                })
                .GroupBy(x => x.FullPath, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                    g.OrderBy(x => x.Include.Length).First())
                .ToList();

        // ------------------------------------------------------------
        // 4. RECREATE normalized ItemGroups
        // ------------------------------------------------------------

        // Remove ALL existing ProjectReference ItemGroups
        doc.Root!
            .Elements(ns + "ItemGroup")
            .Where(ig => ig.Elements(ns + "ProjectReference").Any())
            .Remove();

        // Create ONE clean ProjectReference ItemGroup
        var projectReferenceGroup = new XElement(ns + "ItemGroup");

        foreach (var pr in normalizedProjectReferences)
        {
            projectReferenceGroup.Add(
                new XElement(ns + "ProjectReference",
                    new XAttribute("Include", pr.Include)));
        }

        doc.Root!.Add(projectReferenceGroup);

        doc.Save(csprojPath);
    }

    private static void EnsureGtkWinFormsResourcesDesignerCompileItem(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        const string updatePath =
            @"Properties\GtkWinFormsAppResources.Designer.cs";

        // Check if it already exists anywhere
        var exists = doc
            .Descendants(ns + "Compile")
            .Any(c => (string?)c.Attribute("Update") == updatePath);

        if (exists)
        {
            return;
        }

        var itemGroup =
            new XElement(ns + "ItemGroup",
                new XElement(ns + "Compile",
                    new XAttribute("Update", updatePath),
                    new XElement(ns + "DesignTime", "True"),
                    new XElement(ns + "AutoGen", "True"),
                    new XElement(ns + "DependentUpon",
                        "GtkWinFormsAppResources.resx")
                )
            );

        // Append at the very end of the project root
        doc.Root!.Add(itemGroup);

        doc.Save(csprojPath);
    }

    private static void EnsureJsonNoneItems(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        // Check if the ItemGroup already exists (optional)
        var exists = doc
            .Descendants(ns + "None")
            .Any(e =>
                (string?)e.Attribute("Update") == @"Properties\TestData1.en.json" ||
                (string?)e.Attribute("Update") == @"Properties\TestData1.json");

        if (exists)
        {
            YellowConsole.Write("JSON None items already exist in the project file.");
            return;
        }

        // Create the ItemGroup
        var itemGroup = new XElement(ns + "ItemGroup",
            new XElement(ns + "None",
                new XAttribute("Update", @"Properties\TestData1.en.json"),
                new XElement(ns + "CopyToOutputDirectory", "PreserveNewest")
            ),
            new XElement(ns + "None",
                new XAttribute("Update", @"Properties\TestData1.json"),
                new XElement(ns + "CopyToOutputDirectory", "PreserveNewest")
            )
        );

        // Append at the end of the root element
        doc.Root!.Add(itemGroup);

        doc.Save(csprojPath);

        ConsoleColor.White.Write("JSON None items appended to the project file.");
    }

}
