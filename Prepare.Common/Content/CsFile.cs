using System.Text;

namespace Prepare.Common.Content;

public class CsFile
{
    public static void WriteAllText(string path, string contents)
    {
        File.WriteAllText(path, contents.Replace("\r\n", "\n"));
    }

    public static void WriteAllLines(string path, string[] strings)
    {
        File.WriteAllLines(path, strings.Select(s => s.Replace("\r\n", "\n")));
    }

    public static void WriteAllText(string path, string contents, Encoding encoding)
    {
        File.WriteAllText(path, contents.Replace("\r\n", "\n"), encoding);
    }

    public static Task WriteAllTextAsync(string path, string? contents)
    {
        File.WriteAllText(path, (contents ?? String.Empty).Replace("\r\n", "\n"));
        return Task.CompletedTask;
    }
}
