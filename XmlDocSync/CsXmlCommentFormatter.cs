using System.Text.RegularExpressions;

namespace XmlDocSync;

public static class CsXmlCommentFormatter
{
    public static string FixXmlCommentIndentation(string content)
    {
        if (content == null)
            return null;

        var lines = Regex.Split(content, @"\r?\n");
        var result = new List<string>(lines);

        for (int i = 0; i < result.Count; i++)
        {
            if (!IsXmlCommentLine(result[i]))
                continue;

            int commentStart = i;
            int commentEnd = i;

            // Collect full contiguous /// block
            while (commentEnd + 1 < result.Count &&
                   IsXmlCommentLine(result[commentEnd + 1]))
            {
                commentEnd++;
            }

            int j = commentEnd + 1;

            // Search forward for opening brace
            while (j < result.Count)
            {
                string line = result[j];

                // If blank line → ignore block entirely
                if (IsBlank(line))
                    break;

                var braceMatch = Regex.Match(line, @"^([ \t]*)\{");
                if (braceMatch.Success)
                {
                    string targetIndent = braceMatch.Groups[1].Value;

                    // Fix comment block indentation
                    for (int k = commentStart; k <= commentEnd; k++)
                        result[k] = ReplaceLeadingWhitespace(result[k], targetIndent);

                    // Fix declaration line (line before brace)
                    if (j - 1 > commentEnd)
                        result[j - 1] = ReplaceLeadingWhitespace(result[j - 1], targetIndent);

                    // Fix brace line itself
                    result[j] = ReplaceLeadingWhitespace(result[j], targetIndent);

                    i = j; // Skip ahead
                    break;
                }

                j++;
            }
        }

        return string.Join("\n", result).Trim('\r', '\n', ' ', '\t');
    }

    private static bool IsXmlCommentLine(string line)
    {
        return Regex.IsMatch(line, @"^[ \t]*///");
    }

    private static bool IsBlank(string line)
    {
        return Regex.IsMatch(line, @"^[ \t]*$");
    }

    private static string ReplaceLeadingWhitespace(string line, string newIndent)
    {
        return Regex.Replace(line, @"^[ \t]*", newIndent);
    }
}