using System;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class that contains several console helper methods.
/// </summary>
public static class ConsoleHelpers
{
    /// <summary>
    /// Writes a line to the console in a specific color.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="foregroundColor">The foreground color.</param>
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
