using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

internal partial class OpenApiDataFiller(IOpenApiFakeDataGenerator openApiFakeDataGenerator)
    : IOpenApiDataFiller, ISingletonService
{
    [GeneratedRegex("^[1-5]{1}[0-9]{2}$", RegexOptions.Compiled)]
    private static partial Regex StatusCodeRegex();

    private static readonly Regex _statusCodeRegex = StatusCodeRegex();

    /// <inheritdoc />
    public string BuildServerUrl(OpenApiServer server)
    {
        if (server.Variables?.Any() == false)
        {
            return server.Url;
        }

        return server.Variables
            .Aggregate(
                server.Url,
                (current, variable) => current.Replace($"{{{variable.Key}}}", variable.Value.Default));
    }

    /// <inheritdoc />
    public int BuildHttpStatusCode(string responseKey) =>
        _statusCodeRegex.IsMatch(responseKey) ? int.Parse(responseKey) : 0;

    /// <inheritdoc />
    public string BuildResponseBody(OpenApiResponse response)
    {
        if (response.Content == null || !response.Content.Any())
        {
            return null;
        }

        var content = response.Content
            .FirstOrDefault(c => string.Equals(c.Key, MimeTypes.JsonMime, StringComparison.OrdinalIgnoreCase));

        // If the content has any examples assigned to it, create a response based on that instead of randomly generating a response.
        var example = openApiFakeDataGenerator.GetJsonExample(content.Value);
        if (!string.IsNullOrWhiteSpace(example))
        {
            return example;
        }

        return string.IsNullOrWhiteSpace(content.Key)
            ? null
            : openApiFakeDataGenerator.GetRandomJsonStringValue(content.Value.Schema);
    }

    /// <inheritdoc />
    public IDictionary<string, string> BuildResponseHeaders(OpenApiResponse response)
    {
        var result = response.Headers?.Any() == true
            ? response.Headers.ToDictionary(h => h.Key, GetHeaderValue)
            : new Dictionary<string, string>();

        var contentType = response.Content.FirstOrDefault().Key;
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            result.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, contentType);
        }

        return result;

        string GetHeaderValue(KeyValuePair<string, OpenApiHeader> h)
        {
            var example = openApiFakeDataGenerator.GetExampleForHeader(h.Value);
            return example?.ToString() ?? openApiFakeDataGenerator.GetRandomStringValue(h.Value.Schema);
        }
    }

    /// <inheritdoc />
    public string BuildRequestBody(OpenApiOperation operation)
    {
        var requestBodyDefinitions = operation.RequestBody;
        if (requestBodyDefinitions?.Content == null || !requestBodyDefinitions.Content.Any())
        {
            return null;
        }

        var definition = requestBodyDefinitions.Content
            .FirstOrDefault(c => string.Equals(c.Key, MimeTypes.JsonMime, StringComparison.OrdinalIgnoreCase));

        // If the content has any examples assigned to it, create a response based on that instead of randomly generating a response.
        var example = openApiFakeDataGenerator.GetJsonExample(definition.Value);
        if (!string.IsNullOrWhiteSpace(example))
        {
            return example;
        }

        return string.IsNullOrWhiteSpace(definition.Key)
            ? null
            : openApiFakeDataGenerator.GetRandomJsonStringValue(definition.Value.Schema);
    }

    /// <inheritdoc />
    public string BuildRelativeRequestPath(OpenApiOperation operation, string basePath)
    {
        var relativePath = basePath;
        var parameters = operation.Parameters;

        // Parse path parameters
        var pathParams = parameters
            .Where(p => p.In == ParameterLocation.Path)
            .ToArray();
        if (pathParams.Length != 0)
        {
            relativePath = pathParams
                .Aggregate(
                    relativePath,
                    (current, parameter) => current.Replace($"{{{parameter.Name}}}", GetUrlValue(parameter)));
        }

        // Parse query parameters.
        var queryParams = parameters
            .Where(p => p.In == ParameterLocation.Query)
            .ToArray();
        if (queryParams.Length == 0)
        {
            return relativePath;
        }

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in queryParams)
        {
            queryString.Add(param.Name, GetUrlValue(param));
        }

        return $"{relativePath}?{queryString}";

        string GetUrlValue(OpenApiParameter parameter)
        {
            var example = openApiFakeDataGenerator.GetExampleForParameter(parameter);
            if (example != null)
            {
                return example.ToString();
            }

            var schema = parameter.Schema;
            if (parameter.Schema.Items != null)
            {
                schema = parameter.Schema.Items;
            }

            return openApiFakeDataGenerator.GetRandomStringValue(schema);
        }
    }

    /// <inheritdoc />
    public IDictionary<string, string> BuildRequestHeaders(OpenApiOperation operation)
    {
        // Parse header parameters.
        var headerParams = operation.Parameters
            .Where(p => p.In == ParameterLocation.Header)
            .ToArray();
        var result = headerParams.Length != 0
            ? headerParams.ToDictionary(p => p.Name, p => openApiFakeDataGenerator.GetRandomStringValue(p.Schema))
            : new Dictionary<string, string>();
        if (operation.RequestBody?.Content != null && operation.RequestBody.Content.Any())
        {
            var requestBody = operation.RequestBody.Content.First();
            result.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, requestBody.Key);
        }

        return result;
    }
}
