using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Application.Interfaces.Http;

public interface IHttpContextService
{
    string Method { get; }

    string Path { get; }

    string FullPath { get; }

    string DisplayUrl { get; }

    string RootUrl { get; }

    string GetBody();

    byte[] GetBodyAsBytes();

    IDictionary<string, string> GetQueryStringDictionary();

    string GetQueryString();

    IDictionary<string, string> GetHeaders();

    TObject GetItem<TObject>(string key);

    void SetItem(string key, object item);

    (string, StringValues)[] GetFormValues();

    void SetStatusCode(int statusCode);

    void SetStatusCode(HttpStatusCode statusCode);

    void AddHeader(string key, StringValues values);

    bool TryAddHeader(string key, StringValues values);

    void EnableRewind();

    void ClearResponse();

    Task WriteAsync(byte[] body);

    Task WriteAsync(string body);

    void SetUser(ClaimsPrincipal principal);
}
