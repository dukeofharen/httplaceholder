using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to validate a stub.
/// </summary>
public interface IStubModelValidator
{
    /// <summary>
    ///     Validates a <see cref="StubModel" />.
    /// </summary>
    /// <param name="stub">The stub to validate.</param>
    /// <returns>A list of validation messages.</returns>
    IEnumerable<string> ValidateStubModel(StubModel stub);
}
