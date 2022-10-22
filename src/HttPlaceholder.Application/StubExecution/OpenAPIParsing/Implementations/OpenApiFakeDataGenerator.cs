using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bogus;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

internal class OpenApiFakeDataGenerator : IOpenApiFakeDataGenerator, ISingletonService
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
    public string GetJsonExample(OpenApiMediaType mediaType)
    {
        var example = mediaType?.Example ??
                      mediaType?.Schema?.Example ?? mediaType?.Examples?.Values.FirstOrDefault()?.Value;
        if (example == null)
        {
            return null;
        }

        var result = ExtractJsonFromOpenApiAny(example);

        // Sometimes, a JSON example is formatted as string. We recognize that and return it as a valid JSON object.
        return example is OpenApiString ? JToken.Parse(result).ToString() : result;
    }

    /// <inheritdoc />
    public object GetExampleForHeader(OpenApiHeader header)
    {
        var example = header?.Example ?? header?.Schema?.Example ?? header?.Examples?.Values.FirstOrDefault()?.Value;
        return example == null ? null : ExtractExampleFromOpenApiAny(example);
    }

    public object GetExampleForParameter(OpenApiParameter parameter)
    {
        var example = parameter?.Example ??
                      parameter?.Schema?.Example ?? parameter?.Examples?.Values.FirstOrDefault()?.Value;
        return example == null ? null : ExtractExampleFromOpenApiAny(example);
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
        if (!schema.Properties.Any() && schema.OneOf.Any())
        {
            var oneOf = schema.OneOf.First();
            type = oneOf.Type;
            format = oneOf.Format;
            actualSchema = oneOf;
        }

        if (!schema.Properties.Any() && schema.AllOf.Any())
        {
            var allOf = schema.AllOf.First();
            type = allOf.Type;
            format = allOf.Format;
            actualSchema = allOf;
        }

        if (string.IsNullOrWhiteSpace(type) && schema.Properties.Any())
        {
            // Sort of ugly hack to accomodate to broken OpenAPI definitions.
            type = "object";
        }

        return type switch
        {
            "boolean" => _faker.Random.Bool(),
            "string" => GenerateFakeString(format, actualSchema.Enum),
            "integer" => GenerateRandomInteger(format),
            "number" => GenerateRandomNumber(format),
            "object" => GetRandomObject(actualSchema),
            "array" => GetRandomArray(actualSchema),
            "null" => null,
            _ => string.Empty
        };
    }

    private static object[] GetRandomArray(OpenApiSchema schema)
    {
        var result = new List<object>();
        var noOfItems = _faker.Random.Int(1, 3);
        for (var i = 0; i < noOfItems; i++)
        {
            result.Add(GetRandomValue(schema.Items));
        }

        return result.ToArray();
    }

    private static IDictionary<string, object> GetRandomObject(OpenApiSchema schema) =>
        schema.Properties.ToDictionary(property => property.Key, property => GetRandomValue(property.Value));

    private static string GenerateFakeString(string format, IList<IOpenApiAny> enumValues)
    {
        if (enumValues?.Any() == true)
        {
            // Pick a random value from the enum list.
            var enumValue = _faker.PickRandom(enumValues);
            return (enumValue as OpenApiString)?.Value;
        }

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

    private static string ExtractJsonFromOpenApiAny(IOpenApiAny example)
    {
        using var stringWriter = new StringWriter();
        example.Write(
            new OpenApiJsonWriter(stringWriter,
                new OpenApiWriterSettings {InlineExternalReferences = true, InlineLocalReferences = true}),
            OpenApiSpecVersion.OpenApi3_0);
        var result = stringWriter.ToString();
        return result;
    }

    internal static object ExtractExampleFromOpenApiAny(IOpenApiAny example)
    {
        switch (example.AnyType)
        {
            case AnyType.Primitive:
                var primitiveType = example as IOpenApiPrimitive;
                return ExtractOpenApiPrimitiveValue(example, primitiveType?.PrimitiveType);
            case AnyType.Array:
            case AnyType.Object:
            case AnyType.Null:
            default:
                // Generation of other types is not supported in this method.
                return null;
        }
    }

    internal static object ExtractOpenApiPrimitiveValue(IOpenApiAny example, PrimitiveType? type) =>
        type switch
        {
            PrimitiveType.Binary => ((OpenApiBinary)example).Value,
            PrimitiveType.Byte => ((OpenApiByte)example).Value,
            PrimitiveType.Boolean => ((OpenApiBoolean)example).Value,
            PrimitiveType.Integer => ((OpenApiInteger)example).Value,
            PrimitiveType.Long => ((OpenApiLong)example).Value,
            PrimitiveType.Float => ((OpenApiFloat)example).Value,
            PrimitiveType.Double => ((OpenApiDouble)example).Value,
            PrimitiveType.String => ((OpenApiString)example).Value,
            PrimitiveType.Date => ((OpenApiDate)example).Value,
            PrimitiveType.DateTime => ((OpenApiDateTime)example).Value,
            PrimitiveType.Password => ((OpenApiPassword)example).Value,
            _ => null
        };
}
