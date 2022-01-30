using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is used to determine which stub should be used for the current request.
/// </summary>
public interface IFinalStubDeterminer
{
    /// <summary>
    /// Determine which stub should be used for the current request.
    /// </summary>
    /// <param name="matchedStubs">A list of matched stubs.</param>
    /// <returns>The stub that should be used to generate the response.</returns>
    StubModel DetermineFinalStub(IEnumerable<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs);
}
