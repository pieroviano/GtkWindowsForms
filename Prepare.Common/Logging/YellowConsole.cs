namespace Prepare.Common.Logging;

public static class YellowConsole
{
    public static void Write(string? text)
    {
        ConsoleColor.Yellow.Write(text, true);
    }
}
