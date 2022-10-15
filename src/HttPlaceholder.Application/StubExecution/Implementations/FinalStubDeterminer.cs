using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class FinalStubDeterminer : IFinalStubDeterminer, ISingletonService
{
    /// <inheritdoc/>
    public StubModel DetermineFinalStub(IEnumerable<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs)
    {
        StubModel finalStub;
        var matchedStubsArray = matchedStubs as (StubModel, IEnumerable<ConditionCheckResultModel>)[] ?? matchedStubs.ToArray();
        var highestPriority = matchedStubsArray.Max(s => s.Item1.Priority);
        if (matchedStubsArray.Count(s => s.Item1.Priority == highestPriority) > 1)
        {
            // If there are multiple stubs found that have the highest priority, we want to select the stub with the most executed conditions,
            // because this is always the most specific one.
            finalStub = matchedStubsArray
                .OrderByDescending(s => s.Item2.Count(r => r.ConditionValidation == ConditionValidationType.Valid))
                .Where(s => s.Item1.Priority == highestPriority)
                .Select(s => s.Item1)
                .First();
        }
        else
        {
            // Make sure the stub with the highest priority gets selected.
            finalStub = matchedStubsArray
                .Select(s => s.Item1)
                .OrderByDescending(s => s.Priority)
                .First();
        }

        return finalStub;
    }
}
