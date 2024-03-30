using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     The abstract base class for a stub request condition checker.
/// </summary>
public abstract class BaseConditionChecker : IConditionChecker
{
    /// <inheritdoc />
    public abstract int Priority { get; }

    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        try
        {
            if (!ShouldBeExecuted(stub))
            {
                return await NotExecutedAsync();
            }

            return await PerformValidationAsync(stub, cancellationToken);
        }
        catch (Exception ex)
        {
            return await InvalidAsync(ex.Message);
        }
    }

    /// <summary>
    ///     Checks whether the condition checker should be executed.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <returns>True if the checker should be executed; false otherwise.</returns>
    protected abstract bool ShouldBeExecuted(StubModel stub);

    /// <summary>
    ///     Method for validating the request. Should be inherited by the condition checker.
    /// </summary>
    /// <param name="stub">The <see cref="StubModel" /> for which the conditions should be checked.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validation result.</returns>
    protected abstract Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken);
}
