using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.Extensions.Primitives;

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
            var enumerable = matches as Match[] ?? matches.ToArray();
            if (!enumerable.Any())
            {
                return input;
            }

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

            foreach (var match in enumerable)
            {
                if (match.Groups.Count != 3)
                {
                    continue;
                }

                var formValueName = match.Groups[2].Value;
                formDict.TryGetValue(formValueName, out var replaceValue);

                input = input.Replace(match.Value, replaceValue);
            }

            return input;
        }
    }
}
