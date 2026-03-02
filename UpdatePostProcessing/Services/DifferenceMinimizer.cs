using UpdatePostProcessing.Model;

namespace UpdatePostProcessing.Services;

internal static class DifferenceMinimizer
{
    public static List<ToPersist> GetMinimalDiffs(this IEnumerable<ToPersist> rawDiffsFromGit)
    {
        var result = new List<ToPersist>();

        foreach (var change in rawDiffsFromGit)
        {
            if (change.SourceString.Length < 100 && change.TargetString.Length < 100)
            {
                result.Add(change);
            }
            else
            {
                var (sourceFragment, targetFragment) =
                    GetAnchoredMinimalDifference(change.SourceString, change.TargetString);
                if (!string.IsNullOrEmpty(sourceFragment))
                {
                    // Deduplicate logically identical diffs
                    if (!result.Any(r =>
                            r.SourceFileName == change.SourceFileName &&
                            r.TargetFileName == change.TargetFileName &&
                            r.SourceString == sourceFragment &&
                            r.TargetString == targetFragment))
                    {
                        result.Add(new ToPersist(
                            change.SourceFileName,
                            change.TargetFileName,
                            sourceFragment,
                            targetFragment));
                    }
                }
            }
        }

        return result;
    }

    private static (string sourceFragment, string targetFragment)
        GetAnchoredMinimalDifference(string sourceText, string targetText)
    {
        if (sourceText == targetText)
            return (string.Empty, string.Empty);

        var start = 0;
        var sourceLen = sourceText.Length;
        var targetLen = targetText.Length;

        // 1. Longest common prefix
        while (start < sourceLen &&
               start < targetLen &&
               sourceText[start] == targetText[start])
        {
            start++;
        }

        // 2. Longest common suffix
        var endSource = sourceLen - 1;
        var endTarget = targetLen - 1;

        while (endSource >= start &&
               endTarget >= start &&
               sourceText[endSource] == targetText[endTarget])
        {
            endSource--;
            endTarget--;
        }

        var deleted = sourceText.Substring(start, endSource - start + 1);
        var inserted = targetText.Substring(start, endTarget - start + 1);

        // 3. If we have a real deletion, it's already anchored
        if (!string.IsNullOrEmpty(deleted))
            return (deleted, inserted);

        // 4. INSERTION ONLY → need anchoring
        return AnchorInsertion(
            sourceText,
            targetText,
            start,
            inserted);
    }

    private static (string sourceFragment, string targetFragment)
        AnchorInsertion(
            string sourceText,
            string targetText,
            int insertPosition,
            string inserted)
    {
        // A. Try shortest unique PREFIX
        for (var i = insertPosition - 1; i >= 0; i--)
        {
            var prefix = sourceText.Substring(i, insertPosition - i);

            if (IsUnique(sourceText, prefix))
            {
                return (prefix, prefix + inserted);
            }
        }

        // B. Try shortest unique SUFFIX
        for (var len = 1; insertPosition + len <= sourceText.Length; len++)
        {
            var suffix = sourceText.Substring(insertPosition, len);

            if (IsUnique(sourceText, suffix))
            {
                return (suffix, inserted + suffix);
            }
        }

        // C. Fallback: grow both sides until unique
        for (var left = insertPosition - 1; left >= 0; left--)
        {
            for (var right = insertPosition + 1; right <= sourceText.Length; right++)
            {
                var context = sourceText.Substring(left, right - left);

                if (IsUnique(sourceText, context))
                {
                    var targetContext =
                        context.Insert(insertPosition - left, inserted);

                    return (context, targetContext);
                }
            }
        }

        throw new InvalidOperationException(
            "Unable to find a unique anchor for insertion.");
    }

    private static bool IsUnique(string text, string fragment)
    {
        if (string.IsNullOrEmpty(fragment))
            return false;

        var first = text.IndexOf(fragment, StringComparison.Ordinal);
        if (first == -1)
            return false;

        return first == text.LastIndexOf(fragment, StringComparison.Ordinal);
    }
}
