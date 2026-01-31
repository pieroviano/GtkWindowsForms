using System.Text;
using System.Text.RegularExpressions;

namespace ExtractTranslations;

public static class CommentExtractor
{
    private static readonly Regex MethodRegex =
        new Regex(
            @"(?<return>[\w\<\>\[\]\.?]+)\s+" +
            @"(?<name>\w+)\s*" +
            @"\((?<params>[^\)]*)\)",
            RegexOptions.Compiled);

    public static List<ExtractedComment> Extract(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var results = new List<ExtractedComment>();

        var pendingComments = new StringBuilder();
        string? pendingKind = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            // ---------- XML doc comment ----------
            if (line.StartsWith("///"))
            {
                continue;
            }

            // ---------- Single-line comment ----------
            if (line.StartsWith("//"))
            {
                pendingKind ??= "Line";
                pendingComments.AppendLine(
                    line.Substring(2).Trim());
                continue;
            }

            // ---------- Block comment start ----------
            if (line.StartsWith("/*"))
            {
                pendingKind ??= "Block";
                pendingComments.AppendLine(
                    line.TrimStart('/', '*').Trim());
                continue;
            }

            // ---------- Block comment end ----------
            if (pendingKind == "Block" && line.EndsWith("*/"))
            {
                pendingComments.AppendLine(
                    line.TrimEnd('*', '/').Trim());
                continue;
            }

            // ---------- Method declaration ----------
            var match = MethodRegex.Match(line);
            if (match.Success && pendingComments.Length > 0)
            {
                var signature = BuildMethodSignature(match);

                results.Add(new ExtractedComment
                {
                    Text = pendingComments.ToString().Trim(),
                    Kind = pendingKind!,
                    MethodSignature = signature
                });

                pendingComments.Clear();
                pendingKind = null;
                continue;
            }

            // ---------- Reset on code ----------
            if (pendingComments.Length > 0 && line.Length > 0)
            {
                results.Add(new ExtractedComment
                {
                    Text = pendingComments.ToString().Trim(),
                    Kind = pendingKind!,
                    MethodSignature = null
                });

                pendingComments.Clear();
                pendingKind = null;
            }
        }

        return results;
    }

    // ---------------- helpers ----------------

    private static string BuildMethodSignature(Match match)
    {
        var name = match.Groups["name"].Value;
        var paramText = match.Groups["params"].Value;

        var paramTypes =
            paramText.Length == 0
                ? Array.Empty<string>()
                : paramText
                    .Split(',')
                    .Select(p => ExtractType(p.Trim()))
                    .Where(t => t.Length > 0)
                    .ToArray();

        return $"{name}({string.Join(", ", paramTypes)})";
    }

    private static string ExtractType(string parameter)
    {
        // Remove attributes
        parameter = Regex.Replace(parameter, @"\[[^\]]+\]\s*", "");

        // Remove default values
        parameter = Regex.Replace(parameter, @"\s*=.*$", "");

        // Remove modifiers
        parameter = Regex.Replace(
            parameter,
            @"\b(ref|out|in|params)\b\s*",
            "");

        var parts = parameter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? parts[0]
            : parameter;
    }
}
