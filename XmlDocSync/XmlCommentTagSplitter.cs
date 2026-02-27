using System.Text.RegularExpressions;

namespace XmlDocSync;

public static class XmlCommentTagSplitter
{
    private static readonly Regex TargetRegex =
        new(@"^(?<indent>[ \t]*)///\s*(?<tag><[A-Za-z_\-.]+>)(?<text>.*)$",
            RegexOptions.Compiled);

    public static string Process(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            var match = TargetRegex.Match(lines[i]);
            if (!match.Success)
                continue;

            var indent = match.Groups["indent"].Value;
            var tag = match.Groups["tag"].Value;
            var text = match.Groups["text"].Value.Trim();

            if (text.Length == 0 || text.Length >= 10)
                continue;

            // Replace original line with tag only
            lines[i] = $"{indent}/// {tag}";

            // Insert new xml comment line with short text
            lines.Insert(i + 1, $"{indent}/// {text}");

            i++; // Skip inserted line
        }

        return string.Join("\n", lines);
    }
}