namespace HttPlaceholder.Web.Shared.Attributes;

/// <summary>
///     An attribute that specifies that the class it is decorated on has custom OpenAPI stuff going on.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CustomOpenApiAttribute : Attribute;
