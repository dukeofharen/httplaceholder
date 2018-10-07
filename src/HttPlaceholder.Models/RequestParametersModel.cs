using System.Collections.Generic;

namespace HttPlaceholder.Models
{
   public class RequestParametersModel
   {
      public string Method { get; set; }

      public string Url { get; set; }

      public string Body { get; set; }

      public IDictionary<string, string> Headers { get; set; }

      public string ClientIp { get; set; }
   }
}