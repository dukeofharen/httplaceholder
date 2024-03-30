using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert a given posted form value in the response.
/// </summary>
internal class FormPostResponseVariableParsingHandler(IHttpContextService httpContextService)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "form_post";

    /// <inheritdoc />
    public override string FullName => "Form post";

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}:form_key))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.FormPost;

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var formValues = await httpContextService.GetFormValuesAsync(cancellationToken);
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
