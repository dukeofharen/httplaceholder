using System;
using System.Linq;
using System.Web;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class FormValueConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public FormValueConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var formConditions = stub.Conditions?.Form?.ToArray() ?? new StubFormModel[0];
            if (!formConditions.Any())
            {
                return result;
            }

            try
            {
                var form = _httpContextService.GetFormValues();
                var validConditions = 0;
                foreach (var condition in formConditions)
                {
                    var (formKey, formValues) = form.FirstOrDefault(f => f.Item1 == condition.Key);
                    if (formKey == null)
                    {
                        result.ConditionValidation = ConditionValidationType.Invalid;
                        result.Log = $"No form value with key '{condition.Key}' found.";
                        break;
                    }

                    validConditions += formValues
                        .Count(value =>
                            StringHelper.IsRegexMatchOrSubstring(HttpUtility.UrlDecode(value), condition.Value));
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the form condition is passed and the stub ID is passed to the result.
                if (validConditions == formConditions.Length)
                {
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.Log =
                        $"Number of configured form conditions: '{formConditions.Length}'; number of passed form conditions: '{validConditions}'";
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }
            catch (InvalidOperationException ex)
            {
                result.Log = ex.Message;
                result.ConditionValidation = ConditionValidationType.Invalid;
            }

            return result;
        }

        public int Priority => 8;
    }
}
