using System.IO;
using YamlDotNet.Serialization;

namespace Placeholder.Utilities
{
   public static class YamlHelper
   {
      public static TObject Parse<TObject>(string input)
      {
         var reader = new StringReader(input);
         var deserializer = new Deserializer();
         var yamlObject = deserializer.Deserialize<TObject>(reader);
         return yamlObject;
      }
   }
}
