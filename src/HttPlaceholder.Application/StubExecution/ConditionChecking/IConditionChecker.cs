using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking
{
    public interface IConditionChecker
    {
        ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions);
    }
}
