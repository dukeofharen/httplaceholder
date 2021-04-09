using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking
{
    /// <summary>
    /// Describes a condition checker that checks whether the current request matches any of the conditions in a stub.
    /// </summary>
    public interface IConditionChecker
    {
        /// <summary>
        /// Validates the current request based on a given <see cref="StubConditionsModel"/>.
        /// </summary>
        /// <param name="stubId">The stub ID of the stub to be checked.</param>
        /// <param name="conditions">The <see cref="StubConditionsModel"/> conditions of the stub to be checked.</param>
        /// <returns>A <see cref="ConditionCheckResultModel"/> containing the stub execution results.</returns>
        ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions);

        /// <summary>
        /// The priority of the given condition checker. The higher the number, the earlier this condition checker will be executed.
        /// </summary>
        int Priority { get; }
    }
}
