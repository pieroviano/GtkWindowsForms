using System.Text.RegularExpressions;

namespace XmlDocSync;

public static partial class XmlDocIndentFormatter
{
    public static string FixIndentationAndRemoveBlanks(string source)
    {
        if (string.IsNullOrEmpty(source))
            return source;

        var pattern = @"
(?<comment>
^///.*\r?\n
(?:^///.*\r?\n)*
)
(?<between>
(?:^[ \t]*\r?\n|^\s*//.*\r?\n)*     # blank or // comment lines
(?:^\s*\[.*\]\r?\n)*                # attributes
)
(?<decl>
^\s*[^\r\n{]*                       # declaration
)
(?<brace>\s*\{?)                    # brace optional same line
";

        return Regex.Replace(
            source,
            pattern,
            m =>
            {
                string comment = m.Groups["comment"].Value;
                string between = m.Groups["between"].Value;
                string decl = m.Groups["decl"].Value;
                string bracePart = m.Groups["brace"].Value;

                // Detect indentation from declaration line
                var indentMatch = Regex.Match(decl, @"^[ \t]*");
                string indent = indentMatch.Value;

                string FixBlock(string block)
                {
                    if (string.IsNullOrEmpty(block))
                        return "";

                    var lines = block.Split(
                        new[] { "\r\n", "\n" },
                        StringSplitOptions.None);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        lines[i] = indent + lines[i].TrimStart();
                    }

                    return string.Join(Environment.NewLine, lines)
                        .TrimEnd();
                }

                // Remove empty lines from "between" but keep comments/attributes
                var cleanedBetween = Regex.Replace(
                    between,
                    @"^[ \t]*\r?\n",
                    "",
                    RegexOptions.Multiline);

                var fixedComment = FixBlock(comment);
                var fixedBetween = FixBlock(cleanedBetween);
                var fixedDecl = indent + decl.TrimStart() + bracePart;

                return fixedComment
                       + Environment.NewLine
                       + (string.IsNullOrEmpty(fixedBetween)
                           ? ""
                           : fixedBetween + Environment.NewLine)
                       + fixedDecl;
            },
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
    }
}