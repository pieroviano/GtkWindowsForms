using Prepare.Common.Logging;
using System.Xml.Linq;

namespace XmlDocSync.Formatters;

public static class XmlDocumentationFormatter
{
    public static string Format(string content, (string ToReplace, string DestinationValue)[] fixes)
    {
        var lines = SplitLines(content);
        var result = new List<string>();

        var i = 0;
        while (i < lines.Count)
        {
            if (IsXmlDocLine(lines[i], out var indentation))
            {
                var blockLines = new List<string>();
                var blockIndentation = indentation;

                while (i < lines.Count &&
                       IsXmlDocLine(lines[i], out _))
                {
                    blockLines.Add(RemovePrefix(lines[i]));
                    i++;
                }

                var formattedBlock = FormatBlock(blockLines, fixes);
                foreach (var formattedLine in formattedBlock)
                {
                    result.Add($"{blockIndentation}/// {formattedLine}");
                }
            }
            else
            {
                result.Add(lines[i]);
                i++;
            }
        }

        return Normalize(string.Join('\n', result).Replace("///   ", "/// "));
    }

    private static List<string> FormatBlock(List<string> lines, (string ToReplace, string DestinationValue)[] fixes)
    {
        var rawXml = string.Empty;
        try
        {
            rawXml = string.Join('\n', lines);

            var wrapped = $"<root>{'\n'}{rawXml}{'\n'}</root>";
            var doc = XDocument.Parse(wrapped);

            var formatted = doc.ToString();

            var innerLines = formatted
                .Split('\n')
                .Skip(1) // remove <root>
                .SkipLast(1) // remove </root>
                .ToList();

            return innerLines;
        }
        catch (Exception e)
        {
            var fixedLine = false;
            foreach (var fix in fixes)
            {
                for (var index = 0; index < lines.Count; index++)
                {
                    if (lines[index].Contains(fix.ToReplace))
                    {
                        fixedLine = true;
                        lines[index] = lines[index].Replace(fix.ToReplace, fix.DestinationValue);
                    }
                }
            }

            if (fixedLine)
            {
                return FormatBlock(lines, fixes);
            }

            RedConsole.Write("Badly formatted Xml comment:");
            RedConsole.Write($"n===========================\n{rawXml}\n===========================");
            RedConsole.Write($"{e}");

            return lines;
        }
    }

    private static bool IsXmlDocLine(string line, out string indentation)
    {
        indentation = string.Empty;

        if (line == null)
        {
            return false;
        }

        var index = 0;
        while (index < line.Length && (line[index] == ' ' || line[index] == '\t'))
        {
            index++;
        }

        if (line.Substring(index).StartsWith("///"))
        {
            indentation = line.Substring(0, index);
            return true;
        }

        return false;
    }

    private static string Normalize(string s)
    {
        return s is null ? null : s.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private static string RemovePrefix(string line)
    {
        var index = 0;
        while (index < line.Length && (line[index] == ' ' || line[index] == '\t'))
        {
            index++;
        }

        var trimmed = line.Substring(index);

        if (trimmed.StartsWith("///"))
        {
            trimmed = trimmed.Substring(3);
            if (trimmed.StartsWith(" "))
            {
                trimmed = trimmed.Substring(1);
            }
        }

        return trimmed;
    }

    private static List<string> SplitLines(string content)
    {
        return content.Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n')
            .ToList();
    }
}
