namespace Prepare.Common.Content;

public static class Normalizer
{
    public static void NormalizeNetstandardSymbolRemovingVersion(string targetDirectory)
    {
        const string oldText = "\n#if NETSTANDARD2_0";
        const string newTextConst = "\n#if NETSTANDARD";

        NormalizeNetstandardSymbolRemovingVersion(targetDirectory, oldText, newTextConst);
        NormalizeNetstandardSymbolRemovingVersion(targetDirectory, oldText.TrimStart('\n'), newTextConst.TrimStart('\n'));
    }

    private static void NormalizeNetstandardSymbolRemovingVersion(string targetDirectory, string oldText, string newTextConst)
    {
        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);

            if (!text.Contains(oldText))
            {
                continue;
            }

            var newText = text.Replace(oldText, newTextConst);

            if (!ReferenceEquals(text, newText))
            {
                CsFile.WriteAllText(file, newText);
            }
        }
    }
}