using System.Collections.Generic;

namespace Placeholder.Implementation.Services
{
   public interface IHttpContextService
   {
      string Method { get; }

      string Path { get; }

      string GetBody();

      IDictionary<string, string> GetQueryStringDictionary();

      IDictionary<string, string> GetHeaders();
   }
}
