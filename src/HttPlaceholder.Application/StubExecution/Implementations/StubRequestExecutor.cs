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

internal class StubRequestExecutor : IStubRequestExecutor, ISingletonService
{
    private readonly IEnumerable<IConditionChecker> _conditionCheckers;
    private readonly IFinalStubDeterminer _finalStubDeterminer;
    private readonly ILogger<StubRequestExecutor> _logger;
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IScenarioService _scenarioService;
    private readonly IStubContext _stubContext;
    private readonly IStubResponseGenerator _stubResponseGenerator;
    private readonly IMediator _mediator;

    public StubRequestExecutor(
        IEnumerable<IConditionChecker> conditionCheckers,
        IFinalStubDeterminer finalStubDeterminer,
        ILogger<StubRequestExecutor> logger,
        IRequestLoggerFactory requestLoggerFactory,
        IStubContext stubContext,
        IStubResponseGenerator stubResponseGenerator,
        IScenarioService scenarioService,
        IMediator mediator)
    {
        _conditionCheckers = conditionCheckers;
        _finalStubDeterminer = finalStubDeterminer;
        _logger = logger;
        _requestLoggerFactory = requestLoggerFactory;
        _stubContext = stubContext;
        _stubResponseGenerator = stubResponseGenerator;
        _scenarioService = scenarioService;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public async Task<ResponseModel> ExecuteRequestAsync(CancellationToken cancellationToken)
    {
        var requestLogger = _requestLoggerFactory.GetRequestLogger();
        var foundStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
        var stubs = (await _stubContext.GetStubsAsync(cancellationToken)).Where(s => s.Stub.Enabled).ToArray();
        var orderedConditionCheckers = _conditionCheckers.OrderByDescending(c => c.Priority).ToArray();
        foreach (var fullStub in stubs)
        {
            var stub = fullStub.Stub;
            try
            {
                var validationResults = new List<ConditionCheckResultModel>();
                foreach (var checker in orderedConditionCheckers)
                {
                    var validationResult = await checker.ValidateAsync(stub, cancellationToken);
                    validationResult.CheckerName = checker.GetType().Name;
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
                _logger.LogWarning($"Exception thrown while executing checks for stub '{stub.Id}': {e}");
            }
        }

        if (!foundStubs.Any())
        {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new RequestValidationException(
                $"The '{nameof(foundStubs)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
        }

        var finalStub = _finalStubDeterminer.DetermineFinalStub(foundStubs);
        await _scenarioService.IncreaseHitCountAsync(finalStub.Scenario, cancellationToken);
        requestLogger.SetExecutingStubId(finalStub.Id);
        var response = await _stubResponseGenerator.GenerateResponseAsync(finalStub, cancellationToken);
        await _mediator.Publish(new BeforeStubResponseReturnedNotification {Response = response}, cancellationToken);
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
