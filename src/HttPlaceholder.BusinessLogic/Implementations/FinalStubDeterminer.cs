using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.BusinessLogic.Implementations
{
    internal class FinalStubDeterminer : IFinalStubDeterminer
    {
        public StubModel DetermineFinalStub(IList<(StubModel, IEnumerable<ConditionCheckResultModel>)> matchedStubs)
        {
            StubModel finalStub;
            int highestPriority = matchedStubs.Max(s => s.Item1.Priority);
            if (matchedStubs.Count(s => s.Item1.Priority == highestPriority) > 1)
            {
                // If there are multiple stubs found that have the highest priority, we want to select the stub with the most executed conditions,
                // because this is always the most specific one.
                finalStub = matchedStubs
                    .OrderByDescending(s => s.Item2.Count(r => r.ConditionValidation == ConditionValidationType.Valid))
                    .Where(s => s.Item1.Priority == highestPriority)
                    .Select(s => s.Item1)
                    .First();
            }
            else
            {
                // Make sure the stub with the highest priority gets selected.
                finalStub = matchedStubs
                   .Select(s => s.Item1)
                   .OrderByDescending(s => s.Priority)
                   .First();
            }

            return finalStub;
        }
    }
}
