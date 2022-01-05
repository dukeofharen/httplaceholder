using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bogus;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

/// <inheritdoc />
internal class OpenApiFakeDataGenerator : IOpenApiFakeDataGenerator
{
    private static readonly Faker _faker = new();

    /// <inheritdoc />
    public string GetRandomStringValue(OpenApiSchema schema) => GetRandomValue(schema)?.ToString();

    /// <inheritdoc />
    public string GetRandomJsonStringValue(OpenApiSchema schema)
    {
        var value = GetRandomValue(schema);
        return value == null ? null : JsonConvert.SerializeObject(value);
    }

    /// <inheritdoc />
    public string GetResponseJsonExample(OpenApiMediaType mediaType)
    {
        if (
            mediaType.Examples?.Any() != true ||
            mediaType.Examples.Values.First().Value == null)
        {
            return null;
        }

        var example = mediaType.Examples.Values.First().Value;
        using var stringWriter = new StringWriter();
        example.Write(
            new OpenApiJsonWriter(stringWriter,
                new OpenApiWriterSettings {ReferenceInline = ReferenceInlineSetting.InlineAllReferences}),
            OpenApiSpecVersion.OpenApi3_0);
        var result = stringWriter.ToString();

        // Sometimes, a JSON example is formatted as string. We recognize that and return it as a valid JSON object.
        return example is OpenApiString ? JToken.Parse(result).ToString() : result;
    }

    internal static object GetRandomValue(OpenApiSchema schema)
    {
        if (schema == null)
        {
            return null;
        }

        var type = schema.Type;
        var format = schema.Format;
        var actualSchema = schema;
        if (string.IsNullOrWhiteSpace(type) && !schema.Properties.Any() && schema.OneOf.Any())
        {
            // In some cases, the type is null and the properties are not set, but there is a value in OneOf.
            // So use this value instead.
            var oneOf = schema.OneOf.First();
            type = oneOf.Type;
            format = oneOf.Format;
            actualSchema = oneOf;
        }

        return type switch
        {
            "boolean" => _faker.Random.Bool(),
            "string" => GenerateFakeString(format),
            "integer" => GenerateRandomInteger(format),
            "number" => GenerateRandomNumber(format),
            "object" => GetRandomJsonObject(actualSchema),
            "array" => GetRandomJsonArray(actualSchema),
            "null" => null,
            _ => string.Empty
        };
    }

    private static object[] GetRandomJsonArray(OpenApiSchema schema)
    {
        var result = new List<object>();
        var noOfItems = _faker.Random.Int(1, 3);
        for (var i = 0; i < noOfItems; i++)
        {
            result.Add(GetRandomValue(schema.Items));
        }

        return result.ToArray();
    }

    private static IDictionary<string, object> GetRandomJsonObject(OpenApiSchema schema) =>
        schema.Properties.ToDictionary(property => property.Key, property => GetRandomValue(property.Value));

    private static string GenerateFakeString(string format)
    {
        var date = _faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now);
        switch (format)
        {
            case "byte":
            case "binary":
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(_faker.Random.Words(3)));
            case "date":
                return date.ToString("yyyy-MM-dd");
            case "date-time":
                return date.ToString("yyyy-MM-ddTHH:mm:ssK");
            default:
                return _faker.Random.Word();
        }
    }

    private static object GenerateRandomNumber(string format) =>
        format switch
        {
            "float" => _faker.Random.Float(),
            _ => _faker.Random.Double()
        };

    private static object GenerateRandomInteger(string format) =>
        format switch
        {
            "int64" => _faker.Random.Long(0, 10000),
            _ => _faker.Random.Int(0, 10000)
        };
}
