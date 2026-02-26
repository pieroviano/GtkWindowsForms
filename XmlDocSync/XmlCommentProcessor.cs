using System.Text.RegularExpressions;

namespace XmlDocSync;

public static class XmlCommentProcessor
{
    public static string Process(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

        for (int i = 1; i < lines.Count; i++)
        {
            var currentLine = lines[i];
            var previousLine = lines[i - 1];

            if (IsXmlCommentLine(currentLine) &&
                IsXmlCommentLine(previousLine))
            {
                var currentText = ExtractCommentText(currentLine);

                if (ShouldMerge(currentText) && !Regex.IsMatch(previousLine, "<[A-Za-z_\\-.]+>") && !Regex.IsMatch(previousLine, "<[A-Za-z_\\-.]+/>"))
                {
                    var trimmed = currentText.Trim();

                    lines[i - 1] = previousLine.TrimEnd() + " " + trimmed;
                    lines.RemoveAt(i);
                    i--; // re-evaluate at same position
                }
            }
        }

        return string.Join("\n", lines);
    }

    private static bool IsXmlCommentLine(string line)
    {
        return line.TrimStart().StartsWith("///");
    }

    private static string ExtractCommentText(string line)
    {
        var index = line.IndexOf("///", StringComparison.Ordinal);
        return index >= 0
            ? line.Substring(index + 3)
            : string.Empty;
    }

    private static bool ShouldMerge(string text)
    {
        var trimmed = text.Trim();

        if (trimmed.Length == 0 || trimmed.Length >= 10)
            return false;

        // Skip XML tags like <summary>, </summary>, <param name="x">
        if (trimmed.StartsWith("<") && trimmed.EndsWith(">"))
            return false;

        return true;
    }
}