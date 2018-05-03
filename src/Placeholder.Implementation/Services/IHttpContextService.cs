using System.Collections.Generic;

namespace Placeholder.Implementation.Services
{
   public interface IHttpContextService
   {
      string Method { get; }

      string Path { get; }

      IDictionary<string, string> GetQueryStringDictionary();
   }
}
