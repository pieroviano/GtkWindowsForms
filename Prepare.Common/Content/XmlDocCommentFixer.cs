using System.Text;
using Prepare.Common.Models;

namespace Prepare.Common.Content;

public sealed class XmlDocCommentFixer
{
    public string Rewrite(string csFileContent)
    {
        var lines = csFileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var output = new StringBuilder(csFileContent.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            if (IsSummaryStart(lines[i]))
            {
                var block = ReadDocBlock(lines, ref i);

                if (NeedsFixing(block))
                    output.Append(FixDocBlock(block));
                else
                    AppendBlockAsIs(output, block);
            }
            else
            {
                output.AppendLine(lines[i]);
            }
        }

        return output.ToString().Trim('\r', '\n');
    }

    private static void AppendBlockAsIs(StringBuilder sb, List<string> block)
    {
        foreach (var line in block)
            sb.AppendLine(line);
    }

    private static bool NeedsFixing(List<string> block)
    {
        foreach (var line in block)
        {
            var clean = line.TrimStart().TrimStart('/').Trim();

            if (clean.Equals("Parameter:", StringComparison.OrdinalIgnoreCase))
                return true;

            if (clean.Equals("Exception:", StringComparison.OrdinalIgnoreCase))
                return true;

            if (clean.EndsWith(":") && !clean.StartsWith("<"))
                return true;
        }

        return false;
    }


    private static bool IsSummaryStart(string line)
        => line.TrimStart().StartsWith("/// <summary>", StringComparison.Ordinal);

    private static List<string> ReadDocBlock(string[] lines, ref int index)
    {
        var block = new List<string>();

        while (index < lines.Length && lines[index].TrimStart().StartsWith("///"))
        {
            block.Add(lines[index]);
            index++;
        }

        index--; // compensate for outer loop increment
        return block;
    }

    private static string FixDocBlock(List<string> block)
    {
        var indent = GetIndent(block[0]);
        var content = ExtractContent(block);

        var sb = new StringBuilder();
        sb.AppendLine($"{indent}/// <summary>");

        foreach (var summaryLine in content.SummaryLines)
        {
            sb.AppendLine($"{indent}/// {summaryLine}");
        }

        foreach (var param in content.Parameters)
        {
            sb.AppendLine(
                $"{indent}/// <param name=\"{param.Name}\">{param.Description}</param>");
        }

        foreach (var ex in content.Exceptions)
        {
            sb.AppendLine(
                $"{indent}/// <exception cref=\"{ex.Type}\">{ex.Description}</exception>");
        }

        sb.AppendLine($"{indent}/// </summary>");
        return sb.ToString();
    }

    private static string GetIndent(string line)
    {
        int i = 0;
        while (i < line.Length && char.IsWhiteSpace(line[i])) i++;
        return line.Substring(0, i);
    }

    private static DocContent ExtractContent(List<string> block)
    {
        var content = new DocContent();
        int i = 0;

        while (i < block.Count)
        {
            var line = Clean(block[i]);

            if (line == "<summary>" || line == "</summary>")
            {
                i++;
                continue;
            }

            if (line == "Parameter:")
            {
                i++;
                while (i < block.Count && block[i].Trim().EndsWith(":"))
                {
                    var name = Clean(block[i]).TrimEnd(':');
                    i++;

                    var desc = ReadDescription(block, ref i);
                    content.Parameters.Add(new DocParam(name, desc));
                }
                continue;
            }

            if (line == "Exception:")
            {
                i++;
                if (i < block.Count && Clean(block[i]).StartsWith("T:"))
                {
                    var type = Clean(block[i]).Substring(2).TrimEnd(':');
                    i++;

                    var desc = ReadDescription(block, ref i);
                    content.Exceptions.Add(new DocException(type, desc));
                }
                continue;
            }

            content.SummaryLines.Add(line);
            i++;
        }

        return content;
    }

    private static string ReadDescription(List<string> block, ref int index)
    {
        var sb = new StringBuilder();

        while (index < block.Count)
        {
            var line = Clean(block[index]);

            if (line.EndsWith(":") || line == "Parameter:" || line == "Exception:")
                break;

            sb.Append(line).Append(' ');
            index++;
        }

        return sb.ToString().Trim();
    }

    private static string Clean(string line)
        => line.TrimStart().TrimStart('/').Trim();
}