namespace Prepare.Common.Logging;

public static class YellowConsole
{
    public static void Write(string? text)
    {
        ColoredConsole.WriteInColor(text, ConsoleColor.Yellow, true);
    }
}
