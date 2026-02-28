namespace Prepare.Common.Text;

public static class ConsoleUtilityEx
{
    private static bool _hasWritten;

    private static object lockObject = new object();

    public static void WriteInColor(string? text, ConsoleColor foregroundColor, bool addNewLine = false)
    {
        lock (lockObject)
        {
            text ??= string.Empty;

            string formatted;
            if (!addNewLine)
            {
                int width = GetConsoleWidthSafe();
                formatted = FormatToConsoleWidth(text, width);
            }
            else
            {
                formatted = text;
            }

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            if (_hasWritten && !Console.IsOutputRedirected && !addNewLine)
            {
                // Move cursor to beginning of current line
                Console.SetCursorPosition(0, Console.CursorTop);
            }

            if (addNewLine)
            {
                Console.WriteLine();
            }

            if (addNewLine)
            {
                Console.WriteLine(formatted);
            }
            else
            {
                Console.Write(formatted);
            }

            Console.ForegroundColor = originalColor;

            _hasWritten = true;
        }
    }

    private static int GetConsoleWidthSafe()
    {
        if (!Console.IsOutputRedirected)
            return Console.WindowWidth;

        return 80;
    }

    private static string FormatToConsoleWidth(string text, int width)
    {
        if (width <= 0)
            return string.Empty;

        if (text.Length == width)
            return text;

        if (text.Length < width)
            return text.PadRight(width, ' ');

        if (width <= 3)
            return new string('.', width);

        const string ellipsis = "...";
        int remaining = width - ellipsis.Length;

        int part1Length = remaining / 2;
        int part2Length = remaining - part1Length;

        string part1 = text.Substring(0, part1Length);
        string part2 = text.Substring(text.Length - part2Length);

        return part1 + ellipsis + part2;
    }
}
