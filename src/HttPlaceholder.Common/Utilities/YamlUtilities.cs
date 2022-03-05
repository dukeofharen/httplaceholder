using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class for working with YAML.
/// </summary>
public static class YamlUtilities
{
    /// <summary>
    /// Parses a YAML string.
    /// </summary>
    /// <param name="input">The YAML string.</param>
    /// <typeparam name="TObject">The type the string should be deserialized into.</typeparam>
    /// <returns>The deserialized YAML string.</returns>
    public static TObject Parse<TObject>(string input) => BuildDeserializer().Deserialize<TObject>(input);

    /// <summary>
    /// Serializes an object to YAML.
    /// </summary>
    /// <param name="input">The object to serialize.</param>
    /// <returns>The serialized YAML.</returns>
    public static string Serialize(object input) => BuildSerializer().Serialize(input);

    /// <summary>
    /// Builds a YAML deserializer.
    /// </summary>
    /// <returns>The YAML deserializer.</returns>
    public static IDeserializer BuildDeserializer() =>
        new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

    /// <summary>
    /// Builds a YAML serializer.
    /// </summary>
    /// <returns>The YAML serializer.</returns>
    public static ISerializer BuildSerializer() =>
        new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
}
