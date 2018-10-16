using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Services
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

        string GetClientIp();

        string GetHost();

        bool IsHttps();

        (string, StringValues)[] GetFormValues();

        void SetStatusCode(int statusCode);

        void AddHeader(string key, StringValues values);

        bool TryAddHeader(string key, StringValues values);

        void EnableRewind();

        void ClearResponse();

        Task WriteAsync(byte[] body);
    }
}