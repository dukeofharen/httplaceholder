using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    public class StubRequestExecutor : IStubRequestExecutor
    {
        private readonly IEnumerable<IConditionChecker> _conditionCheckers;
        private readonly IFinalStubDeterminer _finalStubDeterminer;
        private readonly ILogger<StubRequestExecutor> _logger;
        private readonly IRequestLoggerFactory _requestLoggerFactory;
        private readonly IStubContext _stubContainer;
        private readonly IStubResponseGenerator _stubResponseGenerator;
        private readonly IScenarioService _scenarioService;

        public StubRequestExecutor(
            IEnumerable<IConditionChecker> conditionCheckers,
            IFinalStubDeterminer finalStubDeterminer,
            ILogger<StubRequestExecutor> logger,
            IRequestLoggerFactory requestLoggerFactory,
            IStubContext stubContainer,
            IStubResponseGenerator stubResponseGenerator,
            IScenarioService scenarioService)
        {
            _conditionCheckers = conditionCheckers;
            _finalStubDeterminer = finalStubDeterminer;
            _logger = logger;
            _requestLoggerFactory = requestLoggerFactory;
            _stubContainer = stubContainer;
            _stubResponseGenerator = stubResponseGenerator;
            _scenarioService = scenarioService;
        }

        public async Task<ResponseModel> ExecuteRequestAsync()
        {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            var foundStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
            var stubs = (await _stubContainer.GetStubsAsync()).Where(s => s.Stub.Enabled).ToArray();
            var orderedConditionCheckers = _conditionCheckers.OrderByDescending(c => c.Priority).ToArray();
            foreach (var fullStub in stubs)
            {
                var stub = fullStub.Stub;
                try
                {
                    var validationResults = new List<ConditionCheckResultModel>();
                    foreach (var checker in orderedConditionCheckers)
                    {
                        var validationResult = checker.Validate(stub.Id, stub.Conditions);
                        validationResult.CheckerName = checker.GetType().Name;
                        validationResults.Add(validationResult);
                        if (validationResult.ConditionValidation == ConditionValidationType.Invalid)
                        {
                            // If any condition is invalid, skip the rest.
                            break;
                        }
                    }

                    var passed = (validationResults.All(r =>
                                      r.ConditionValidation != ConditionValidationType.Invalid) &&
                                  validationResults.Any(r =>
                                      r.ConditionValidation != ConditionValidationType.NotExecuted &&
                                      r.ConditionValidation != ConditionValidationType.NotSet)) ||
                                 validationResults.All(
                                     r => r.ConditionValidation == ConditionValidationType.NotExecuted);
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
            _scenarioService.IncreaseHitCount(finalStub.Scenario);
            requestLogger.SetExecutingStubId(finalStub.Id);
            var response = await _stubResponseGenerator.GenerateResponseAsync(finalStub);
            return response;
        }
    }
}
