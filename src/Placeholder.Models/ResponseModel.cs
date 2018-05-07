using System.Collections.Generic;

namespace Placeholder.Models
{
   public class ResponseModel
   {
      public int StatusCode { get; set; }

      public byte[] Body { get; set; }

      public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
   }
}
