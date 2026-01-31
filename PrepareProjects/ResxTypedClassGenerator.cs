using System.CodeDom;
using System.CodeDom.Compiler;
using System.Resources.Tools;
using Microsoft.CSharp;

namespace PrepareProjects;

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

        string directory = Path.GetDirectoryName(resxFullPath)!;
        string baseName = Path.GetFileNameWithoutExtension(resxFullPath);

        string outputPath = Path.Combine(
            directory,
            baseName + ".Designer.cs");

        CodeCompileUnit codeUnit = StronglyTypedResourceBuilder.Create(
            resxFullPath,
            baseName,                // class name
            targetNamespace,         // namespace
            new CSharpCodeProvider(),   // default provider
            publicClass,
            out string[] errors);

        if (errors != null && errors.Length > 0)
        {
            throw new InvalidOperationException(
                "Resource generation failed:\n" + string.Join("\n", errors));
        }

        using var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);

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