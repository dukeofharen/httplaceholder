using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
/// Describes a class that is used for generating fake data for use in OpenAPI stubs.
/// </summary>
public interface IOpenApiFakeDataGenerator
{
    /// <summary>
    /// Gets a random value and converts it to a string.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random string value.</returns>
    string GetRandomStringValue(OpenApiSchema schema);

    /// <summary>
    /// Gets a random value and formats it as a JSON string.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random string value as JSON.</returns>
    string GetRandomJsonStringValue(OpenApiSchema schema);

    /// <summary>
    /// Gets a random value.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random value.</returns>
    object GetRandomValue(OpenApiSchema schema);

    /// <summary>
    /// Gets a random array.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random array.</returns>
    object[] GetRandomJsonArray(OpenApiSchema schema);

    /// <summary>
    /// Gets a random object.
    /// </summary>
    /// <param name="schema">The OpenAPI schema.</param>
    /// <returns>The random array as dictionary.</returns>
    IDictionary<string, object> GetRandomJsonObject(OpenApiSchema schema);
}
