using System.Text;
using Prepare.Common.Content;

namespace Prepare.Common.Languages;

public static class Translator
{
    public static void ProcessFileTranslation(string fileToTranslate,
        Func<string, string?, string, string> patchProjectNamespace,
        string? xmlTranslationFileName)
    {
        var xmlTranslationPath = Prepare.Common.IO.RepoPaths.At($@"Resources\{xmlTranslationFileName}.xml");
        var xmlTranslations = TranslationsHelper.LoadTranslations(xmlTranslationPath);
        TranslateFile(null, null, fileToTranslate, null,
            patchProjectNamespace, null, xmlTranslations);
    }

    public static void ReplaceStringsInFiles(string? sourceProjectName,
        string targetDirectory, string? targetProjectName, string? localizationResourceName,
        Func<string, string?, string, string> patchProjectNamespace,
        IReadOnlyList<KeyValuePair<string, string>> localizableItemsOrderedByValueLengthDecending,
        List<KeyValuePair<string, string>> rawTranslations)
    {
        if (!Directory.Exists(targetDirectory))
        {
            throw new DirectoryNotFoundException(targetDirectory);
        }

        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*.cs", SearchOption.AllDirectories))
        {
            sourceProjectName = TranslateFile(sourceProjectName,
                targetProjectName, file, localizationResourceName, patchProjectNamespace,
                localizableItemsOrderedByValueLengthDecending, rawTranslations);
        }
    }

    private static (string? originalText, string? updatedText) GetOriginalUpdatedText(string file)
    {
        if (!File.Exists(file))
        {
            return (null, null);
        }

        var originalText = File.ReadAllText(file);

        return (originalText, originalText);
    }

    private static string? MakeRawTranslationOnFile(string file,
        string? fileContent, List<KeyValuePair<string, string>> translations)
    {
        foreach (var translation in translations)
        {
            var cn = translation.Key;
            var en = translation.Value;
            // Skip empty Chinese, and skip empty-English (untranslated) entries so untranslated
            // Chinese is left in place instead of being erased.
            if (string.IsNullOrEmpty(cn) || string.IsNullOrEmpty(en))
            {
                continue;
            }

            if (Path.GetExtension(file) == ".cs")
            {
                // Perform case-insensitive search/replacement so we can inspect the previous character.
                var sb = new StringBuilder();
                var searchStart = 0;
                while (true)
                {
                    var idx = fileContent?.IndexOf(cn, searchStart, StringComparison.OrdinalIgnoreCase) ?? -1;
                    if (idx == -1)
                    {
                        // append remainder and finish
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        sb.Append(fileContent.Substring(searchStart));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        break;
                    }

                    // append text before match
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    sb.Append(fileContent.Substring(searchStart, idx - searchStart));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    // decide replacement form based on previous character
                    var shouldPrefixSpaceAndLower = false;
                    if (idx > 0)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        var prev = fileContent[idx - 1];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        // If previous char is not '.' and is alphanumeric or punctuation -> apply rule
                        if (prev != '.' && (char.IsLetterOrDigit(prev) || char.IsPunctuation(prev)))
                        {
                            shouldPrefixSpaceAndLower = true;
                        }
                    }

                    var replacement = en ?? string.Empty;
                    if (shouldPrefixSpaceAndLower && replacement.Length > 0)
                    {
                        // make first letter lowercase if it's a letter
                        var first = replacement[0];
                        if (char.IsLetter(first))
                        {
                            replacement = char.ToLowerInvariant(first) + replacement.Substring(1);
                        }

                        // prefix a single space
                        replacement = " " + replacement;
                    }

                    sb.Append(replacement);

                    // advance search past the matched source text
                    searchStart = idx + cn.Length;
                }

                fileContent = sb.ToString();
            }
            else
            {
                if (cn != null)
                {
                    fileContent = fileContent.ReplaceIgnoreCase(cn, en ?? string.Empty);
                }
            }
        }

        return fileContent;
    }

    private static string ProcessResourceTranslation(ref string? sourceProjectName,
        string? targetProjectName, string? localizationResourceName, string fileContent,
        Func<string, string?, string, string> patchProjectNamespace,
        IReadOnlyList<KeyValuePair<string, string>> localizableItemsOrderedByValueLengthDecending)
    {
        sourceProjectName ??= string.Empty;
        foreach (var pair in localizableItemsOrderedByValueLengthDecending)
        {
            var literal = pair.Value;

            if (string.IsNullOrEmpty(literal))
            {
                continue;
            }

            var replacement =
                $"Properties.{localizationResourceName}.{pair.Key}";

            if (fileContent.Contains($"\"{literal}\""))
            {
                fileContent = fileContent.Replace($"\"{literal}\"", replacement);
            }

            if (patchProjectNamespace != null)
            {
                fileContent = patchProjectNamespace.Invoke(sourceProjectName, targetProjectName, fileContent);
            }
        }

        return fileContent;
    }

    private static string? TranslateFile(string? sourceProjectName, string? targetProjectName, string fileToTranslate,
        string? localizationResourceName, Func<string, string?, string, string> patchProjectNamespace,
        IReadOnlyList<KeyValuePair<string, string>>? localizableItemsOrderedByValueLengthDecending,
        List<KeyValuePair<string, string>> rawTranslations)
    {
        var (originalText, updatedText) = GetOriginalUpdatedText(fileToTranslate);
        if (originalText == null)
        {
            return sourceProjectName;
        }

        if (localizableItemsOrderedByValueLengthDecending != null)
        {
            updatedText = ProcessResourceTranslation(ref sourceProjectName, targetProjectName,
                localizationResourceName, updatedText ?? "", patchProjectNamespace,
                localizableItemsOrderedByValueLengthDecending);
        }

        updatedText = MakeRawTranslationOnFile(fileToTranslate, updatedText, rawTranslations);

        if (!ReferenceEquals(originalText, updatedText) &&
            originalText != updatedText)
        {
            CsFile.WriteAllText(fileToTranslate, updatedText ?? string.Empty);
        }

        return sourceProjectName;
    }
}
