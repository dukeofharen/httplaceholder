using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttPlaceholder.Attributes;
using NJsonSchema;
using NSwag;

namespace HttPlaceholder.Utilities;

/// <summary>
///     A utility class that contains several OpenAPI related utility methods.
/// </summary>
public static class OpenApiUtilities
{
    /// <summary>
    ///     Handles custom logic and updates the given OpenAPI document.
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
                foreach (var oneOfType in oneOfProperty.AdditionalPropertiesTypes)
                {
                    UpdateSchema(document, oneOfType, schemaProperty.AdditionalPropertiesSchema);
                }

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
        var isPrimitiveOrString = IsPrimitiveOrString(oneOfType);
        var schemaIsNew = !document.Components.Schemas.ContainsKey(oneOfType.Name);
        var oneOfTypeSchema =
            schemaIsNew ? JsonSchema.FromType(oneOfType) : document.Components.Schemas[oneOfType.Name];

        if (!isPrimitiveOrString && schemaIsNew)
        {
            RemoveNullTypes(oneOfTypeSchema);
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
                schemaProp.Value.Type = (JsonObjectType)(schemaProp.Value.Type - JsonObjectType.Null);
            }
        }
    }

    private static JsonSchemaProperty GetSchemaProperty(OpenApiDocument document, MemberInfo type, MemberInfo property)
    {
        var schema = document.Components.Schemas[type.Name];
        var schemaProperty = schema.ActualProperties.Single(p =>
            p.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase)).Value;
        return schemaProperty;
    }

    private static OneOfAttribute GetOneOfAttribute(ICustomAttributeProvider property) =>
        property
            .GetCustomAttributes(typeof(OneOfAttribute), false)
            .Cast<OneOfAttribute>()
            .FirstOrDefault();

    private static IEnumerable<Type> GetCustomOpenApiClasses() =>
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsDefined(typeof(CustomOpenApiAttribute), false));
}
