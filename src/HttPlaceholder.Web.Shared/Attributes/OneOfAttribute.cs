namespace HttPlaceholder.Web.Shared.Attributes;

/// <summary>
///     An attribute to specify the "oneOf" property of OpenAPI.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class OneOfAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the types for the oneOf property.
    /// </summary>
    public Type[] Types { get; set; } = Type.EmptyTypes;

    /// <summary>
    ///     Gets or sets the items types for the oneOf property.
    /// </summary>
    public Type[] ItemsTypes { get; set; } = Type.EmptyTypes;

    /// <summary>
    ///     Gets or sets the additional properties types for the oneOf property.
    /// </summary>
    public Type[] AdditionalPropertiesTypes { get; set; } = Type.EmptyTypes;
}
