using System.Text.RegularExpressions;

namespace ExtractTranslations;

static class TextNormalize
{
    public static string Normalize(string text)
    {
        text = Regex.Replace(text, "<.*?>", ""); // remove XML tags
        text = text.Replace("：", ":");
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    public static IEnumerable<string> Split(string text)
    {
        return Regex
            .Split(text, @"[\.!\?。！？;\n\r]")
            .Select(s => s.Trim())
            .Where(s => s.Length > 0);
    }
}