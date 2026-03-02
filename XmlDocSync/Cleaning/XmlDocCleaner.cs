using System.Text;
using System.Text.RegularExpressions;

namespace XmlDocSync.Cleaning;

public static class XmlDocCleaner
{
    public static string RemoveTextBeforeNonOpenedSummary(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return source;

        var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var result = new StringBuilder();
        var block = new List<string>();

        void FlushBlock()
        {
            if (block.Count == 0)
                return;

            var blockText = string.Join("\n", block);

            var openCount = Regex.Matches(blockText, @"<summary>").Count;
            var closeCount = Regex.Matches(blockText, @"</summary>").Count;

            if (openCount > 0 && closeCount > openCount)
            {
                blockText = CleanupExtraClosingSummaries(blockText);
            }

            // Ensure exactly one newline after a flushed XML doc block
            result.AppendLine(blockText.TrimEnd('\r', '\n'));

            block.Clear();
        }

        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("///"))
            {
                block.Add(line);
            }
            else
            {
                FlushBlock();
                result.AppendLine(line);
            }
        }

        FlushBlock();

        return result
            .ToString()
            .Replace("\r\n", "\n")
            .TrimEnd(' ', '\t', '\n');
    }

    private static string CleanupExtraClosingSummaries(string block)
    {
        var summaryPattern = new Regex(
            @"///\s*<summary>[\s\S]*?///\s*</summary>",
            RegexOptions.Multiline);

        var match = summaryPattern.Match(block);
        if (!match.Success)
            return block;

        var cleaned = new StringBuilder();

        // Keep first valid summary block
        cleaned.Append(match.Value.TrimEnd());

        var remainder = block.Substring(match.Index + match.Length);

        remainder = Regex.Replace(
            remainder,
            @"\s*(?:///(?!\s*<summary>).*(?:\r\n|\n|\r))*\s*///\s*</summary>\s*(?:\r\n|\n|\r)?",
            string.Empty,
            RegexOptions.Multiline);

        if (!string.IsNullOrWhiteSpace(remainder))
        {
            cleaned.Append('\n');
            cleaned.Append(remainder.TrimEnd());
        }

        return cleaned.ToString();
    }
}
