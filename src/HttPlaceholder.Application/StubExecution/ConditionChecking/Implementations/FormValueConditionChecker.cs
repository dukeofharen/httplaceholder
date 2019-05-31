using System.Linq;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class FormValueConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public FormValueConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var formConditions = conditions?.Form?.ToArray();
            if (formConditions != null)
            {
                var form = _httpContextService.GetFormValues();
                int validConditions = 0;
                foreach (var condition in formConditions)
                {
                    var formValue = form.FirstOrDefault(f => f.Item1 == condition.Key);
                    if (formValue.Item1 == null)
                    {
                        result.ConditionValidation = ConditionValidationType.Invalid;
                        result.Log = $"No form value with key '{condition.Key}' found.";
                        break;
                    }

                    foreach (string value in formValue.Item2)
                    {
                        if (StringHelper.IsRegexMatchOrSubstring(value, condition.Value))
                        {
                            validConditions++;
                        }
                    }
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the form condition is passed and the stub ID is passed to the result.
                if (validConditions == formConditions.Length)
                {
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.Log = $"Number of configured form conditions: '{formConditions.Length}'; number of passed form conditions: '{validConditions}'";
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }
    }
}
