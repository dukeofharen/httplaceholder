using System;
using System.Threading.Tasks;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A static class that contains several validation utilities.
/// </summary>
public static class ValidationUtilities
{
    /// <summary>
    ///     A method that executes a given action if the passed task returns a null value.
    /// </summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="action">The action to be executed if the task returns null.</param>
    /// <typeparam name="T">The return type of the task.</typeparam>
    /// <returns>The result of the executed task.</returns>
    public static async Task<T> IfNull<T>(this Task<T> task, Action action)
    {
        var result = await task;
        if (result == null)
        {
            action.Invoke();
        }

        return result;
    }

    /// <summary>
    ///     A method that executes a given action if the passed value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="action">The action to be executed if the task returns null.</param>
    /// <typeparam name="T">The return type of the task.</typeparam>
    /// <returns>The result of the executed task.</returns>
    public static T IfNull<T>(this T value, Action action)
    {
        if (value == null)
        {
            action.Invoke();
        }

        return value;
    }
}
