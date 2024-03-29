﻿using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
///     Describes a class that is used for generating fake data for use in OpenAPI stubs.
/// </summary>
public interface IOpenApiFakeDataGenerator
{
    /// <summary>
    ///     Gets a random value and converts it to a string.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random string value.</returns>
    string GetRandomStringValue(OpenApiSchema schema);

    /// <summary>
    ///     Gets a random value and formats it as a JSON string.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random string value as JSON.</returns>
    string GetRandomJsonStringValue(OpenApiSchema schema);

    /// <summary>
    ///     Used to receive an OpenAPI example and generate an example value for it for use in stubs.
    /// </summary>
    /// <param name="mediaType">The OpenAPI media type.</param>
    /// <returns>The example as JSON. null if no example was found</returns>
    string GetJsonExample(OpenApiMediaType mediaType);

    /// <summary>
    ///     Used to receive an OpenAPI header example and generate an example value for it for use in stubs.
    /// </summary>
    /// <param name="header">The header to get an example for.</param>
    /// <returns>The found example. null if no example was found.</returns>
    object GetExampleForHeader(OpenApiHeader header);

    /// <summary>
    ///     Used to receive an OpenAPI parameter example and generate an example value for it for use in stubs.
    /// </summary>
    /// <param name="parameter">The parameter to get an example for.</param>
    /// <returns>The found example. null if no example was found.</returns>
    object GetExampleForParameter(OpenApiParameter parameter);
}
