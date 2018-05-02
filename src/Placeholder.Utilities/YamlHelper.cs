using System.IO;
using YamlDotNet.Serialization;

namespace Placeholder.Utilities
{
   public static class YamlHelper
   {
      public static object Parse(string input)
      {
         var reader = new StringReader(input);
         var deserializer = new Deserializer();
         var yamlObject = deserializer.Deserialize(reader);
         return yamlObject;
      }
   }
}
