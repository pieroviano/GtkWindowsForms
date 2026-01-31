using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ExtractTranslations;

public static class XmlDocCommentExtractor
{
    public static List<ExtractedComment> Extract(string xmlDocPath)
    {
        var doc = XDocument.Load(xmlDocPath);

        var members = doc
            .Root?
            .Element("members")?
            .Elements("member");

        if (members == null)
            return new List<ExtractedComment>();

        var results = new List<ExtractedComment>();

        foreach (var member in members)
        {
            var nameAttr = member.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(nameAttr))
                continue;

            var signature = BuildMemberSignature(nameAttr);

            // -------- summary --------
            var summary = member.Element("summary")?.Value;
            if (!string.IsNullOrWhiteSpace(summary))
            {
                results.Add(new ExtractedComment
                {
                    Text = Normalize(summary),
                    Kind = "Summary",
                    MethodSignature = signature
                });
            }

            // -------- parameters --------
            foreach (var param in member.Elements("param"))
            {
                var paramText = param.Value;
                if (string.IsNullOrWhiteSpace(paramText))
                    continue;

                results.Add(new ExtractedComment
                {
                    Text = Normalize(paramText),
                    Kind = "Param",
                    MethodSignature = signature
                });
            }
        }

        return results;
    }

    // ---------------- helpers ----------------

    private static string BuildMemberSignature(string xmlName)
    {
        // xmlName examples:
        // T:Namespace.Type
        // P:Namespace.Type.Property
        // M:Namespace.Type.Method(Type1,Type2)

        var colonIndex = xmlName.IndexOf(':');
        if (colonIndex < 0)
            return xmlName;

        var kind = xmlName[..colonIndex];
        var body = xmlName[(colonIndex + 1)..];

        if (kind != "M")
            return body;

        // Method: extract name + param types
        var match = Regex.Match(
            body,
            @"^(?<type>.+?)\.(?<name>\w+)\((?<params>.*)\)$");

        if (!match.Success)
            return body;

        var name = match.Groups["name"].Value;
        var paramText = match.Groups["params"].Value;

        var paramTypes =
            string.IsNullOrWhiteSpace(paramText)
                ? Array.Empty<string>()
                : paramText
                    .Split(',')
                    .Select(ShortTypeName)
                    .ToArray();

        return $"{name}({string.Join(", ", paramTypes)})";
    }

    private static string ShortTypeName(string fullName)
    {
        // System.Boolean -> Boolean
        var lastDot = fullName.LastIndexOf('.');
        return lastDot >= 0
            ? fullName[(lastDot + 1)..]
            : fullName;
    }

    private static string Normalize(string text)
    {
        return Regex.Replace(
            text,
            @"\s+",
            " ")
            .Trim();
    }
}
