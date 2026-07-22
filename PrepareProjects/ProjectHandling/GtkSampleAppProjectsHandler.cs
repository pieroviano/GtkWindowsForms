using System.Reflection;
using System.Xml.Linq;
using Prepare.Common.Alignment;
using Prepare.Common.Content;
using Prepare.Common.IO;
using Prepare.Common.Languages;
using Prepare.Common.Logging;
using Prepare.Common.Projects;
using PrepareProjects.Preparation.Source;

namespace PrepareProjects.ProjectHandling;

public static class GtkSampleAppProjectsHandler
{
    /// <summary>The one sample that <c>GtkWinFormsApp.UITests</c> drives; its Designer members are widened and its assembly grants the test project internal access.</summary>
    private const string UiTestableSample = "GtkWinFormsApp";

    /// <summary>The UI-test project granted <c>InternalsVisibleTo</c> on <see cref="UiTestableSample"/>.</summary>
    private const string UiTestProjectName = "GtkWinFormsApp.UITests";

    /// <summary><c>GtkForms.snk</c>'s public key - the sample signs with this key, so the grant must name it.</summary>
    private const string GtkFormsPublicKey =
        "0024000004800000940000000602000000240000525341310004000001000100518ef14d1a0c9efc88ac393975" +
        "09c94a703ec7a674ed0414a18d3bad86b3618863d9d20204b50c28a13c56df0493d2943f2013a9b4f2bd5a82921" +
        "7698fa2444fbbebd449d6a8942a1278c87fe9ccba10535b9b2b1f47295168308a2b2a85af1fc655f8c736b51fe1" +
        "d457d1b7be6c807516f58d5d981b112929c5c24f3ff1c0ce";


    /// <summary>
    /// The sample transform <c>fs</c> for ONE sample: copy <c>Samples/&lt;source&gt;</c> → <c>&lt;target&gt;</c>,
    /// overlay the localization/data resources, rewrite the csproj, and translate + key the Chinese.
    /// Everything resolves through <see cref="RepoPaths"/>, so a caller that points the root at a staging
    /// tree (<see cref="SampleTransformPipeline"/>) never touches the live samples or submodule.
    /// </summary>
    public static async Task<string> PrepareAsync(string? sourceProjectName, string? targetProjectName,
        SampleTransformOptions? options = null)
    {
        options ??= SampleTransformOptions.Pipeline;

        var sourceDir = RepoPaths.At($@"gtksystem-windows-forms\Samples\{sourceProjectName}");

        var targetDir = RepoPaths.At($"{targetProjectName}");

        ProjectUtility.PrepareEmptyDirectory(targetDir);
        ProjectUtility.CopyDirectory(sourceDir, targetDir);

        // Let GtkWinFormsApp.UITests drive the sample's Designer controls without reflection: promote the
        // generated Designer fields from private to internal and (below) grant the test assembly access.
        // Applied to both alignment sides equally, so it is delta-neutral for AlignSamples.
        if (targetProjectName == UiTestableSample)
        {
            InternalizeDesignerFields(targetDir);
        }

        // Provenance: every generated file starts as a straight copy of its upstream counterpart; the
        // steps below overwrite the entries they redirect elsewhere (overlays, renames, deletions).
        var provenance = options.Provenance;
        MapCopiedTree(provenance, targetDir, sourceDir);

        ReplaceTestDataJson(targetDir, provenance);

        DeleteIfExists(Path.Combine(targetDir, "工程说明.txt"));
        provenance?.Remove(RepoRelative(Path.Combine(targetDir, "工程说明.txt")));
        CopyIfExists(Path.Combine(targetDir, "Resources", "010.jpg"), Path.Combine(targetDir, "Resources", "img11.jpg"));
        MapIfExists(provenance, Path.Combine(targetDir, "Resources", "img11.jpg"),
            Path.Combine(sourceDir, "Resources", "010.jpg"));

        var propertiesDir = Path.Combine(targetDir, "Properties");
        ProjectUtility.CopyLocalizationResourcesFiles(propertiesDir,
            "GtkWinFormsAppResources.xml", "GtkWinFormsAppResources.zh-Hans.xml", "TestData1.json", "TestData1.en.json");
        MapLocalizationOverlay(provenance, propertiesDir);

        var csprojPath = RenameProjectFiles(targetDir, sourceProjectName, targetProjectName, provenance, sourceDir);

        // Skipped under AlignSamples: this queries nuget.org for the newest GtkSharp and rewrites the
        // csproj, so the produced BASE/THEIRS trees would differ by whatever the feed served that minute -
        // network churn entering the alignment delta as if it were an upstream change. The live sample
        // keeps its pinned version; version normalization is OURS-side state.
        if (options.EnsureLatestGtkSharp)
        {
            await ProjectUtility.EnsureLatestGtkSharpAsync(csprojPath);
        }

        EnsureStrongNameSigning(csprojPath);
        if (targetProjectName == UiTestableSample)
        {
            EnsureInternalsVisibleTo(csprojPath);
        }

        EnsureResxEmbeddedResources(csprojPath);
        EnsureResources(csprojPath);

        UpdateLinkedCompileItems(csprojPath);
        provenance?.Remove(RepoRelative(Path.Combine(targetDir, "System.ComponentModel.cs")));
        provenance?.Remove(RepoRelative(Path.Combine(targetDir, "System.Resources.ResourceManager.cs")));
        UpdateProjectReferences(csprojPath, sourceProjectName, targetProjectName);

        EnsureResourcesDesignerCompileItem(csprojPath);
        EnsurePrepareBuildTarget(csprojPath);

        CopyAndFormatGtkWinFormsDesigner(csprojPath, targetDir);
        MapIfExists(provenance, Path.Combine(propertiesDir, "GtkWinFormsAppResources.Designer.cs"),
            RepoPaths.At(Path.Combine("Resources", "GtkWinFormsAppResources.Designer.cs")));

        EnsureGtkWinFormsResourcesDesignerCompileItem(csprojPath);
        EnsureGtkWinFormsResourcesEmbedded(csprojPath);
        EnsureResourcesResxDesigner(csprojPath);
        EnsureJsonNoneItems(csprojPath);

        // The former per-file `Form1.cs`/`Form3.cs` string fix-ups now live in
        // Resources/samplesTranslations.xml (keyed with their `//` comment prefix so the dictionary
        // reproduces them verbatim): dictionary-driven, manifest-visible, and applied to every sample
        // rather than two hardcoded paths. See AlignSamples-Plan.md Phase 1.
        ProjectTranslation.TranslateFiles(sourceProjectName, targetProjectName, targetDir,
            ProjectPatching.PatchProjectNamespace, "GtkWinFormsAppResources", "samplesTranslations");

        ConsoleColor.White.Write("Copy completed, project updated.", true);
#pragma warning disable CS8604 // Possible null reference argument.
        var projPath = $"{Path.Combine(targetDir, targetProjectName)}.csproj";
#pragma warning restore CS8604 // Possible null reference argument.
        CsprojUpdater.EnsureNoWarn(projPath);
        
        return projPath;
    }

