using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IFinalStubDeterminer
    {
        StubModel DetermineFinalStub(IList<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs);
    }
}
