using System;
using System.Threading.Tasks;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A static class that contains several object utilities.
/// </summary>
public static class ObjectUtilities
{
    /// <summary>
    ///     A method that executes a given action if the passed task returns a null value.
    /// </summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="action">The action to be executed if the task returns null.</param>
    /// <typeparam name="T">The return type of the task.</typeparam>
    /// <returns>The result of the executed task.</returns>
    public static async Task<T> IfNullAsync<T>(this Task<T> task, Action action) => IfNull(await task, action);

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

    /// <summary>
    ///     A method that accepts a function and an action that will be executed if the function returns true.
    ///     Handy for one line fetches and checks.
    /// </summary>
    /// <param name="task">The input value.</param>
    /// <param name="func">The checking function.</param>
    /// <param name="action">The action to execute when the function returns true.</param>
    /// <returns>The original input value.</returns>
    public static async Task<T> IfAsync<T>(this Task<T> task, Func<T, bool> func, Action<T> action) =>
        If(await task, func, action);

    /// <summary>
    ///     A method that accepts a function and an action that will be executed if the function returns true.
    ///     Handy for one line fetches and checks.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="func">The checking function.</param>
    /// <param name="action">The action to execute when the function returns true.</param>
    /// <returns>The original input value.</returns>
    public static T If<T>(this T value, Func<T, bool> func, Action<T> action)
    {
        if (func(value))
        {
            action(value);
        }

        return value;
    }

    /// <summary>
    ///     Maps the input to the given output.
    /// </summary>
    /// <param name="task">The value to map.</param>
    /// <param name="func">A function to map the value.</param>
    /// <returns>The mapped value.</returns>
    public static async Task<TResult> MapAsync<T, TResult>(this Task<T> task, Func<T, TResult> func) => func(await task);

    /// <summary>
    ///     Maps the input to the given output.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="func">A function to map the value.</param>
    /// <returns>The mapped value.</returns>
    public static TResult Map<T, TResult>(this T value, Func<T, TResult> func) => func(value);
}
