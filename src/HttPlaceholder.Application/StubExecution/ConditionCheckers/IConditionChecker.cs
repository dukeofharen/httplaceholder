﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Describes a condition checker that checks whether the current request matches any of the conditions in a stub.
/// </summary>
public interface IConditionChecker
{
    /// <summary>
    ///     The priority of the given condition checker. The higher the number, the earlier this condition checker will be
    ///     executed.
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     Validates the current request based on a given <see cref="StubConditionsModel" />.
    /// </summary>
    /// <param name="stub">The <see cref="StubModel" /> for which the conditions should be checked.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ConditionCheckResultModel" /> containing the stub execution results.</returns>
    Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken);
}