    private static void ReplaceTestDataJson(string targetDir, ProvenanceManifest? provenance)
    {
        foreach (var testData in new[] { "TestData1.json", "TestData1.en.json" })
        {
            var sourceFileName = RepoPaths.At(Path.Combine("Resources", testData));
            var destFileName = Path.Combine(targetDir, testData);
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            LogUtility.FileCopyAndLog(sourceFileName, destFileName);
#pragma warning restore CS8604 // Possible null reference argument.
            MapIfExists(provenance, destFileName, sourceFileName);
        }
    }

    // --- provenance -------------------------------------------------------------------------------
    // generated file -> the file it actually came from, recorded as each copy/overlay/rename happens.
    // The alignment conflict pause reads this to name the exact upstream original of a conflicted file
    // (AlignSamples-Plan.md Phase 1); nothing else depends on it, so every hook is null-tolerant.

    /// <summary>
    /// Repo-relative, forward-slashed path - used for BOTH sides of a manifest entry. The source side is
    /// relative too because <c>fs</c> runs against a throwaway staging root: an absolute path there names
    /// a directory that is deleted before the operator ever reads the manifest. The alignment driver
    /// rebases relative sources onto the live repo, where the file really exists.
    /// </summary>
    private static string RepoRelative(string fullPath) =>
        Path.GetRelativePath(RepoPaths.Root, Path.GetFullPath(fullPath)).Replace('\\', '/');

