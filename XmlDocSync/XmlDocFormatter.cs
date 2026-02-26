using System.Text;
using System.Text.RegularExpressions;

namespace XmlDocSync;

public static class XmlDocFormatter
{
    public static string Format(string source)
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

            var processed = ProcessXmlBlock(string.Join("\n", block));
            result.Append(processed);
            block.Clear();
        }

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line, @"^[ \t]*///"))
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

        return result.ToString().Replace("\r\n", "\n").TrimEnd('\r', '\n');
    }

    private static string ProcessXmlBlock(string block)
    {
        // PASS 1 — split lines like: /// <tag>text
        block = Regex.Replace(
            block,
            @"^(?<indent>[ \t]*/// )(?<open><[^/!\?][^>]*>)(?<text>.+)$",
            m =>
            {
                var indent = m.Groups["indent"].Value;
                var open = m.Groups["open"].Value;
                var text = m.Groups["text"].Value.TrimStart();

                return $"{indent}{open}\n{indent}{text}";
            },
            RegexOptions.Multiline);
        return block + "\n";
    }
}