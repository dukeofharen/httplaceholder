using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;

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
            throw new NotImplementedException();
        }

        public int Priority => 1;
    }
}
