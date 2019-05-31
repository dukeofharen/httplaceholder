using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
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

        public StubRequestExecutor(
            IEnumerable<IConditionChecker> conditionCheckers,
            IFinalStubDeterminer finalStubDeterminer,
           ILogger<StubRequestExecutor> logger,
           IRequestLoggerFactory requestLoggerFactory,
           IStubContext stubContainer,
           IStubResponseGenerator stubResponseGenerator)
        {
            _conditionCheckers = conditionCheckers;
            _finalStubDeterminer = finalStubDeterminer;
            _logger = logger;
            _requestLoggerFactory = requestLoggerFactory;
            _stubContainer = stubContainer;
            _stubResponseGenerator = stubResponseGenerator;
        }

        public async Task<ResponseModel> ExecuteRequestAsync()
        {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();

            var foundStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
            var stubs = await _stubContainer.GetStubsAsync();

            foreach (var fullStub in stubs)
            {
                var stub = fullStub.Stub;
                try
                {
                    var passed = false;
                    var validationResults = new List<ConditionCheckResultModel>();
                    var negativeValidationResults = new List<ConditionCheckResultModel>();
                    foreach (var checker in _conditionCheckers)
                    {
                        var result = CheckConditions(stub.Id, checker, stub.Conditions, false);
                        validationResults.Add(result);
                        if (result.ConditionValidation == ConditionValidationType.Invalid)
                        {
                            // If any condition is invalid, skip the rest.
                            break;
                        }

                        // Then check the "negative" conditions. These conditions are the "not" scenarios.
                        result = CheckConditions(stub.Id, checker, stub.NegativeConditions, true);
                        negativeValidationResults.Add(result);
                        if (result.ConditionValidation == ConditionValidationType.Invalid)
                        {
                            // If any condition is invalid, skip the rest.
                            break;
                        }
                    }

                    var allValidationResults = validationResults.Concat(negativeValidationResults);
                    if (allValidationResults.All(r => r.ConditionValidation != ConditionValidationType.Invalid) && validationResults.Any(r => r.ConditionValidation != ConditionValidationType.NotExecuted && r.ConditionValidation != ConditionValidationType.NotSet) ||
                        allValidationResults.All(r => r.ConditionValidation == ConditionValidationType.NotExecuted))
                    {
                        passed = true;
                        foundStubs.Add((stub, allValidationResults));
                    }

                    requestLogger.SetStubExecutionResult(stub.Id, passed, validationResults, negativeValidationResults);
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Exception thrown while executing checks for stub '{stub.Id}': {e}");
                }
            }

            if (!foundStubs.Any())
            {
                // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
                throw new RequestValidationException($"The '{nameof(foundStubs)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
            }

            var finalStub = _finalStubDeterminer.DetermineFinalStub(foundStubs);
            requestLogger.SetExecutingStubId(finalStub.Id);
            var response = await _stubResponseGenerator.GenerateResponseAsync(finalStub);
            return response;
        }

        private ConditionCheckResultModel CheckConditions(string stubId, IConditionChecker checker, StubConditionsModel conditions, bool negative)
        {
            var validationResult = checker.Validate(stubId, conditions);
            validationResult.CheckerName = checker.GetType().Name;
            var conditionValidation = validationResult.ConditionValidation;
            if (negative && conditionValidation != ConditionValidationType.NotExecuted)
            {
                validationResult.ConditionValidation = conditionValidation == ConditionValidationType.Invalid ? ConditionValidationType.Valid : ConditionValidationType.Invalid;
            }

            return validationResult;
        }
    }
}
