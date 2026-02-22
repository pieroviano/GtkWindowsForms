using System.Runtime.InteropServices.JavaScript;
using System.Xml.Linq;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Prepare.Common.Projects;

public static class VersionPrepare
{
    public static string PrepareSolutionVersion()
    {
        var csprojPath = Path.GetFullPath(
            @"..\..\..\..\gtksystem-windows-forms\Source\GTKSystem.Windows.Forms\GTKSystem.Windows.Forms.csproj");

        var directoryBuildPropsPath = Path.GetFullPath(
            @"..\..\..\..\Directory.Build.Props");

        var directoryNugetPropsPath = Path.GetFullPath(
            @"..\..\..\..\Directory.Nuget.Props");

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

        Console.WriteLine(@$"Detected Version: {versionText}");
        Console.WriteLine(@$"First three parts: {firstThree}");
        Console.WriteLine(@$"Last part: {lastPart}");

        // --- Update Directory.Build.Props ---
        var destinationValue = GetDestinationValue(lastPart);
        UpdatePropsValue(
                    directoryBuildPropsPath,
                    "VersionSuffix",
                    destinationValue);

        // --- Update Directory.Nuget.Props ---
        UpdatePropsValue(
            directoryNugetPropsPath,
            "Net4xGtkWindowsFormsVersion",
            firstThree);

        Console.WriteLine(@"Done.");
        return versionText;
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
        Console.WriteLine(@$"Updated {elementName} in {Path.GetFileName(propsPath)}");
    }
}