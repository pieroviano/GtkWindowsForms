using System.Text;

namespace Prepare.Common.Content;

public sealed class XmlDocSummaryNormalizer
{
    public string Normalize(string csFileContent)
    {
        var lines = csFileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var output = new StringBuilder(csFileContent.Length);

        for (var i = 0; i < lines.Length; i++)
        {
            if (IsDocLine(lines[i]))
            {
                var block = ReadDocBlock(lines, ref i);
                output.Append(NormalizeDocBlock(block));
            }
            else
            {
                output.AppendLine(lines[i]);
            }
        }

        var trimEnd = NormalizeSummaryBlockLines(output.ToString()).TrimEnd('\r', '\n').Replace("/// /// ", "/// ");
        return trimEnd;
    }

    public static string NormalizeSummaryBlockLines(string csFileContent)
    {
        var lines = csFileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var output = new StringBuilder(csFileContent.Length);

        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            var startIndex = line.IndexOf("<summary>", StringComparison.Ordinal);

            if (startIndex >= 0)
            {
                var indent = line.Substring(0, startIndex); // spaces before <summary>
                var summaryEnd = startIndex + "<summary>".Length;

                // Everything after <summary>
                var afterSummary = line.Length > summaryEnd
                    ? line.Substring(summaryEnd).Trim()
                    : string.Empty;

                // If afterSummary contains </summary>, split it
                if (!string.IsNullOrEmpty(afterSummary) && afterSummary.Contains("</summary>"))
                {
                    var endIndex = afterSummary.IndexOf("</summary>", StringComparison.Ordinal);
                    var textBeforeClosing = afterSummary.Substring(0, endIndex).Trim();
                    var closingTag = "</summary>";

                    // Write <summary> line
                    output.AppendLine($"{indent}/// <summary>");

                    // Write text line if any
                    if (!string.IsNullOrEmpty(textBeforeClosing))
                        output.AppendLine($"{indent}/// {textBeforeClosing}");

                    // Write </summary> on its own line
                    output.AppendLine($"{indent}/// {closingTag}");
                }
                else
                {
                    // Only <summary> or <summary> + text without </summary>
                    output.AppendLine($"{indent}/// <summary>");
                    if (!string.IsNullOrEmpty(afterSummary))
                        output.AppendLine($"{indent}/// {afterSummary}");
                }

                continue; // skip original line
            }

            output.AppendLine(line);
        }

        return output.ToString().TrimEnd('\r', '\n');
    }


    private static bool IsDocLine(string line)
        => line.TrimStart().StartsWith("///", StringComparison.Ordinal);

    private static List<string> ReadDocBlock(string[] lines, ref int index)
    {
        var block = new List<string>();

        while (index < lines.Length && IsDocLine(lines[index]))
        {
            block.Add(lines[index]);
            index++;
        }

        index--;
        return block;
    }

    private static string NormalizeDocBlock(List<string> block)
    {
        var indent = GetIndent(block[0]);
        var content = ExtractXmlLines(block);

        var summaryOpen = false;
        var summaryClosed = false;

        var normalized = new List<string>();
        var insertSummaryCloseAt = -1;

        for (var i = 0; i < content.Count; i++)
        {
            var line = content[i];

            if (line.Contains("<summary>"))
            {
                summaryOpen = true;
                normalized.Add(line);
                continue;
            }

            if (line.Contains("</summary>"))
            {
                summaryClosed = true;

                if (!summaryOpen)
                    continue; // stray closing tag, drop it

                normalized.Add(line);
                continue;
            }

            if (summaryOpen && !summaryClosed && IsNonSummaryElement(line))
            {
                insertSummaryCloseAt = normalized.Count;
                summaryClosed = true;
                summaryOpen = false;
            }

            normalized.Add(line);
        }

        if (insertSummaryCloseAt >= 0 && !normalized.Contains("</summary>"))
        {
            normalized.Insert(insertSummaryCloseAt, "/// </summary>");
        }

        var sb = new StringBuilder();
        foreach (var line in normalized)
        {
            sb.AppendLine(indent + line);
        }

        var normalizeDocBlock = sb.ToString();
        return RemoveDuplicateSummaryClosings(normalizeDocBlock);
    }

    public static string RemoveDuplicateSummaryClosings(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        const string closingTag = "</summary>";

        // Find all positions of </summary>
        var positions = new List<int>();
        var index = 0;
        while ((index = input.IndexOf(closingTag, index, StringComparison.Ordinal)) >= 0)
        {
            positions.Add(index);
            index += closingTag.Length;
        }

        // If 0 or 1 occurrence, nothing to remove
        if (positions.Count <= 1)
            return input;

        // Keep only the last </summary>, remove all previous
        var sb = new StringBuilder();
        var start = 0;
        for (var i = 0; i < positions.Count - 1; i++)
        {
            // Append text up to the duplicate </summary>
            sb.Append(input[start..positions[i]]);
            start = positions[i] + closingTag.Length; // skip this duplicate
        }

        // Append the rest including the last </summary>
        sb.Append(input[start..]);
        return sb.ToString();
    }


    private static bool IsNonSummaryElement(string line)
    {
        var t = line.Trim();
        return t.StartsWith("/// <param", StringComparison.Ordinal)
               || t.StartsWith("/// <exception", StringComparison.Ordinal)
               || t.StartsWith("/// <returns", StringComparison.Ordinal)
               || t.StartsWith("/// <typeparam", StringComparison.Ordinal)
               || t.StartsWith("/// <remarks", StringComparison.Ordinal);
    }

    private static List<string> ExtractXmlLines(List<string> block)
    {
        var list = new List<string>();

        foreach (var line in block)
        {
            var trimmed = line.TrimStart();
            list.Add(trimmed);
        }

        return list;
    }

    private static string GetIndent(string line)
    {
        var i = 0;
        while (i < line.Length && char.IsWhiteSpace(line[i])) i++;
        return line.Substring(0, i);
    }
}