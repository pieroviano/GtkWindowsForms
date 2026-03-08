using System.Text;
using System.Text.RegularExpressions;

namespace Prepare.Common.Content;

public static class MemberSpacingFormatter
{
    // Rough match for member starts (fields, properties, methods, constructors)
    private static readonly Regex MemberStartRegex = new(
        @"^\s*(public|private|protected|internal|static|readonly|sealed|abstract|virtual|override|extern|unsafe)",
        RegexOptions.Compiled);

    public static void TransformFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var transformed = EnsureBlankLineBetweenMembers(content);
            CsFile.WriteAllText(file, transformed);
        }
    }

    /// <summary>
    /// Ensures at least one blank line between members (fields, methods, properties),
    /// but does NOT add a blank line before attributes or inside XML comments.
    /// </summary>
    public static string EnsureBlankLineBetweenMembers(string csFileContent)
    {
        var lines = csFileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var output = new StringBuilder(csFileContent.Length);

        var currentBlock = new List<string>();
        var previousBlockWasMember = false;

        var insideXmlComment = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            // Track XML comment blocks
            if (trimmed.StartsWith("///"))
            {
                insideXmlComment = true;
                currentBlock.Add(line);
                continue;
            }
            else if (insideXmlComment)
            {
                // End of XML comment block
                insideXmlComment = false;
                currentBlock.Add(line);
                continue;
            }

            // Ignore empty lines in block
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                currentBlock.Add(line);
                continue;
            }

            // Check if line is an attribute
            var isAttribute = trimmed.StartsWith("[");
            // Check if line is a member start
            var isMember = MemberStartRegex.IsMatch(trimmed);

            if (isMember)
            {
                // Flush previous block if it was a member (not an attribute or XML comment)
                if (previousBlockWasMember)
                {
                    // Ensure at least one blank line before this member
                    if (currentBlock.Count > 0)
                    {
                        if (currentBlock[currentBlock.Count - 1].Trim().Length != 0)
                            output.AppendLine(); // insert blank line
                    }
                }

                foreach (var b in currentBlock)
                    output.AppendLine(b);

                currentBlock.Clear();
                currentBlock.Add(line);
                previousBlockWasMember = true;
                continue;
            }

            // Attribute lines or other lines: just append to current block
            currentBlock.Add(line);

            // Attributes do not count as a member for spacing
            if (!isAttribute)
                previousBlockWasMember = false;
        }

        // Flush remaining lines
        foreach (var b in currentBlock)
            output.AppendLine(b);

        return output.ToString().TrimEnd('\r', '\n');
    }
}
