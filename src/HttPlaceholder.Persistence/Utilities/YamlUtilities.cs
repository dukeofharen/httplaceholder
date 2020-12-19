using System.IO;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Persistence.Utilities
{
    public static class YamlUtilities
    {
        public static TObject Parse<TObject>(string input)
        {
            var reader = new StringReader(input);
            var deserializer = new Deserializer();
            return deserializer.Deserialize<TObject>(reader);
        }
    }
}
