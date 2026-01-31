using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ExtractTranslations;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = ["old.xml", "new.xml", "translations"];
        }

        if (args.Length == 1)
        {
            args = [args[0], "new.xml", "translations"];
        }

        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ExtractTranslations <ChineseFile> <EnglishFile>");
            return 1;
        }

        var chineseFile = args[0];
        var englishFile = args[1];
        var destination = args[2];
        if (chineseFile.StartsWith("samples"))
        {
            destination = "samplesTranslations";
            if (chineseFile.StartsWith("samples") && !englishFile.StartsWith("samples"))
            {
                englishFile = $"samples{englishFile[0].ToString().ToUpper()}{englishFile.Substring(1)}";
            }
        }

        if (!File.Exists(chineseFile) || !File.Exists(englishFile))
        {
            Console.WriteLine("One or both input files do not exist.");
            return 1;
        }

        List<ExtractedComment> cnComments;
        List<ExtractedComment> enComments;
        if (args[0].EndsWith(".xml"))
        {
            cnComments = XmlDocCommentExtractor.Extract(chineseFile);
            enComments = XmlDocCommentExtractor.Extract(englishFile);
        }
        else
        {
            cnComments = CommentExtractor.Extract(chineseFile);
            enComments = CommentExtractor.Extract(englishFile);
        }

        var count = Math.Min(cnComments.Count, enComments.Count);

        string outputPath = @$"..\..\..\..\PrepareProjects\{destination}.xml";

        // Load or create translation memory
        var doc = File.Exists(outputPath)
            ? XDocument.Load(outputPath)
            : new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Translations"));

        var root = doc.Root!;

        // Existing translations (Chinese → English)
        var translations = root
            .Elements("Entry")
            .ToDictionary(
                e => e.Element("Chinese")!.Value,
                e => e.Element("English")!.Value);

        int added = 0;

        var enByMethod = enComments
            .Where(c => c.MethodSignature != null)
            .GroupBy(c => (c.MethodSignature, c.Kind))
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var cnC in cnComments)
        {
            if (cnC.MethodSignature == null)
                continue;

            if (!enByMethod.TryGetValue(
                    (cnC.MethodSignature, cnC.Kind),
                    out var enC))
                continue;

            var cnText = Normalize(cnC.Text);
            var enText = Normalize(enC.Text);
            if (!TextUtil.ContainsChinese(cnText))
                continue;

            var cnParts = Split(cnText).ToList();
            var enParts = Split(enText).ToList();

            var partCount = Math.Min(cnParts.Count, enParts.Count);

            for (int p = 0; p < partCount; p++)
            {
                var (cn, en) = RemoveCommonParts(cnParts[p], enParts[p]);

                if (cn.Length == 0 || en.Length == 0)
                    continue;

                if (translations.ContainsKey(cn))
                    continue;

                translations[cn] = en;
                root.Add(
                    new XElement("Entry",
                        new XElement("Chinese", cn),
                        new XElement("English", en)));

                added++;
            }
        }

        doc.Save(outputPath);

        Console.WriteLine($"Added {added} new entries.");
        Console.WriteLine($"Total entries: {translations.Count}");
        Console.WriteLine($"Saved to: {Path.GetFullPath(outputPath)}");

        return 0;
    }

    // ---------------- helpers ----------------

    static string Normalize(string text)
    {
        text = Regex.Replace(text, "<.*?>", "");
        text = text.Replace("：", ":");
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    static IEnumerable<string> Split(string text)
    {
        return Regex
            .Split(text, @"[\.!\?。！？;\r\n]")
            .Select(s => s.Trim())
            .Where(s => s.Length > 0);
    }

    static (string cn, string en) RemoveCommonParts(string cn, string en)
    {
        var cnTokens = cn.ToCharArray().Select(c => c.ToString()).ToList();
        var enTokens = en.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        var lcs = LongestCommonSubsequence(cnTokens, enTokens);

        foreach (var token in lcs)
        {
            cnTokens.Remove(token);
            enTokens.Remove(token);
        }

        return (
            string.Concat(cnTokens).Trim(),
            string.Join(' ', enTokens).Trim()
        );
    }

    static List<string> LongestCommonSubsequence(
        List<string> a,
        List<string> b)
    {
        var dp = new int[a.Count + 1, b.Count + 1];

        for (int i = 1; i <= a.Count; i++)
            for (int j = 1; j <= b.Count; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1] + 1
                    : Math.Max(dp[i - 1, j], dp[i, j - 1]);

        var result = new List<string>();
        int x = a.Count, y = b.Count;

        while (x > 0 && y > 0)
        {
            if (a[x - 1] == b[y - 1])
            {
                result.Add(a[x - 1]);
                x--; y--;
            }
            else if (dp[x - 1, y] > dp[x, y - 1])
                x--;
            else
                y--;
        }

        result.Reverse();
        return result;
    }
}