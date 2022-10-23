using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to replace all the variables in a response body with its corresponding variable
///     parsing handler.
/// </summary>
public interface IResponseVariableParser
{
    /// <summary>
    ///     Replaces all variables in the response body with its corresponding variable parsing handler.
    /// </summary>
    /// <param name="input">The response body.</param>
    /// <param name="stub">The matched stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed response body.</returns>
    Task<string> ParseAsync(string input, StubModel stub, CancellationToken cancellationToken);
}
