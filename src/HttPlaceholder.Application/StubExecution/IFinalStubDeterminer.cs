using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

public interface IFinalStubDeterminer
{
    StubModel DetermineFinalStub(IEnumerable<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs);
}