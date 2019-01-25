using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ducode.Essentials.Mvc.Interfaces;

namespace HttPlaceholder.BusinessLogic.Implementations.VariableHandlers
{
    public class FormPostVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;

        public FormPostVariableHandler(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public string Name => "form_post";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var formValues = _httpContextService.GetFormValues();

            // TODO there can be multiple form values, so this should be fixed in the future.
            var formDict = formValues.ToDictionary(f => f.Item1, f => f.Item2.First());

            foreach (var match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    string formValueName = match.Groups[2].Value;
                    string replaceValue = string.Empty;
                    formDict.TryGetValue(formValueName, out replaceValue);

                    input = input.Replace(match.Value, replaceValue);
                }
            }

            return input;
        }
    }
}
