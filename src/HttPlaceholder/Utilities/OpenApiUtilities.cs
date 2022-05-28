using System;
using System.Linq;
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
        var customOpenApiClasses = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsDefined(typeof(CustomOpenApiAttribute), false))
                    .ToArray();
                foreach (var type in customOpenApiClasses)
                {
                    var properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        var oneOfProperty = property
                            .GetCustomAttributes(typeof(OneOfAttribute), false)
                            .Cast<OneOfAttribute>()
                            .FirstOrDefault();
                        if (oneOfProperty != null)
                        {
                            var schema = document.Components.Schemas[type.Name];
                            var schemaProperty = schema.ActualProperties.Single(p =>
                                p.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase)).Value;
                            foreach (var oneOfType in oneOfProperty.Types)
                            {
                                var oneOfTypeSchema = JsonSchema.FromType(oneOfType);
                                var isPrimitiveOrString = oneOfType.IsPrimitive || oneOfType == typeof(string);
                                if (!isPrimitiveOrString &&
                                    !document.Components.Schemas.Any(s => s.Key.Equals(oneOfType.Name)))
                                {
                                    document.Components.Schemas.Add(oneOfType.Name, oneOfTypeSchema);
                                }

                                schemaProperty.OneOf.Add(isPrimitiveOrString
                                    ? oneOfTypeSchema
                                    : new JsonSchema {Reference = oneOfTypeSchema});
                            }

                            foreach (var oneOfType in oneOfProperty.ItemsTypes)
                            {
                                var oneOfTypeSchema = JsonSchema.FromType(oneOfType);
                                var isPrimitiveOrString = oneOfType.IsPrimitive || oneOfType == typeof(string);
                                if (!isPrimitiveOrString &&
                                    !document.Components.Schemas.Any(s => s.Key.Equals(oneOfType.Name)))
                                {
                                    document.Components.Schemas.Add(oneOfType.Name, oneOfTypeSchema);
                                }

                                schemaProperty.Item.OneOf.Add(isPrimitiveOrString
                                    ? oneOfTypeSchema
                                    : new JsonSchema {Reference = oneOfTypeSchema});
                            }
                        }
                    }
                }
    }
}
