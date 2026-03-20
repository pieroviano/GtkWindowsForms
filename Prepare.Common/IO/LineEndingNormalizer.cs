using System.Text;

namespace Prepare.Common.IO;

public static class LineEndingNormalizer
{
    public static void NormalizeCsFiles(string sourceFolder)
    {
        var files = Directory.GetFiles(sourceFolder, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);

            // Normalize line endings
            content = content.Replace("\r\n", "\n");
            content = content.Replace("\n", "\r\n");
            content = content.Replace("\r\n\r\n\r\n", "\r\n\r\n");

            // Save using UTF8 (without BOM if you prefer)
            File.WriteAllText(file, content, new UTF8Encoding(false));
        }
    }
}