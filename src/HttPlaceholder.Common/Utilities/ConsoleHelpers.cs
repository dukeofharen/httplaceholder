using System;

namespace HttPlaceholder.Common.Utilities
{
    public static class ConsoleHelpers
    {
        public static void WriteLineColor(string message, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            var originalBackgroundColor = Console.BackgroundColor;
            var originalForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(message);

            Console.BackgroundColor = originalBackgroundColor;
            Console.ForegroundColor = originalForegroundColor;
        }
    }
}
