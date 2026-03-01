namespace Prepare.Common.Logging;

public static class RedConsole
{
    public static void Write(string? text)
    {
        ColoredConsole.WriteInColor(text, ConsoleColor.Red, true);
    }
}
