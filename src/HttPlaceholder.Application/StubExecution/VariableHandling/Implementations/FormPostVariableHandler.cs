using Ducode.Essentials.Mvc.Interfaces;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
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
            if (matches.Any())
            {
                ValueTuple<string, StringValues>[] formValues;
                try
                {
                    // We don't care about any exceptions here.
                    formValues = _httpContextService.GetFormValues();
                }
                catch
                {
                    formValues = new ValueTuple<string, StringValues>[0];
                }

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
            }

            return input;
        }
    }
}
