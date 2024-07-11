using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to perform a string, regex or JSONPath replace in the response.
/// </summary>
public class StringReplaceResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => -11;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var replace = stub.Response?.Replace?.ToArray();
        if (replace == null || replace.Length == 0 || response.Body == null || response.Body.Length == 0)
        {
            return IsNotExecuted(GetType().Name).AsTask();
        }

        var body = Encoding.UTF8.GetString(response.Body);
        var log = new List<string>();
        body = replace.Aggregate(body, (b, m) => PerformReplace(b, m, log));
        response.Body = Encoding.UTF8.GetBytes(body);

        return IsExecuted(GetType().Name, string.Join(Environment.NewLine, log)).AsTask();
    }

    private static string PerformReplace(string body, StubResponseReplaceModel model, IList<string> log)
    {
        if (!string.IsNullOrWhiteSpace(model.Text))
        {
            var stringComparison = !model.IgnoreCase.HasValue || model.IgnoreCase.Value
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            body = body.Replace(model.Text, model.ReplaceWith, stringComparison);
        }

        if (!string.IsNullOrWhiteSpace(model.Regex))
        {
            var regex = new Regex(model.Regex);
            var matches = regex.Matches(body);
            foreach (var match in matches)
            {
                body = body.Replace(match.ToString() ?? string.Empty, model.ReplaceWith);
            }
        }

        if (!string.IsNullOrWhiteSpace(model.JsonPath))
        {
            try
            {
                var jObject = JToken.Parse(body);
                jObject = jObject.ReplacePath(model.JsonPath, model.ReplaceWith);
                body = jObject.ToString(Formatting.None);
            }
            catch (JsonReaderException ex)
            {
                log.Add(ex.Message);
            }
        }

        return body;
    }
}
