using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
/// Describes a class that is used to convert an OpenAPI YAML or JSON string to a data structure the application can work with.
/// </summary>
public interface IOpenApiParser
{
    /// <summary>
    /// Accepts an OpenAPI YAML or JSON string and parses it to a <see cref="OpenApiResult"/>.
    /// </summary>
    /// <param name="input">The YAML or JSON describing the API.</param>
    /// <returns>A <see cref="OpenApiResult"/>.</returns>
    OpenApiResult ParseOpenApiDefinition(string input);
}
