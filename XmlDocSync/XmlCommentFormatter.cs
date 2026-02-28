using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XmlDocSync;

public static class XmlCommentFormatter
{
    const string LF = "\n";

    /// <summary>
    /// Formats an XML comment:
    /// - normalize newlines to LF
    /// - move inline text after opening tags to the next comment line
    /// - merge consecutive comment text lines while keeping merged content under 80 chars
    /// - stop merging at non-self-closing opening tags and at closing tags
    /// - collapse certain tag-start and tag-in-middle continuations into previous lines
    /// </summary>
    public static string FormatXmlComment(string xmlComment)
    {
        if (xmlComment is null)
            return null;

        // Normalize all newline kinds to '\n' so comparisons are deterministic on Windows/Unix.
        xmlComment = xmlComment.Replace("\n\r", LF).Replace("\r\n", LF).Replace("\r", LF);

        var lines = xmlComment.Split(new[] { LF }, StringSplitOptions.None).Where(i=>!string.IsNullOrEmpty(i)).ToArray();
        var moved = MoveTextAfterOpenTags(lines);
        var merged = MergeTextLines(moved);
        var collapsed = CollapseSelfClosingTagStarts(merged);
        var collapsedMid = MergeSelfClosingMidlineContinuations(collapsed);

        var formatXmlComment = string.Join(LF, collapsedMid);
        return Normalize($"{formatXmlComment}\n");
    }

    // Split lines like "/// <tag>Inline text" into "/// <tag>" and "/// Inline text"
    private static List<string> MoveTextAfterOpenTags(string[] lines)
    {
        var outLines = new List<string>();

        foreach (var raw in lines)
        {
            var line = raw ?? string.Empty;

            // Capture prefix (including trailing space), tag, and rest.
            var m = Regex.Match(line, @"^([ \t]*///\s*)(<[^>]+>)(\s*.*)$");
            if (m.Success)
            {
                var prefix = m.Groups[1].Value;
                var tag = m.Groups[2].Value;
                var rest = m.Groups[3].Value;

                var tagTrim = tag.Trim();

                // Do not split closing tags or self-closing tags
                if (tagTrim.StartsWith("</") || tagTrim.EndsWith("/>"))
                {
                    outLines.Add(line);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(rest))
                {
                    var text = rest.TrimStart();
                    outLines.Add($"{prefix}{tag}");
                    outLines.Add($"{prefix}{text}");
                    continue;
                }
            }

            outLines.Add(line);
        }

        return outLines;
    }

