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
        string? lastContextLine = null;

        foreach (var line in lines)
        {
            if (line.StartsWith("\\ "))
                continue;
            
            if (line.StartsWith("@@"))
            {
                FlushReplacement(oldBuilder, newBuilder, replacements, lastContextLine);
                insideHunk = true;
                lastContextLine = null;
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
                // Context line
                FlushReplacement(oldBuilder, newBuilder, replacements, lastContextLine);

                lastContextLine = line; // store anchor
            }
        }

        // Final flush
        FlushReplacement(oldBuilder, newBuilder, replacements, lastContextLine);
        foreach (var replacement in replacements)
        {
            replacement.NewText = replacement.NewText.Replace("\r\n", "\n");
        }
        return replacements;
    }

    private void FlushReplacement(
        StringBuilder oldBuilder,
        StringBuilder newBuilder,
        List<TextReplacement> replacements,
        string? lastContextLine)
    {
        bool hasOld = oldBuilder.Length > 0;
        bool hasNew = newBuilder.Length > 0;

        if (!hasOld && !hasNew)
            return;

        var oldText = oldBuilder.ToString().TrimEnd('\r', '\n');
        var newText = newBuilder.ToString().TrimEnd('\r', '\n');

        // PURE INSERTION (even blank line)
        if (!hasOld && hasNew)
        {
            if (!string.IsNullOrEmpty(lastContextLine))
            {
                replacements.Add(new TextReplacement
                {
                    OldText = lastContextLine,
                    NewText = lastContextLine + Environment.NewLine + newText
                });
            }
        }
        else
        {
            replacements.Add(new TextReplacement
            {
                OldText = oldText,
                NewText = newText
            });
        }

        oldBuilder.Clear();
        newBuilder.Clear();
    }
}