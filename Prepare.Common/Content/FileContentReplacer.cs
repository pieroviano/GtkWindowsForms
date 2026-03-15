namespace Prepare.Common.Content;

public class FileContentReplacer
{
    public static void ReplaceContent(string sourcePath, string destinationPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source file not found.", sourcePath);

        if (!File.Exists(destinationPath))
            throw new FileNotFoundException("Destination file not found.", destinationPath);

        string content = File.ReadAllText(sourcePath);
        ConsoleColor.White.Write($@"Replacing {destinationPath} with {sourcePath}");
        File.WriteAllText(destinationPath, content);
    }
}