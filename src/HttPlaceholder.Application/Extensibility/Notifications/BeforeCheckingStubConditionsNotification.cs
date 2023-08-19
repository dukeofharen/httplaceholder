using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Extensibility.Notifications;

/// <summary>
///     A notification that is sent right before the stub request is checked against the configured stubs.
/// </summary>
public class BeforeCheckingStubConditionsNotification : INotification
{
    /// <summary>
    ///     Gets or sets the configured stubs.
    /// </summary>
    public IEnumerable<FullStubModel> Stubs { get; set; }

    /// <summary>
    ///     Gets or sets the configured condition checkers.
    /// </summary>
    public IEnumerable<IConditionChecker> ConditionCheckers { get; set; }

    /// <summary>
    ///     Gets or sets the response that should be returned.
    ///     Leave this null to continue stub execution, set this if you want to return the response directly without continuing
    ///     stub execution.
    /// </summary>
    public ResponseModel Response { get; set; }
}
