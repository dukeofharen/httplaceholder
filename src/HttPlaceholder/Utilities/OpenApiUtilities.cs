using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttPlaceholder.Attributes;
using NJsonSchema;
using NSwag;

namespace HttPlaceholder.Utilities;

/// <summary>
/// A utility class that contains several OpenAPI related utility methods.
/// </summary>
public static class OpenApiUtilities
{
    /// <summary>
    /// Handles custom logic and updates the given OpenAPI document.
    /// </summary>
    /// <param name="document">The OpenAPI document.</param>
    public static void PostProcessOpenApiDocument(OpenApiDocument document)
    {
        foreach (var type in GetCustomOpenApiClasses())
        {
            foreach (var property in type.GetProperties())
            {
                var oneOfProperty = GetOneOfAttribute(property);
                if (oneOfProperty == null)
                {
                    continue;
                }

                var schemaProperty = GetSchemaProperty(document, type, property);
                foreach (var oneOfType in oneOfProperty.Types)
                {
                    UpdateSchema(document, oneOfType, schemaProperty);
                }

                foreach (var oneOfType in oneOfProperty.ItemsTypes)
                {
                    UpdateSchema(document, oneOfType, schemaProperty.Item);
                }
            }
        }
    }

    private static void UpdateSchema(
        OpenApiDocument document,
        Type oneOfType,
        JsonSchema schemaToUpdate)
    {
        var oneOfTypeSchema = JsonSchema.FromType(oneOfType);
        RemoveNullTypes(oneOfTypeSchema);

        var isPrimitiveOrString = IsPrimitiveOrString(oneOfType);
        if (!isPrimitiveOrString &&
            !document.Components.Schemas.Any(s => s.Key.Equals(oneOfType.Name)))
        {
            document.Components.Schemas.Add(oneOfType.Name, oneOfTypeSchema);
        }

        schemaToUpdate.OneOf.Add(isPrimitiveOrString
            ? oneOfTypeSchema
            : new JsonSchema {Reference = oneOfTypeSchema});
    }

    private static bool IsPrimitiveOrString(Type oneOfType) => oneOfType.IsPrimitive || oneOfType == typeof(string);

    private static void RemoveNullTypes(JsonSchema oneOfTypeSchema)
    {
        foreach (var schemaProp in oneOfTypeSchema.Properties)
        {
            if (schemaProp.Value.Type.HasFlag(JsonObjectType.Null))
            {
                schemaProp.Value.Type = (JsonObjectType) (schemaProp.Value.Type - JsonObjectType.Null);
            }
        }
    }

    private static JsonSchemaProperty GetSchemaProperty(OpenApiDocument document, Type type, PropertyInfo property)
    {
        var schema = document.Components.Schemas[type.Name];
        var schemaProperty = schema.ActualProperties.Single(p =>
            p.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase)).Value;
        return schemaProperty;
    }

    private static OneOfAttribute GetOneOfAttribute(PropertyInfo property) =>
        property
            .GetCustomAttributes(typeof(OneOfAttribute), false)
            .Cast<OneOfAttribute>()
            .FirstOrDefault();

    private static IEnumerable<Type> GetCustomOpenApiClasses() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsDefined(typeof(CustomOpenApiAttribute), false));
}