    // Merge text lines while:
    // - stopping at a closing-tag-only line (</...>) or opening non-self-closing tag-only (<...>)
    // - allowing self-closing tags (<... />) to be merged inline
    // - enforcing merged content length < 80 characters (content only; prefix not counted)
    private static List<string> MergeTextLines(List<string> lines)
    {
        var outLines = new List<string>();
        int i = 0;

        // Track open tags stack
        var openTags = new Stack<string>();

        while (i < lines.Count)
        {
            var line = lines[i] ?? string.Empty;
            var m = Regex.Match(line, @"^([ \t]*///)(?:\s*)(.*)$");
            if (!m.Success)
            {
                outLines.Add(line);
                i++;
                continue;
            }

            var prefix = m.Groups[1].Value;
            var content = m.Groups[2].Value.TrimStart();

            // Check for tag-only lines (opening or closing)
            var trimmed = content.Trim();
            bool isOpeningTagOnly = !string.IsNullOrEmpty(trimmed) &&
                                    trimmed.StartsWith("<") &&
                                    trimmed.EndsWith(">") &&
                                    !trimmed.EndsWith("/>") &&
                                    !trimmed.StartsWith("</");

            bool isClosingTagOnly = trimmed.StartsWith("</") && trimmed.EndsWith(">");

            // Update open tag stack
            if (isOpeningTagOnly)
            {
                // Push tag name (without < >)
                var tagName = Regex.Match(trimmed, @"<(\w+)").Groups[1].Value;
                openTags.Push(tagName);
                outLines.Add(line);
                i++;
                continue;
            }

            if (isClosingTagOnly)
            {
                if (openTags.Count > 0)
                    openTags.Pop();
                outLines.Add(line);
                i++;
                continue;
            }

            // Only merge if we are inside a tag block
            if (openTags.Count == 0 || string.IsNullOrWhiteSpace(content))
            {
                outLines.Add(line);
                i++;
                continue;
            }

            // Merge consecutive text lines inside current open tag block
            var merged = new StringBuilder(content);
            int j = i + 1;

            while (j < lines.Count)
            {
                var nextLine = lines[j] ?? string.Empty;
                var nm = Regex.Match(nextLine, @"^([ \t]*///)(?:\s*)(.*)$");
                if (!nm.Success)
                    break;

                var nextContent = nm.Groups[2].Value.TrimStart();
                var nextTrim = nextContent.Trim();

                // Stop if next line contains a closing-tag-only or opening non-self-closing tag-only
                bool nextIsOpeningTagOnly = !string.IsNullOrEmpty(nextTrim) &&
                                            nextTrim.StartsWith("<") &&
                                            nextTrim.EndsWith(">") &&
                                            !nextTrim.EndsWith("/>") &&
                                            !nextTrim.StartsWith("</");

                bool nextIsClosingTagOnly = nextTrim.StartsWith("</") && nextTrim.EndsWith(">");

                if (nextIsOpeningTagOnly || nextIsClosingTagOnly)
                    break;

                // Stop if merged content exceeds 80 characters
                if (merged.Length + 1 + nextContent.Length >= 80)
                    break;

                if (!string.IsNullOrEmpty(nextContent))
                    merged.Append(" ").Append(nextContent);

                j++;
            }

            outLines.Add($"{prefix} {merged.ToString().TrimEnd()}");
            i = j;
        }

        return outLines;
    }
    // Post-process: if a comment line starts with a self-closing tag and then has trailing text,
    // and the previous line is a normal text comment line, merge them into a single line.
    private static List<string> CollapseSelfClosingTagStarts(List<string> lines)
    {
        var outLines = new List<string>();
        int i = 0;
        while (i < lines.Count)
        {
            if (i + 1 < lines.Count)
            {
                var cur = lines[i] ?? string.Empty;
                var next = lines[i + 1] ?? string.Empty;

                var curMatch = Regex.Match(cur, @"^([ \t]*///)(?:\s*)(.*\S)\s*$");
                var nextMatch = Regex.Match(next, @"^([ \t]*///\s*)(<[^>]+/>\s+.+)$");

                if (curMatch.Success && nextMatch.Success)
                {
                    var prefix = curMatch.Groups[1].Value;
                    var curContent = curMatch.Groups[2].Value.TrimEnd();
                    var nextTagAndRest = nextMatch.Groups[2].Value.Trim();

                    outLines.Add($"{prefix} {curContent} {nextTagAndRest}");
                    i += 2;
                    continue;
                }
            }

            outLines.Add(lines[i]);
            i++;
        }

        return outLines;
    }

    // New post-process: if a line contains a self-closing tag (anywhere) and the following line
    // is a continuation text line (not tag-only), merge them (respecting the 80-char rule).
    private static List<string> MergeSelfClosingMidlineContinuations(List<string> lines)
    {
        var outLines = new List<string>();
        int i = 0;
        while (i < lines.Count)
        {
            if (i + 1 < lines.Count)
            {
                var cur = lines[i] ?? string.Empty;
                var next = lines[i + 1] ?? string.Empty;

                var curMatch = Regex.Match(cur, @"^([ \t]*///)(?:\s*)(.*\S)\s*$");
                var nextMatch = Regex.Match(next, @"^([ \t]*///)(?:\s*)(.*\S)\s*$");

                if (curMatch.Success && nextMatch.Success)
                {
                    var prefix = curMatch.Groups[1].Value;
                    var curContent = curMatch.Groups[2].Value.TrimEnd();
                    var nextContent = nextMatch.Groups[2].Value.Trim();

                    // current line must contain a self-closing tag anywhere
                    bool curHasSelfClosing = Regex.IsMatch(curContent, @"<[^>]+/>\s*");
                    // next line must NOT be tag-only (opening/closing) — allow normal text continuation
                    bool nextIsTagOnly = nextContent.StartsWith("<") && nextContent.EndsWith(">");

                    if (curHasSelfClosing && !nextIsTagOnly)
                    {
                        // Respect 80-char limit for merged content (content-only)
                        if (curContent.Length + 1 + nextContent.Length < 80)
                        {
                            outLines.Add($"{prefix} {curContent} {nextContent}");
                            i += 2;
                            continue;
                        }
                    }
                }
            }

            outLines.Add(lines[i]);
            i++;
        }

        return outLines;
    }

    private static string Normalize(string s) =>
        s is null ? null : s.Replace("\r\n", "\n").Replace("\r", "\n");


}