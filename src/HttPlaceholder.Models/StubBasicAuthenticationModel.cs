using YamlDotNet.Serialization;

namespace HttPlaceholder.Models
{
   public class StubBasicAuthenticationModel
   {
      [YamlMember(Alias = "username")]
      public string Username { get; set; }

      [YamlMember(Alias = "password")]
      public string Password { get; set; }

      public override string ToString()
      {
         return $@"[Username = '{Username}', Password = '{Password}']";
      }
   }
}