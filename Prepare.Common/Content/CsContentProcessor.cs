using System.Text.RegularExpressions;

namespace Prepare.Common;

public static class CsContentProcessor
{
    public static string? ExtractNamespace(string content)
    {
        // File-scoped namespace: namespace My.Namespace;
        var fileScoped = Regex.Match(content,
            @"namespace\s+([A-Za-z0-9_.]+)\s*;",
            RegexOptions.Multiline);

        if (fileScoped.Success)
            return fileScoped.Groups[1].Value;

        // Classic namespace: namespace My.Namespace { ... }
        var classic = Regex.Match(content,
            @"namespace\s+([A-Za-z0-9_.]+)\s*{",
            RegexOptions.Multiline);

        if (classic.Success)
            return classic.Groups[1].Value;

        return null;
    }
}