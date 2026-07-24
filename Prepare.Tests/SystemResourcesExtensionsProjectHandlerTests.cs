using System.Xml.Linq;
using PrepareProjects.ProjectHandling;

namespace Prepare.Tests
{
    /// <summary>
    /// Covers the csproj the prepare pipeline writes for <c>Net4x.System.Resources.Extensions</c>.
    /// <para>
    /// The whole project directory is deleted and re-copied from the upstream submodule on every run
    /// (<c>ProjectUtility.CopyProjectAndTranslateIt</c> calls <c>PrepareEmptyDirectory</c>), so anything the
    /// shipped project needs that upstream does not have - the displacing <c>AssemblyName</c>, the package
    /// id, the README that tells consumers the assembly is empty on purpose - has to be re-emitted here or
    /// it silently reverts to upstream's on the next pipeline run.
    /// </para>
    /// </summary>
    public class SystemResourcesExtensionsProjectHandlerTests : IDisposable
    {
        /// <summary>The upstream csproj, as it arrives from the submodule before the handler rewrites it.</summary>
        private const string UpstreamCsproj =
            """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <DocumentationFile>$(MSBuildProjectDirectory)\$(MSBuildProjectName).xml</DocumentationFile>
                <GenerateDocumentationFile>true</GenerateDocumentationFile>
                <TargetFramework>net6.0</TargetFramework>
                <AllowUnsafeBlocks>True</AllowUnsafeBlocks>
              </PropertyGroup>
              <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
                <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
              </PropertyGroup>
              <ItemGroup>
                <Compile Remove="System\**" />
                <EmbeddedResource Remove="System\**" />
                <None Remove="System\**" />
              </ItemGroup>
              <ItemGroup>
                <Folder Include="Properties\" />
              </ItemGroup>
            </Project>
            """;

        private readonly string _csprojPath =
            Path.Combine(Path.GetTempPath(), $"srx-{Guid.NewGuid():N}", "Net4x.System.Resources.Extensions.csproj");

