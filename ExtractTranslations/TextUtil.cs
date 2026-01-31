using System.Text.RegularExpressions;

namespace ExtractTranslations;

static class TextUtil
{
    private static readonly Regex ChineseRegex =
        new(@"[\u4e00-\u9fff]", RegexOptions.Compiled);

    public static bool ContainsChinese(string text) =>
        ChineseRegex.IsMatch(text);
}