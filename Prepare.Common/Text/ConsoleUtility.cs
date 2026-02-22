namespace Prepare.Common.Text;

public static class ConsoleUtility
{
    public static void WriteInGreen(string? text)
    {
        var previousColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ForegroundColor = previousColor;
    }
}