    /// <summary>Maps every file just copied into <paramref name="targetDir"/> back to its counterpart under the upstream sample.</summary>
    private static void MapCopiedTree(ProvenanceManifest? provenance, string targetDir, string sourceDir)
    {
        if (provenance == null)
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(targetDir, "*", SearchOption.AllDirectories)
                     .OrderBy(f => f, StringComparer.Ordinal))
        {
            var relative = Path.GetRelativePath(targetDir, file);
            provenance.Add(RepoRelative(file), RepoRelative(Path.Combine(sourceDir, relative)));
        }
    }

    /// <summary>Records <paramref name="generatedPath"/> ← <paramref name="sourcePath"/> when the generated file was actually produced.</summary>
    private static void MapIfExists(ProvenanceManifest? provenance, string generatedPath, string sourcePath)
    {
        if (provenance != null && File.Exists(generatedPath))
        {
            provenance.Add(RepoRelative(generatedPath), RepoRelative(sourcePath));
        }
    }

    /// <summary>
    /// The localization overlay: the generated <c>Properties</c> resx/json come from the repo's
    /// <c>Resources</c> dir (the fork's own translated copies), not from upstream.
    /// </summary>
    private static void MapLocalizationOverlay(ProvenanceManifest? provenance, string propertiesDir)
    {
        if (provenance == null)
        {
            return;
        }

        foreach (var (generated, source) in new[]
                 {
                     ("GtkWinFormsAppResources.resx", "GtkWinFormsAppResources.xml"),
                     ("GtkWinFormsAppResources.zh-Hans.resx", "GtkWinFormsAppResources.zh-Hans.xml"),
                     ("TestData1.json", "TestData1.json"),
                     ("TestData1.en.json", "TestData1.en.json")
                 })
        {
            MapIfExists(provenance, Path.Combine(propertiesDir, generated),
                RepoPaths.At(Path.Combine("Resources", source)));
        }
    }

    private static void CopyAndFormatGtkWinFormsDesigner(
        string csprojPath,
        string targetDir)
    {
        var projectName =
            Path.GetFileNameWithoutExtension(csprojPath);

        var resourcesDir = RepoPaths.At("Resources");

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

    /// <summary>
    /// Promotes the generated Designer member declarations from <c>private</c> to <c>internal</c> so
    /// <c>GtkWinFormsApp.UITests</c> can reach the control fields (via <see cref="EnsureInternalsVisibleTo"/>)
    /// without reflection. In these generated <c>*.Designer.cs</c> files every <c>private</c> begins a
    /// member declaration (a control field, the <c>components</c> field, or <c>InitializeComponent</c>) -
    /// never inside a string literal - so a line-anchored replace is exact.
    /// </summary>
    private static void InternalizeDesignerFields(string targetDir)
    {
        foreach (var designer in Directory.EnumerateFiles(targetDir, "*.Designer.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(designer);
            var updated = System.Text.RegularExpressions.Regex.Replace(
                text, @"(?m)^(\s*)private\b", "$1internal");
            if (updated != text)
            {
                CsFile.WriteAllText(designer, updated);
            }
        }
    }

    /// <summary>
    /// Grants <c>GtkWinFormsApp.UITests</c> access to the sample's internals via an
    /// <c>InternalsVisibleTo</c> assembly attribute keyed to <see cref="GtkFormsPublicKey"/> (the sample's
    /// own strong-name key). Idempotent.
    /// </summary>
    private static void EnsureInternalsVisibleTo(string csprojPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        var already = doc
            .Descendants(ns + "AssemblyAttribute")
            .Any(a => (string?)a.Attribute("Include") == "System.Runtime.CompilerServices.InternalsVisibleTo"
                      && (a.Element(ns + "_Parameter1")?.Value?.StartsWith(UiTestProjectName + ",") ?? false));

        if (already)
        {
            return;
        }

        var itemGroup = new XElement(ns + "ItemGroup",
            new XElement(ns + "AssemblyAttribute",
                new XAttribute("Include", "System.Runtime.CompilerServices.InternalsVisibleTo"),
                new XElement(ns + "_Parameter1", $"{UiTestProjectName}, PublicKey={GtkFormsPublicKey}")));

        doc.Root!.Add(itemGroup);
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

    private static string RenameProjectFiles(string targetDir, string? sourceProjectName, string? targetProjectName,
        ProvenanceManifest? provenance = null, string? sourceDir = null)
    {
        var oldCsproj = Path.Combine(targetDir, $"{sourceProjectName}.csproj");
        var newCsproj = Path.Combine(targetDir, $"{targetProjectName}.csproj");

        RenameIfExists(oldCsproj, newCsproj);
        RemapRename(provenance, oldCsproj, newCsproj, sourceDir, $"{sourceProjectName}.csproj");

        var oldUser = Path.Combine(targetDir, $"{sourceProjectName}.csproj.user");
        var newUser = Path.Combine(targetDir, $"{targetProjectName}.csproj.user");
        RenameIfExists(oldUser, newUser);
        RemapRename(provenance, oldUser, newUser, sourceDir, $"{sourceProjectName}.csproj.user");

        return newCsproj;
    }

    /// <summary>Moves a manifest entry with the file: the new path keeps pointing at the upstream original.</summary>
    private static void RemapRename(ProvenanceManifest? provenance, string oldPath, string newPath,
        string? sourceDir, string sourceFileName)
    {
        if (provenance == null || sourceDir == null)
        {
            return;
        }

        provenance.Remove(RepoRelative(oldPath));
        MapIfExists(provenance, newPath, Path.Combine(sourceDir, sourceFileName));
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
