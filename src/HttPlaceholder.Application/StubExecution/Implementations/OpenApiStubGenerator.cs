using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class OpenApiStubGenerator : IOpenApiStubGenerator
{
    private static readonly Random _random = new();
    private static readonly Regex _statusCodeRegex = new Regex("^[1-5]{1}[0-9]{2}$", RegexOptions.Compiled);

    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly IHttpResponseToStubResponseService _httpResponseToStubResponseService;
    private readonly IStubContext _stubContext;

    // TODO handle date-time, int32 and int64 handling and perhaps other types (see https://swagger.io/specification/ for format strings)
    // TODO move OpenAPI code to separate class for easier unit testing
    public OpenApiStubGenerator(
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IHttpResponseToStubResponseService httpResponseToStubResponseService,
        IStubContext stubContext)
    {
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _httpResponseToStubResponseService = httpResponseToStubResponseService;
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateOpenApiStubs(string input, bool doNotCreateStub)
    {
        // TODO set description
        // TODO catch exceptions and turn them into ValidationException.
        var openapi = new OpenApiStringReader().Read(input, out _);
        var serverUrl = openapi.Servers.Any() ? openapi.Servers.First().Url : "http://localhost";
        var stubs = new List<FullStubModel>();
        foreach (var path in openapi.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                foreach (var operationResponse in operation.Value.Responses)
                {
                    var request = new HttpRequestModel
                    {
                        Body = BuildRequestBody(operation.Value),
                        Headers = BuildRequestHeaders(operation.Value),
                        Method = operation.Key.ToString().ToUpper(),
                        Url = $"{serverUrl}{BuildRelativePath(operation.Value, path.Key)}"
                    };
                    var response = new HttpResponseModel
                    {
                        Content = BuildResponseBody(operationResponse.Value),
                        Headers = BuildResponseHeaders(operationResponse.Value),
                        StatusCode = BuildHttpStatusCode(operationResponse.Key)
                    };
                }
            }
        }

        return stubs;
    }

    private static int BuildHttpStatusCode(string responseKey) =>
        _statusCodeRegex.IsMatch(responseKey) ? int.Parse(responseKey) : 0;

    private static string BuildResponseBody(OpenApiResponse response)
    {
        if (response.Content == null || !response.Content.Any())
        {
            return null;
        }

        var content = response.Content.First();
        return content.Key != Constants.JsonMime ? null : GetRandomJsonStringValue(content.Value.Schema);
    }

    private static IDictionary<string, string> BuildResponseHeaders(OpenApiResponse response)
    {
        var result = response.Headers?.Any() == true
            ? response.Headers.ToDictionary(h => h.Key, h => GetRandomStringValue(h.Value.Schema))
            : new Dictionary<string, string>();

        var contentType = response.Content.FirstOrDefault().Key;
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            result.AddOrReplaceCaseInsensitive("content-type", contentType);
        }

        return result;
    }

    private static string BuildRequestBody(OpenApiOperation operation)
    {
        var requestBodyDefinitions = operation.RequestBody;
        if (requestBodyDefinitions?.Content == null || !requestBodyDefinitions.Content.Any())
        {
            return null;
        }

        var definition = requestBodyDefinitions.Content.First();
        return definition.Key != Constants.JsonMime ? null : GetRandomJsonStringValue(definition.Value.Schema);
    }

    private static string BuildRelativePath(OpenApiOperation operation, string basePath)
    {
        var relativePath = basePath;
        var parameters = operation.Parameters;

        // Parse path parameters
        var pathParams = parameters
            .Where(p => p.In == ParameterLocation.Path)
            .ToArray();
        if (pathParams.Any())
        {
            relativePath = pathParams
                .Aggregate(
                    relativePath,
                    (current, parameter) => current.Replace($"{{{parameter.Name}}}",
                        GetRandomStringValue(parameter.Schema)));
        }

        // Parse query parameters.
        var queryParams = parameters
            .Where(p => p.In == ParameterLocation.Query)
            .ToArray();
        if (!queryParams.Any())
        {
            return relativePath;
        }

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in queryParams)
        {
            queryString.Add(param.Name, GetRandomStringValue(param.Schema));
        }

        return $"{relativePath}?{queryString}";
    }

    private static IDictionary<string, string> BuildRequestHeaders(OpenApiOperation operation)
    {
        // Parse header parameters.
        var headerParams = operation.Parameters
            .Where(p => p.In == ParameterLocation.Header)
            .ToArray();
        var result = headerParams.Any()
            ? headerParams.ToDictionary(p => p.Name, p => GetRandomStringValue(p.Schema))
            : new Dictionary<string, string>();
        if (operation.RequestBody?.Content != null && operation.RequestBody.Content.Any())
        {
            var requestBody = operation.RequestBody.Content.First();
            result.AddOrReplaceCaseInsensitive("content-type", requestBody.Key);
        }

        return result;
    }

    private static string GetRandomStringValue(OpenApiSchema schema) => GetRandomValue(schema)?.ToString();

    private static string GetRandomJsonStringValue(OpenApiSchema schema) =>
        JsonConvert.SerializeObject(GetRandomValue(schema));

    private static object GetRandomValue(OpenApiSchema schema)
    {
        // TODO use something like Bogus for this?
        var type = schema.Type;
        var actualSchema = schema;
        if (string.IsNullOrWhiteSpace(type) && !schema.Properties.Any() && schema.OneOf.Any())
        {
            // In some cases, the type is null and the properties are not set, but there is a value in OneOf.
            // So use this value instead.
            var oneOf = schema.OneOf.First();
            type = oneOf.Type;
            actualSchema = oneOf;
        }

        return type switch
        {
            "boolean" => true,
            "string" => Guid.NewGuid().ToString(),
            "integer" => 42,
            "number" => 12.34,
            "object" => GetRandomJsonObject(actualSchema),
            "array" => GetRandomJsonArray(actualSchema),
            "null" => null,
            _ => string.Empty
        };
    }

    private static object[] GetRandomJsonArray(OpenApiSchema schema)
    {
        var result = new List<object>();
        var noOfItems = _random.Next(1, 3); // TODO use Bogus for this.
        for (var i = 0; i < noOfItems; i++)
        {
            result.Add(GetRandomValue(schema.Items));
        }

        return result.ToArray();
    }

    private static Dictionary<string, object> GetRandomJsonObject(OpenApiSchema schema) =>
        schema.Properties.ToDictionary(property => property.Key, property => GetRandomValue(property.Value));
}
