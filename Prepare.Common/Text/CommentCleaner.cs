using System.Text.RegularExpressions;

namespace Prepare.Common.Text;

public static class CommentCleaner
{
    public static string RemoveCommentBlock(string content, string comment)
    {
        var lines = content
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .ToList();

        var regex = TryCreateRegex(comment);
        var useRegex = regex != null;

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var commentStart = line.IndexOf("//", StringComparison.Ordinal);
            if (commentStart < 0)
                continue;

            var matchIndex = useRegex
                ? GetRegexMatchIndex(regex!, line)
                : line.IndexOf(comment, StringComparison.Ordinal);

            if (matchIndex < 0)
                continue;

            // Ignore matches that appear before the comment portion
            if (matchIndex < commentStart)
                continue;

            // CASE 1: Inline comment (code before it)
            if (!line.TrimStart().StartsWith("//"))
            {
                HandleInlineCommentWithCodeBeforeIt(lines, i, commentStart);
                continue;
            }

            // CASE 2: Comment block line
            i = HandleBlockOfLineComments(i, lines);
        }
        CollapseMultipleEmptyLines(lines);

        return string.Join("\n", lines);
    }

    private static Regex? TryCreateRegex(string comment)
    {
        if (comment.Length >= 2 &&
            comment.StartsWith("/") &&
            comment.EndsWith("/"))
        {
            var pattern = comment.Substring(1, comment.Length - 2);
            return new Regex(pattern, RegexOptions.Compiled);
        }

        return null;
    }

    private static int GetRegexMatchIndex(Regex regex, string line)
    {
        var match = regex.Match(line);
        return match.Success ? match.Index : -1;
    }

    private static int HandleBlockOfLineComments(int i, List<string> lines)
    {
        var start = i;
        while (start >= 0 && lines[start].TrimStart().StartsWith("//"))
            start--;
        start++;

        var end = i;
        while (end < lines.Count && lines[end].TrimStart().StartsWith("//"))
            end++;
        end--;

        lines.RemoveRange(start, end - start + 1);

        return start - 1;
    }

    private static void HandleInlineCommentWithCodeBeforeIt(List<string> lines, int i, int commentStart)
    {
        lines[i] = lines[i]
            .Substring(0, commentStart)
            .TrimEnd();
    }

    private static void CollapseMultipleEmptyLines(List<string> lines)
    {
        // Remove leading empty lines
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
            lines.RemoveAt(0);

        for (int i = 1; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]) &&
                string.IsNullOrWhiteSpace(lines[i - 1]))
            {
                lines.RemoveAt(i);
                i--;
            }
        }
    }
}
