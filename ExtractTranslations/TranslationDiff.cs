namespace ExtractTranslations;

static class TranslationDiff
{
    public static (string cn, string en) RemoveCommon(string cn, string en)
    {
        var cnTokens = cn.ToCharArray().Select(c => c.ToString()).ToList();
        var enTokens = en.Split(' ').ToList();

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

    private static List<string> LongestCommonSubsequence(
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