using System.Collections.Generic;

namespace Budgetkar.Services
{
   public interface IHttpContextService
   {
      string Method { get; }

      string Path { get; }

      string FullPath { get; }

      string DisplayUrl { get; }

      string GetBody();

      IDictionary<string, string> GetQueryStringDictionary();

      IDictionary<string, string> GetHeaders();

      TObject GetItem<TObject>(string key);

      void SetItem(string key, object item);
   }
}
