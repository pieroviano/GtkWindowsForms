using System.Text;
using System.Text.RegularExpressions;

namespace XmlDocSync.Formatters;

public static class XmlDocFileFormatter
{
    public static string? FormatFile(string? source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        source = source.Replace("\r\n", "\n");

        var lines = source.Split('\n');
        var result = new StringBuilder();

        var block = new StringBuilder();
        var insideDoc = false;

        foreach (var line in lines)
        {
            if (IsDocLine(line))
            {
                insideDoc = true;
                block.AppendLine(line);
            }
            else
            {
                if (insideDoc)
                {
                    result.Append(ProcessBlock(block.ToString()));
                    block.Clear();
                    insideDoc = false;
                }

                result.AppendLine(line);
            }
        }

        if (insideDoc)
            result.Append(ProcessBlock(block.ToString()));

        return result.ToString().Replace("\n", Environment.NewLine);
    }

    private static bool IsDocLine(string line)
    {
        return line.TrimStart().StartsWith("///");
    }

    private static string ProcessBlock(string block)
    {
        var lines = block.Replace("\r\n", "\n").Split('\n');
        var result = new StringBuilder();

        var summarySeen = false;
        var insideSummary = false;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var trimmed = line.TrimStart();

            if (!trimmed.StartsWith("///"))
            {
                result.AppendLine(line);
                continue;
            }

            var prefixIndex = line.IndexOf("///", StringComparison.Ordinal);
            var indent = line.Substring(0, prefixIndex);
            var content = trimmed.Substring(3).TrimStart();

            if (content.Length == 0)
            {
                result.Append(indent).AppendLine("///");
                continue;
            }

            // Split elements if multiple exist on one line
            var parts = Regex.Split(content, @"(?=<)");

            foreach (var rawPart in parts)
            {
                var part = rawPart.Trim();
                if (part.Length == 0)
                    continue;

                // Detect summary start/end
                if (part.StartsWith("<summary", StringComparison.OrdinalIgnoreCase))
                {
                    if (summarySeen)
                    {
                        insideSummary = true;
                        continue; // skip duplicate summary
                    }

                    summarySeen = true;
                    insideSummary = true;
                }

                if (insideSummary)
                {
                    // Write lines until closing </summary>
                    result.Append(indent).Append("/// ").AppendLine(part);

                    if (part.IndexOf("</summary>", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        insideSummary = false;
                    }

                    continue;
                }

                // Skip everything inside duplicate summary
                if (summarySeen && part.StartsWith("</summary>", StringComparison.OrdinalIgnoreCase))
                {
                    insideSummary = false;
                    continue;
                }

                // Normal element
                result.Append(indent)
                    .Append("/// ")
                    .AppendLine(part);
            }
        }

        return result.ToString();
    }
}
