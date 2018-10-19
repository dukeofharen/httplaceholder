using System.Collections.Generic;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
    public interface IFinalStubDeterminer
    {
        StubModel DetermineFinalStub(IList<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs);
    }
}
