using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a given posted form value in the response.
/// </summary>
internal class FormPostResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public FormPostResponseVariableParsingHandler(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public string Name => "form_post";

    /// <inheritdoc />
    public string FullName => "Form post";

    /// <inheritdoc />
    public string Example => "((form_post:form_key))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub)
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
            formValues = Array.Empty<(string, StringValues)>();
        }

        // TODO there can be multiple form values, so this should be fixed in the future.
        var formDict = formValues.ToDictionary(f => f.Item1, f => f.Item2.First());

        foreach (var match in enumerable)
        {
            var formValueName = match.Groups[2].Value;
            formDict.TryGetValue(formValueName, out var replaceValue);

            input = input.Replace(match.Value, replaceValue);
        }

        return input;
    }
}
