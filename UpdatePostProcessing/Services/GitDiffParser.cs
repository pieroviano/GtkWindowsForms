using System.Text;
using UpdatePostProcessing.Model;

namespace UpdatePostProcessing.Services;

internal class GitDiffParser
{
    public List<TextReplacement> Parse(string diffText)
    {
        var replacements = new List<TextReplacement>();

        if (string.IsNullOrWhiteSpace(diffText))
            return replacements;

        var lines = diffText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var oldBuilder = new StringBuilder();
        var newBuilder = new StringBuilder();

        bool insideHunk = false;

        foreach (var line in lines)
        {
            if (line.StartsWith("@@"))
            {
                // Flush previous block if exists
                FlushReplacement(oldBuilder, newBuilder, replacements);
                insideHunk = true;
                continue;
            }

            if (!insideHunk)
                continue;

            if (line.StartsWith("-") && !line.StartsWith("---"))
            {
                oldBuilder.AppendLine(line.Substring(1));
            }
            else if (line.StartsWith("+") && !line.StartsWith("+++"))
            {
                newBuilder.AppendLine(line.Substring(1));
            }
            else
            {
                // Context line → block boundary
                FlushReplacement(oldBuilder, newBuilder, replacements);
            }
        }

        // Final flush
        FlushReplacement(oldBuilder, newBuilder, replacements);

        return replacements;
    }

    void FlushReplacement(StringBuilder oldBuilder, StringBuilder newBuilder, List<TextReplacement> replacements)
    {
        if (oldBuilder.Length == 0 && newBuilder.Length == 0)
            return;

        var oldText = oldBuilder.ToString().TrimEnd('\r', '\n');
        var newText = newBuilder.ToString().TrimEnd('\r', '\n');

        replacements.Add(new TextReplacement
        {
            OldText = oldText,
            NewText = newText
        });

        oldBuilder.Clear();
        newBuilder.Clear();
    }
}