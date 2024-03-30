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
    private static readonly string[] _startParts = ["curl"];

    public string Convert(RequestResultModel request)
    {
        var reqParams = request.RequestParameters;
        return string.Join(' ', _startParts
            .Concat(AddMethod(reqParams))
            .Concat(AddUrl(reqParams))
            .Concat(AddHeaders(reqParams))
            .Concat(AddRequestBody(reqParams)));
    }

    private static string[] AddMethod(RequestParametersModel reqParams) =>
        _methodsToSkip.Any(m => m.Equals(reqParams.Method, StringComparison.OrdinalIgnoreCase))
            ? Array.Empty<string>()
            : ["-X", reqParams.Method.ToUpper()];

    private static string[] AddUrl(RequestParametersModel reqParams) => [$"'{reqParams.Url}'"];

    private static IEnumerable<string> AddHeaders(RequestParametersModel reqParams) =>
        reqParams.Headers == null || !reqParams.Headers.Any()
            ? []
            : reqParams.Headers
                .Where(h => !_headersToSkip.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .Select(h => $"-H '{h.Key}: {h.Value}'");

    private static string[] AddRequestBody(RequestParametersModel reqParams) =>
        reqParams?.BinaryBody == null || reqParams.BinaryBody.Length == 0
            ? []
            : ["-d", $"'{Encoding.UTF8.GetString(reqParams.BinaryBody)}'"];
}
