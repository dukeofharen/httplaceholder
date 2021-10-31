﻿using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class ScenarioMaxHitCounterConditionChecker : IConditionChecker
    {
        private readonly IScenarioService _scenarioService;

        public ScenarioMaxHitCounterConditionChecker(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var maxHits = stub.Conditions?.Scenario?.MaxHits;
            if (maxHits == null)
            {
                return result;
            }

            var scenario = stub.Scenario;
            var actualHitCount = _scenarioService.GetHitCount(scenario);
            if (actualHitCount == null)
            {
                result.Log = "No hit count could be found.";
                result.ConditionValidation = ConditionValidationType.Invalid;
            }
            else if (actualHitCount >= maxHits)
            {
                result.Log =
                    $"Scenario '{scenario}' should have less than '{maxHits}' hits, but '{actualHitCount}' hits were counted.";
                result.ConditionValidation = ConditionValidationType.Invalid;
            }
            else if (actualHitCount < maxHits)
            {
                result.ConditionValidation = ConditionValidationType.Valid;
            }

            return result;
        }

        public int Priority => 8;
    }
}
