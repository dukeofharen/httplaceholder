using System.Collections.Generic;
using System.Net;

namespace Placeholder.Models
{
   public class ResponseModel
   {
      public HttpStatusCode StatusCode { get; set; }

      public string Body { get; set; }

      public IDictionary<string, string> Headers { get; set; }
   }
}
