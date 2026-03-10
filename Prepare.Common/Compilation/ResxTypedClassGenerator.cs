#if !NETSTANDARD
using System.CodeDom.Compiler;
using System.Resources.Tools;
using System.Text;
using Microsoft.CSharp;

namespace PrepareProjects.Compilation;

public static class ResxTypedClassGenerator
{
    public static void GenerateDesigner(
        string resxFullPath,
        string targetNamespace,
        bool publicClass = true)
    {
        if (string.IsNullOrWhiteSpace(resxFullPath))
            throw new ArgumentException("RESX path is null or empty.");

        if (!File.Exists(resxFullPath))
            throw new FileNotFoundException("RESX file not found.", resxFullPath);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        var directory = Path.GetDirectoryName(resxFullPath);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        var baseName = Path.GetFileNameWithoutExtension(resxFullPath);

        var outputPath = Path.Combine(
#pragma warning disable CS8604 // Possible null reference argument.
            directory,
#pragma warning restore CS8604 // Possible null reference argument.
            baseName + ".Designer.cs");

#pragma warning disable CA1416
        var codeUnit = StronglyTypedResourceBuilder.Create(
            resxFullPath,
            baseName,                // class name
            targetNamespace,         // namespace
            new CSharpCodeProvider(),   // default provider
            publicClass,
            out string[] errors);
#pragma warning restore CA1416

        if (errors != null && errors.Length > 0)
        {
            throw new InvalidOperationException(
                "Resource generation failed:\n" + string.Join("\n", errors));
        }

        using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

        using var provider = CodeDomProvider.CreateProvider("CSharp");

        provider.GenerateCodeFromCompileUnit(
            codeUnit,
            writer,
            new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = true
            });
    }
}
#endif
