using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
/// Describes a class that is used to fill the HTTP requests and responses for the stubs based on the OpenAPI schema.
/// </summary>
public interface IOpenApiDataFiller
{
    /// <summary>
    /// Parses the response key to an HTTP status code. Returns "0" if the key is no valid status code.
    /// </summary>
    /// <param name="responseKey">The OpenAPI response key.</param>
    /// <returns>Status code, or 0 if key is not a valid status code.</returns>
    int BuildHttpStatusCode(string responseKey);

    /// <summary>
    /// Builds a response body based on the OpenAPI response.
    /// Only works if the content type is "application/json"; null will be returned if this is not the case.
    /// </summary>
    /// <param name="response">The OpenAPI response definition.</param>
    /// <returns>The generated body.</returns>
    string BuildResponseBody(OpenApiResponse response);

    /// <summary>
    /// Builds the response headers based on the response definition.
    /// </summary>
    /// <param name="response">The OpenAPI response definition.</param>
    /// <returns>The generated response headers.</returns>
    IDictionary<string, string> BuildResponseHeaders(OpenApiResponse response);

    /// <summary>
    /// Builds a request body based on the OpenAPI operation.
    /// Only works if the content type is "application/json"; null will be returned if this is not the case.
    /// </summary>
    /// <param name="operation">The OpenAPI operation definition.</param>
    /// <returns>The generated body.</returns>
    string BuildRequestBody(OpenApiOperation operation);

    /// <summary>
    /// Builds a relative path based on the provided operation. The parameters in the path will be replaced and the query parameters will be appended.
    /// </summary>
    /// <param name="operation">The OpenAPI operation definition.</param>
    /// <param name="basePath">The server base URL.</param>
    /// <returns>The generated relative path.</returns>
    string BuildRelativeRequestPath(OpenApiOperation operation, string basePath);

    /// <summary>
    /// Builds the request headers based on the operation definition.
    /// </summary>
    /// <param name="operation">The OpenAPI operation definition.</param>
    /// <returns>The generated request headers.</returns>
    IDictionary<string, string> BuildRequestHeaders(OpenApiOperation operation);
}
