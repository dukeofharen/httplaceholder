using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Describes a class that is used to read a response body and parse the variables.
///     A variable in the response body can be in the form of "((variable))" or "((variable:parameter))".
/// </summary>
public interface IResponseVariableParsingHandler
{
    /// <summary>
    ///     Gets or sets the variable parsing handler name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets or sets the variable parsing handler full name.
    /// </summary>
    string FullName { get; }

    /// <summary>
    ///     Gets or sets the variable parsing handler examples.
    /// </summary>
    string[] Examples { get; }

    /// <summary>
    ///     Gets the variable parsing handler description.
    /// </summary>
    string GetDescription();

    /// <summary>
    ///     Accepts a response body as input and parses the specific variable.
    /// </summary>
    /// <param name="input">The response body.</param>
    /// <param name="matches">The regex matches that have been found for this variable.</param>
    /// <param name="stub">The matched stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed response body.</returns>
    Task<string> ParseAsync(string input, IEnumerable<Match> matches, StubModel stub, CancellationToken cancellationToken);
}
