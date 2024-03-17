using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Export.Implementations;

internal class RequestToCurlCommandService : IRequestToCurlCommandService, ISingletonService
{
    private static readonly string[] _methodsToSkip = ["GET", "POST"];
    private static readonly string[] _headersToSkip = ["Content-Length"];

    public string Convert(RequestResultModel request)
    {
        var result = new List<string> { "curl" };
        var reqParams = request.RequestParameters;
        result.AddRange(AddMethod(reqParams));
        result.AddRange(AddUrl(reqParams));
        result.AddRange(AddHeaders(reqParams));
        result.AddRange(AddRequestBody(reqParams));
        return string.Join(' ', result);
    }

    private static IEnumerable<string> AddMethod(RequestParametersModel reqParams) =>
        _methodsToSkip.Any(m => m.Equals(reqParams.Method, StringComparison.OrdinalIgnoreCase))
            ? Array.Empty<string>()
            : ["-X", reqParams.Method.ToUpper()];

    private static IEnumerable<string> AddUrl(RequestParametersModel reqParams) => new[] { $"'{reqParams.Url}'" };

    private static IEnumerable<string> AddHeaders(RequestParametersModel reqParams) =>
        reqParams.Headers == null || !reqParams.Headers.Any()
            ? Array.Empty<string>()
            : reqParams.Headers
                .Where(h => !_headersToSkip.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .Select(h => $"-H '{h.Key}: {h.Value}'");

    private static IEnumerable<string> AddRequestBody(RequestParametersModel reqParams) =>
        reqParams?.BinaryBody == null || reqParams?.BinaryBody?.Any() == false
            ? Array.Empty<string>()
            : ["-d", $"'{Encoding.UTF8.GetString(reqParams.BinaryBody)}'"];
}
