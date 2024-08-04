using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Extensibility.Notifications;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubRequestExecutor(
    IEnumerable<IConditionChecker> conditionCheckers,
    IFinalStubDeterminer finalStubDeterminer,
    ILogger<StubRequestExecutor> logger,
    IRequestLoggerFactory requestLoggerFactory,
    IStubContext stubContext,
    IStubResponseGenerator stubResponseGenerator,
    IMediator mediator)
    : IStubRequestExecutor, ISingletonService
{
    /// <inheritdoc />
    public async Task<ResponseModel> ExecuteRequestAsync(CancellationToken cancellationToken)
    {
        var requestLogger = requestLoggerFactory.GetRequestLogger();
        var foundStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
        var stubs = (await stubContext.GetStubsAsync(cancellationToken)).Where(s => s.Stub.Enabled).ToArray();
        var orderedConditionCheckers = conditionCheckers.OrderByDescending(c => c.Priority).ToArray();

        var beforeCheckingNotification = new BeforeCheckingStubConditionsNotification
        {
            ConditionCheckers = orderedConditionCheckers, Stubs = stubs
        };
        await mediator.Publish(beforeCheckingNotification, cancellationToken);
        if (beforeCheckingNotification.Response != null)
        {
            return beforeCheckingNotification.Response;
        }

        foreach (var fullStub in stubs)
        {
            var stub = fullStub.Stub;
            try
            {
                var validationResults = new List<ConditionCheckResultModel>();
                foreach (var checker in orderedConditionCheckers)
                {
                    var validationResult = await checker.ValidateAsync(stub, cancellationToken);
                    validationResult.SetCheckerName(checker.GetType().Name);
                    validationResults.Add(validationResult);
                    if (validationResult.ConditionValidation == ConditionValidationType.Invalid)
                    {
                        // If any condition is invalid, skip the rest.
                        break;
                    }
                }

                var passed = RequestPassed(validationResults);
                if (passed)
                {
                    foundStubs.Add((stub, validationResults));
                }

                requestLogger.SetStubExecutionResult(stub.Id, passed, validationResults);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Exception thrown while executing checks for stub '{StubId}'.", stub.Id);
            }
        }

        if (foundStubs.Count == 0)
        {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new RequestValidationException(string.Format(StubResources.StubValidationError, nameof(foundStubs)));
        }

        var finalStub = finalStubDeterminer.DetermineFinalStub(foundStubs);
        await stubContext.IncreaseHitCountAsync(finalStub.Scenario, cancellationToken);
        requestLogger.SetExecutingStubId(finalStub.Id);
        var response = await stubResponseGenerator.GenerateResponseAsync(finalStub, cancellationToken);
        await mediator.Publish(new BeforeStubResponseReturnedNotification { Response = response }, cancellationToken);
        return response;
    }

    private static bool RequestPassed(IReadOnlyCollection<ConditionCheckResultModel> validationResults) =>
        // Check whether the validation results do not contain "Invalid" and any other results besides "NotExecuted" and "NotSet" (so "Valid")
        // OR if all the validation results are "NotExecuted".
        (validationResults.All(r =>
             r.ConditionValidation != ConditionValidationType.Invalid) &&
         validationResults.Any(r =>
             r.ConditionValidation != ConditionValidationType.NotExecuted &&
             r.ConditionValidation != ConditionValidationType.NotSet)) ||
        validationResults.All(
            r => r.ConditionValidation == ConditionValidationType.NotExecuted);
}