        public SystemResourcesExtensionsProjectHandlerTests()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_csprojPath)!);
            File.WriteAllText(_csprojPath, UpstreamCsproj);
        }

        public void Dispose()
        {
            var directory = Path.GetDirectoryName(_csprojPath)!;

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void UpdateCsproj_PacksTheReadmeAtThePackageRoot()
        {
            var project = Rewrite();

            var readme = project
                .Elements("ItemGroup")
                .Elements("None")
                .SingleOrDefault(n => (string?)n.Attribute("Include") == "README.md");

            Assert.NotNull(readme);
            Assert.Equal("true", (string?)readme!.Attribute("Pack"));
            Assert.Equal(@"\", (string?)readme.Attribute("PackagePath"));
        }

        /// <summary>
        /// NuGet only surfaces a readme when <c>PackageReadmeFile</c> names one that is actually packed;
        /// either half alone produces a package with no readme (or a pack error).
        /// </summary>
        [Fact]
        public void UpdateCsproj_DeclaresThePackedReadmeAsThePackageReadme()
        {
            var project = Rewrite();

            Assert.Equal("README.md", (string?)project.Descendants("PackageReadmeFile").SingleOrDefault());
        }

        [Fact]
        public void UpdateCsproj_KeepsThePackageIdThatUpstreamDoesNotHave()
        {
            var project = Rewrite();

            Assert.Equal("Gtk.System.Resources.Extensions",
                (string?)project.Descendants("PackageId").SingleOrDefault());
        }

        /// <summary>
        /// The package has to be produced by a plain build, into the folder the pipeline's publish step
        /// reads. Upstream sets neither, so the project used to build a library nobody ever packed.
        /// </summary>
        [Fact]
        public void UpdateCsproj_PacksOnBuildIntoThePublishedPackagesFolder()
        {
            var project = Rewrite();

            Assert.Equal("True", (string?)project.Descendants("GeneratePackageOnBuild").SingleOrDefault());
            Assert.Equal("$(SolutionDir)Packages/",
                (string?)project.Descendants("PackageOutputPath").SingleOrDefault());
        }

        /// <summary>
        /// Left to the SDK, <c>$(VersionSuffix)</c> becomes a prerelease label - the package would ship as
        /// 1.3.24-26204 while every sibling ships 1.3.24.26204.
        /// </summary>
        [Fact]
        public void UpdateCsproj_VersionsThePackageLikeItsSiblings()
        {
            var project = Rewrite();

            Assert.Equal("$(VersionPrefix).$(VersionSuffix)",
                (string?)project.Descendants("Version").SingleOrDefault());
        }

        /// <summary>Otherwise NuGet packs its placeholders ("Package Description", the assembly name as author).</summary>
        [Fact]
        public void UpdateCsproj_GivesThePackageRealAuthorAndDescription()
        {
            var project = Rewrite();

            Assert.Equal("Piero Viano", (string?)project.Descendants("Authors").SingleOrDefault());

            var description = (string?)project.Descendants("Description").SingleOrDefault();
            Assert.NotNull(description);
            Assert.DoesNotContain("Package Description", description!);
            Assert.Contains("intentionally empty", description!);
        }

        /// <summary>A declared <c>PackageLicenseFile</c> that is not packed fails the pack outright.</summary>
        [Fact]
        public void UpdateCsproj_PacksTheLicenseItDeclares()
        {
            var project = Rewrite();

            Assert.Equal("LICENSE.txt", (string?)project.Descendants("PackageLicenseFile").SingleOrDefault());

            var licence = project
                .Elements("ItemGroup")
                .Elements("None")
                .SingleOrDefault(n => ((string?)n.Attribute("Include"))?.EndsWith("LICENSE.txt") == true);

            Assert.NotNull(licence);
            Assert.Equal("true", (string?)licence!.Attribute("Pack"));
            Assert.Equal(@"\", (string?)licence.Attribute("PackagePath"));
        }

        /// <summary>The README's whole subject: the assembly is named to displace Microsoft's.</summary>
        [Fact]
        public void UpdateCsproj_RetargetsAndRenamesTheAssemblyToDisplaceMicrosofts()
        {
            var project = Rewrite();

            Assert.Equal("System.Resources.Extensions",
                (string?)project.Descendants("AssemblyName").SingleOrDefault());
            Assert.Equal("netstandard2.0", (string?)project.Descendants("TargetFramework").SingleOrDefault());
            Assert.Equal(@"$(MSBuildProjectDirectory)\System.Resources.Extensions.xml",
                (string?)project.Descendants("DocumentationFile").SingleOrDefault());
        }

        /// <summary>
        /// A reader of the project file gets the short version of why it is empty; the long version ships
        /// in README.md.
        /// </summary>
        [Fact]
        public void UpdateCsproj_ExplainsTheEmptyAssemblyAndPointsAtTheReadme()
        {
            var project = Rewrite();

            var comment = Assert.Single(project.Elements("PropertyGroup").Nodes().OfType<XComment>());

            Assert.Contains("intentionally empty", comment.Value);
            Assert.Contains("README.md", comment.Value);
        }

        /// <summary>
        /// Upstream's item groups reference source folders this project does not ship, and its
        /// configuration-conditional group is dead weight; both must be gone before the README group is added.
        /// </summary>
        [Fact]
        public void UpdateCsproj_DropsUpstreamsItemGroupsAndConditionalPropertyGroup()
        {
            var project = Rewrite();

            Assert.Empty(project.Descendants("Compile"));
            Assert.Empty(project.Descendants("Folder"));
            Assert.Empty(project.Elements("PropertyGroup").Where(pg => pg.Attribute("Condition") != null));
        }

        /// <summary>
        /// The handler runs against a freshly copied upstream csproj each time, but a re-run over its own
        /// output must not duplicate the comment, the properties or the README item group.
        /// </summary>
        [Fact]
        public void UpdateCsproj_IsIdempotent()
        {
            SystemResourcesExtensionsProjectHandler.UpdateCsproj(_csprojPath);
            var project = Rewrite();

            Assert.Single(project.Descendants("PackageId"));
            Assert.Single(project.Descendants("PackageReadmeFile"));
            Assert.Single(project.Descendants("GeneratePackageOnBuild"));
            Assert.Single(project.Descendants("Version"));
            Assert.Single(project.Elements("PropertyGroup").Nodes().OfType<XComment>());
            Assert.Single(project.Descendants("None").Where(n => (string?)n.Attribute("Include") == "README.md"));
        }

        /// <summary>Runs the handler and returns the rewritten &lt;Project&gt; element.</summary>
        private XElement Rewrite()
        {
            SystemResourcesExtensionsProjectHandler.UpdateCsproj(_csprojPath);

            return XDocument.Load(_csprojPath).Root!;
        }
    }
}
