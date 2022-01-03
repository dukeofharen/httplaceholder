using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

/// <inheritdoc />
internal class OpenApiFakeDataGenerator : IOpenApiFakeDataGenerator
{
    private static readonly Random _random = new();

    /// <inheritdoc />
    public string GetRandomStringValue(OpenApiSchema schema) => GetRandomValue(schema)?.ToString();

    /// <inheritdoc />
    public string GetRandomJsonStringValue(OpenApiSchema schema) => JsonConvert.SerializeObject(GetRandomValue(schema));

    /// <inheritdoc />
    public object GetRandomValue(OpenApiSchema schema)
    {
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

    /// <inheritdoc />
    public object[] GetRandomJsonArray(OpenApiSchema schema)
    {
        var result = new List<object>();
        var noOfItems = _random.Next(1, 3);
        for (var i = 0; i < noOfItems; i++)
        {
            result.Add(GetRandomValue(schema.Items));
        }

        return result.ToArray();
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetRandomJsonObject(OpenApiSchema schema) =>
        schema.Properties.ToDictionary(property => property.Key, property => GetRandomValue(property.Value));
}
