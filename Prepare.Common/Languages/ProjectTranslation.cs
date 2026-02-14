using Prepare.Common.IO;

namespace Prepare.Common.Languages;

public static class ProjectTranslation
{
    public static void TranslateFiles(string? sourceProjectName, string? targetProjectName, string targetDirectory,
        Func<string, string?, string, string> patchProjectNamespace, string? localizationResourceName, string? rawTranslationsName)
    {
        var localizableItemsOrderedByValueLengthDecending = ResxReader.ReadLocalizableItemsOrderedByValueLengthDecending(Path.Combine(targetDirectory, "Properties",
            $"{localizationResourceName}.zh-Hans.resx"));
        var rawTranslations = LoadRawTranslations(rawTranslationsName);
        Translator.ReplaceStringsInFiles(sourceProjectName, targetDirectory, targetProjectName, 
            localizationResourceName, patchProjectNamespace, localizableItemsOrderedByValueLengthDecending, rawTranslations);
    }

    private static List<KeyValuePair<string, string>> LoadRawTranslations(string? rawTranslationsName)
    {
        var translationsPath = Path.GetFullPath(@$"..\..\..\{rawTranslationsName}.xml");
        var rawTranslations = TranslationsHelper.LoadTranslations(translationsPath);
        return rawTranslations;
    }
}