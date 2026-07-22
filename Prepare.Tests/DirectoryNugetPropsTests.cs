using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Prepare.Tests
{
    /// <summary>
    /// Keeps <c>Directory.Nuget.Props</c> to the versions this repository actually consumes.
    /// <para>
    /// It was once a copy of the version manifest shared across the wider CommonLibrary tree - ~290
    /// properties, of which eight were read here. Carrying the rest meant this repository could not be
    /// built or reasoned about standalone: an edit made elsewhere, for another consumer, arrived as a
    /// version change nobody working on this solution had asked for, and one unresolvable entry was
    /// enough to stop the whole solution restoring.
    /// </para>
    /// These tests fail in both directions - a property here that nothing references, and a reference
    /// with no definition - so the file cannot silently drift back into a catalogue or lose an entry
    /// something needs.
    /// </summary>
    public class DirectoryNugetPropsTests
    {
        /// <summary>
        /// Properties MSBuild itself defines, so a reference to one needs no definition in this repository.
        /// </summary>
        private static readonly HashSet<string> MsBuildIntrinsics = new(StringComparer.OrdinalIgnoreCase)
        {
            "TargetFrameworkVersion",
            "NETCoreSdkVersion",
            "BundledNETCoreAppPackageVersion",
        };

        private static readonly string[] IgnoredDirectories =
        {
            "obj", "bin", ".git", ".vs", "mono", "gtksystem-windows-forms", "Packages", "TestResults",
        };

        [Fact]
        public void EveryPropertyDefined_IsActuallyUsedSomewhereInTheRepository()
        {
            var defined = DefinedProperties();
            var referenced = ReferencedProperties();

            var dead = defined.Keys.Where(name => !referenced.ContainsKey(name)).OrderBy(n => n).ToList();

            Assert.True(dead.Count == 0,
                "Directory.Nuget.Props defines properties nothing in this repository references: " +
                string.Join(", ", dead) +
                ". Versions for packages this solution does not consume belong in the consuming repository, " +
                "not here - that coupling is what this file was minimised to remove.");
        }

        [Fact]
        public void EveryVersionReferenced_HasADefinition()
        {
            var defined = DefinedProperties();
            var buildProps = PropertiesOf(Path.Combine(RepositoryRoot, "Directory.Build.Props"));

            var missing = ReferencedProperties()
                .Where(pair => pair.Key.EndsWith("Version", StringComparison.Ordinal))
                .Where(pair => !defined.ContainsKey(pair.Key))
                .Where(pair => !buildProps.ContainsKey(pair.Key))
                .Where(pair => !MsBuildIntrinsics.Contains(pair.Key))
                // A project may define a version for its own use; that is not this file's business.
                .Where(pair => !pair.Value.All(file => PropertiesOf(file).ContainsKey(pair.Key)))
                .Select(pair => $"{pair.Key} (referenced by {string.Join(", ", pair.Value.Select(Path.GetFileName).Distinct())})")
                .OrderBy(text => text)
                .ToList();

            Assert.True(missing.Count == 0,
                "These version properties are referenced but defined nowhere, so they expand to empty: " +
                string.Join("; ", missing));
        }

        [Fact]
        public void TheFileStaysSmall()
        {
            var defined = DefinedProperties();

            // Not an arbitrary limit: the point of the file is that its contents can be checked by eye.
            // If this trips because the solution genuinely took on new dependencies, raise it. If it trips
            // because the shared CommonLibrary manifest was copied back over the file, do not.
            Assert.True(defined.Count <= 20,
                $"Directory.Nuget.Props defines {defined.Count} properties. It is meant to carry only what " +
                "this solution consumes - check whether the shared CommonLibrary manifest has been copied " +
                "back over it.");
        }

        [Fact]
        public void TheSolutionVersion_LivesInTheFirstPropertyGroup()
        {
            // Prepare.Common/Projects/VersionPrepare.cs rewrites this element in place during the pipeline
            // and looks for it in the FIRST PropertyGroup only, throwing otherwise.
            var document = XDocument.Load(NugetPropsPath);
            var ns = document.Root!.Name.Namespace;
            var first = document.Root.Elements(ns + "PropertyGroup").FirstOrDefault();

            Assert.NotNull(first);
            Assert.NotNull(first!.Element(ns + "Net4xGtkWindowsFormsVersion"));
        }

        [Fact]
        public void TheImportGuard_IsPresent()
        {
            // Directory.Build.Props imports this file only while DirectoryNuGet is empty; without the flag
            // being set here the import is not idempotent.
            Assert.True(DefinedProperties().ContainsKey("DirectoryNuGet"));

            var buildProps = File.ReadAllText(Path.Combine(RepositoryRoot, "Directory.Build.Props"));
            Assert.Contains("Directory.Nuget.Props", buildProps);
            Assert.Contains("$(DirectoryNuGet)", buildProps);
        }

        private static string NugetPropsPath => Path.Combine(RepositoryRoot, "Directory.Nuget.Props");

        private static string RepositoryRoot
        {
            get
            {
                var directory = new DirectoryInfo(AppContext.BaseDirectory);
                while (directory != null)
                {
                    if (File.Exists(Path.Combine(directory.FullName, "Directory.Nuget.Props")))
                    {
                        return directory.FullName;
                    }

                    directory = directory.Parent;
                }

                throw new DirectoryNotFoundException("Could not locate the repository root.");
            }
        }

        /// <summary>Property names defined in <c>Directory.Nuget.Props</c>, and their values.</summary>
        private static Dictionary<string, string> DefinedProperties() => PropertiesOf(NugetPropsPath);

        private static Dictionary<string, string> PropertiesOf(string propsPath)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!File.Exists(propsPath) ||
                !propsPath.EndsWith(".props", StringComparison.OrdinalIgnoreCase) &&
                !propsPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) &&
                !propsPath.EndsWith(".targets", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            XDocument document;
            try
            {
                document = XDocument.Load(propsPath);
            }
            catch (Exception)
            {
                return result;
            }

            var ns = document.Root!.Name.Namespace;
            foreach (var group in document.Root.Descendants(ns + "PropertyGroup"))
            {
                foreach (var property in group.Elements())
                {
                    result[property.Name.LocalName] = property.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Every <c>$(Property)</c> referenced by an MSBuild file in the repository, mapped to the files
        /// referencing it. The submodules are excluded: they are upstream sources, not consumers of this
        /// file.
        /// </summary>
        private static Dictionary<string, List<string>> ReferencedProperties()
        {
            var result = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            var pattern = new Regex(@"\$\(([A-Za-z_][\w.]*)\)", RegexOptions.Compiled);

            foreach (var file in MsBuildFiles(RepositoryRoot))
            {
                if (string.Equals(Path.GetFileName(file), "Directory.Nuget.Props", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (Match match in pattern.Matches(File.ReadAllText(file)))
                {
                    var name = match.Groups[1].Value;
                    if (!result.TryGetValue(name, out var files))
                    {
                        result[name] = files = new List<string>();
                    }

                    if (!files.Contains(file))
                    {
                        files.Add(file);
                    }
                }
            }

            // VersionPrepare rewrites the solution version through the XML rather than an MSBuild
            // reference, so it would otherwise look dead.
            result.TryAdd("Net4xGtkWindowsFormsVersion", new List<string> { "Prepare.Common/Projects/VersionPrepare.cs" });

            return result;
        }

        private static IEnumerable<string> MsBuildFiles(string root)
        {
            foreach (var file in Directory.EnumerateFiles(root))
            {
                if (file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".props", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".targets", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }

            foreach (var directory in Directory.EnumerateDirectories(root))
            {
                var name = Path.GetFileName(directory);
                if (IgnoredDirectories.Contains(name, StringComparer.OrdinalIgnoreCase) ||
                    name.EndsWith("Backup", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var file in MsBuildFiles(directory))
                {
                    yield return file;
                }
            }
        }
    }
}
