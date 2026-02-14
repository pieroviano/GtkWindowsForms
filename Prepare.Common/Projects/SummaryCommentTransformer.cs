using System.Text;
using System.Text.RegularExpressions;

namespace Prepare.Common.Projects;

public class SummaryCommentTransformer
{
    public void TransformFolder(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            TransformFile(file);
        }
    }

    private void TransformFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath).ToList();
        var output = new StringBuilder();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (IsSummaryStart(line))
            {
                var indent = GetIndentation(line);

                // Remove preceding empty comment line if present
                if (output.Length > 0)
                {
                    var previousLine = GetLastLine(output);

                    if (IsEmptyCommentLine(previousLine))
                    {
                        RemoveLastLine(output);
                        output.AppendLine(); // replace with blank line
                    }
                }

                var summaryLines = new List<string>();

                // Move past "Summary:"
                i++;

                // Collect subsequent comment lines
                while (i < lines.Count && IsCommentLine(lines[i]))
                {
                    var content = ExtractCommentContent(lines[i]).Trim();

                    if (!string.IsNullOrWhiteSpace(content))
                        summaryLines.Add(content);

                    i++;
                }

                i--; // adjust for loop increment

                // Write XML summary
                output.AppendLine($"{indent}/// <summary>");
                foreach (var summaryLine in summaryLines)
                {
                    output.AppendLine($"{indent}/// {summaryLine}");
                }
                output.AppendLine($"{indent}/// </summary>");
            }
            else
            {
                output.AppendLine(line);
            }
        }

        File.WriteAllText(filePath, output.ToString());
    }

    private bool IsSummaryStart(string line)
    {
        return Regex.IsMatch(line, @"^\s*//\s*Summary:\s*$");
    }

    private bool IsCommentLine(string line)
    {
        return Regex.IsMatch(line, @"^\s*//");
    }

    private bool IsEmptyCommentLine(string line)
    {
        return Regex.IsMatch(line, @"^\s*//\s*$");
    }

    private string ExtractCommentContent(string line)
    {
        return Regex.Replace(line, @"^\s*//\s?", "");
    }

    private string GetIndentation(string line)
    {
        return new string(line.TakeWhile(char.IsWhiteSpace).ToArray());
    }

    private string GetLastLine(StringBuilder sb)
    {
        var content = sb.ToString().TrimEnd('\r', '\n');
        var lastNewLine = content.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

        if (lastNewLine < 0)
            return content;

        return content.Substring(lastNewLine + Environment.NewLine.Length);
    }

    private void RemoveLastLine(StringBuilder sb)
    {
        var content = sb.ToString().TrimEnd('\r', '\n');
        var lastNewLine = content.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

        if (lastNewLine >= 0)
            sb.Clear().Append(content.Substring(0, lastNewLine + Environment.NewLine.Length));
        else
            sb.Clear();
    }
}