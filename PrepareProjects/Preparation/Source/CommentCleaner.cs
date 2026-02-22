namespace PrepareProjects.Preparation.Source;

public static class CommentCleaner
{
    public static string RemoveCommentBlock(string content, string comment)
    {
        var lines = content
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            int index = lines[i].IndexOf(comment, StringComparison.Ordinal);
            if (index < 0)
                continue;

            // CASE 1: Inline comment (code before it)
            if (!lines[i].TrimStart().StartsWith("//"))
            {
                lines[i] = lines[i].Substring(0, index).TrimEnd();
                continue; // keep scanning next lines
            }

            // CASE 2: Comment block line
            int start = i;
            while (start >= 0 && lines[start].TrimStart().StartsWith("//"))
                start--;
            start++;

            int end = i;
            while (end < lines.Count && lines[end].TrimStart().StartsWith("//"))
                end++;
            end--;

            lines.RemoveRange(start, end - start + 1);

            // IMPORTANT:
            // Move index back to start so we don't skip lines
            i = start - 1;
        }

        return string.Join(Environment.NewLine, lines);
    }
}