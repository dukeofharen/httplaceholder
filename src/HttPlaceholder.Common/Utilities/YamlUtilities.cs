using System.IO;
using YamlDotNet.Serialization;

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
    public static TObject Parse<TObject>(string input)
    {
        var reader = new StringReader(input);
        var deserializer = new Deserializer();
        return deserializer.Deserialize<TObject>(reader);
    }

    /// <summary>
    /// Serializes an object to YAML.
    /// </summary>
    /// <param name="input">The object to serialize.</param>
    /// <returns>The serialized YAML.</returns>
    public static string Serialize(object input)
    {
        var serializer = new Serializer();
        return serializer.Serialize(input);
    }
}
