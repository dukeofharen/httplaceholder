using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert a given posted form value in the response.
/// </summary>
internal class FormPostResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public FormPostResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) :
        base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "form_post";

    /// <inheritdoc />
    public override string FullName => "Form post";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}:form_key))"};

    /// <inheritdoc />
    protected override string InsertVariables(string input, Match[] matches, StubModel stub)
    {
        var formValues = _httpContextService.GetFormValues();
        // TODO there can be multiple form values, so this should be fixed in the future.
        var formDict = formValues.ToDictionary(f => f.Item1, f => f.Item2.First());
        return matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input, (current, match) => InsertFormValue(current, match, formDict));
    }

    private static string InsertFormValue(string current, Match match, IDictionary<string, string> formDict)
    {
        var formValueName = match.Groups[2].Value;
        if (!formDict.TryGetValue(formValueName, out var replaceValue))
        {
            replaceValue = string.Empty;
        }

        return current.Replace(match.Value, replaceValue);
    }
}
