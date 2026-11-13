using System.Xml.Linq;
using Prepare.Common.IO;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Prepare.Common.Projects;

public static class VersionPrepare
{
    public static (string VersionText, Action UpdatePropsFiles) PrepareSolutionVersion()
    {
        var csprojPath = Path.GetFullPath(
            RepoPaths.At(@"gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKSystem.Windows.Forms.csproj"));

        var directoryBuildPropsPath = Path.GetFullPath(
            RepoPaths.At("Directory.Build.props"));

        var directoryNugetPropsPath = Path.GetFullPath(
            RepoPaths.At("Directory.Nuget.props"));

        // --- Read Version from csproj ---
        var csprojDoc = XDocument.Load(csprojPath);

        var ns = csprojDoc.Root.Name.Namespace;

        var versionElement = csprojDoc
            .Descendants(ns + "Version")
            .FirstOrDefault();

        if (versionElement == null)
        {
            throw new InvalidOperationException("Version element not found in csproj.");
        }

        var versionText = versionElement.Value.Trim();

        var parts = versionText.Split('.');
        if (parts.Length < 4)
        {
            throw new InvalidOperationException("Version must have at least 4 parts.");
        }

        var firstThree = string.Join(".", parts.Take(3));
        var lastPart = parts.Last();

        ConsoleColor.White.Write($@"Detected Version: {versionText}", true);
        ConsoleColor.White.Write($@"First three parts: {firstThree}", true);
        ConsoleColor.White.Write($@"Last part: {lastPart}", true);

        var updatePropsFiles = new Action(() => UpdatePropsFiles(lastPart, directoryBuildPropsPath, directoryNugetPropsPath, firstThree));

        ConsoleColor.White.Write(@"Preparing solution version done.", true);
        return (versionText, updatePropsFiles);
    }

    private static void UpdatePropsFiles(string lastPart, string directoryBuildPropsPath, string directoryNugetPropsPath,
        string firstThree)
    {
        // --- Update Directory.Build.props ---
        var destinationValue = GetDestinationValue(lastPart);
        UpdatePropsValue(
            directoryBuildPropsPath,
            "VersionSuffix",
            destinationValue);

        // --- Update Directory.Nuget.props ---
        UpdatePropsValue(
            directoryNugetPropsPath,
            "Net4xGtkWindowsFormsVersion",
            firstThree);
    }

    private static string GetDestinationValue(string lastPart)
    {
#if DEBUG
        var destinationValue = $"{lastPart}-a{DateTime.Now:yyyyMMddHHmmss}";
#else
        var destinationValue = string.Format("{0}{1}",
            (DateTime.Today.Year % 100).ToString().PadLeft(2, '0'),
            DateTime.Today.DayOfYear.ToString().PadLeft(3, '0'));
#endif
        return destinationValue;
    }

    private static void UpdatePropsValue(string propsPath, string elementName, string value)
    {
        var doc = XDocument.Load(propsPath);
        var ns = doc.Root.Name.Namespace;

        var propertyGroup = doc
            .Root
            .Elements(ns + "PropertyGroup")
            .FirstOrDefault();

        if (propertyGroup == null)
        {
            throw new InvalidOperationException($"No PropertyGroup found in {propsPath}");
        }

        var element = propertyGroup.Element(ns + elementName);
        if (element == null)
        {
            throw new InvalidOperationException(
                $"Element <{elementName}> not found in {propsPath}");
        }

        element.Value = value;

        doc.Save(propsPath);
        ConsoleColor.White.Write($@"Updated {elementName} in {Path.GetFileName(propsPath)}");
    }
}
