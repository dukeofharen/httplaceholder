using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to replace all the variables in a response body with its corresponding variable parsing handler.
/// </summary>
public interface IResponseVariableParser
{
    /// <summary>
    /// Replaces all variables in the response body with its corresponding variable parsing handler.
    /// </summary>
    /// <param name="input">The response body.</param>
    /// <param name="stub">The matched stub.</param>
    /// <returns>The parsed response body.</returns>
    string Parse(string input, StubModel stub);
}
