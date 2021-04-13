using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class JsonConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public JsonConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var body = _httpContextService.GetBody();
            var jsonObject = JObject.Parse(body);
            return new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.NotExecuted};
        }

        public int Priority => 1;
    }
}
