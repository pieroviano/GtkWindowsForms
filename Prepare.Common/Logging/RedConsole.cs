namespace Prepare.Common.Logging;

public static class RedConsole
{
    public static void Write(string? text)
    {
        ConsoleColor.Red.Write(text, true, new object[0]);
    }
}
