namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to check whether a string provided through a request condition from the stub is correct.
/// </summary>
public interface IStringChecker
{
    /// <summary>
    /// Checks whether the given input is correct according to the condition.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <param name="condition">The condition to check against.</param>
    /// <returns>True if the condition passed; false otherwise.</returns>
    bool CheckString(string input, object condition);
}
