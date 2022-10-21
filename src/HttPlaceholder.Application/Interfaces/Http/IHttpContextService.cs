using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace HttPlaceholder.Application.Interfaces.Http;

/// <summary>
///     Describes a class that is used to provide all kinds of data about the current HTTP request.
/// </summary>
public interface IHttpContextService
{
    /// <summary>
    ///     Gets the HTTP method.
    /// </summary>
    string Method { get; }

    /// <summary>
    ///     Gets the current URL path.
    /// </summary>
    string Path { get; }

    /// <summary>
    ///     Gets the current full path (URL path + query string).
    /// </summary>
    string FullPath { get; }

    /// <summary>
    ///     Gets the current display URL (the full URL + protocol and hostname).
    /// </summary>
    string DisplayUrl { get; }

    /// <summary>
    ///     Gets the current root URL (protocol + hostname).
    /// </summary>
    string RootUrl { get; }

    /// <summary>
    ///     Gets the posted body as string.
    /// </summary>
    /// <returns>The posted body as string.</returns>
    string GetBody();

    /// <summary>
    ///     Gets the posted body as byte array.
    /// </summary>
    /// <returns>The posted body as byte array.</returns>
    byte[] GetBodyAsBytes();

    /// <summary>
    ///     Gets the query strings as dictionary.
    /// </summary>
    /// <returns>The query strings as dictionary.</returns>
    IDictionary<string, string> GetQueryStringDictionary();

    /// <summary>
    ///     Gets the full query string.
    /// </summary>
    /// <returns>The full query string.</returns>
    string GetQueryString();

    /// <summary>
    ///     Gets the request headers as dictionary.
    /// </summary>
    /// <returns>The request headers as dictionary.</returns>
    IDictionary<string, string> GetHeaders();

    /// <summary>
    ///     Retrieves an item from the current HttpContext based on the key.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <returns>The object.</returns>
    TObject GetItem<TObject>(string key);

    /// <summary>
    ///     Puts an item on the current HttpContext based on the key.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <param name="item">The item to store.</param>
    void SetItem(string key, object item);

    /// <summary>
    ///     Gets the posted form values as tuple list of string and <see cref="StringValues" />.
    /// </summary>
    /// <returns>The posted form values.</returns>
    (string, StringValues)[] GetFormValues();

    /// <summary>
    ///     Sets the HTTP response status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    void SetStatusCode(int statusCode);

    /// <summary>
    ///     Sets the HTTP response status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    void SetStatusCode(HttpStatusCode statusCode);

    /// <summary>
    ///     Adds an HTTP response header.
    /// </summary>
    /// <param name="key">The HTTP header key.</param>
    /// <param name="values">The HTTP header value.</param>
    void AddHeader(string key, StringValues values);

    /// <summary>
    ///     Tries to add an HTTP response header.
    /// </summary>
    /// <param name="key">The HTTP header key.</param>
    /// <param name="values">The HTTP header value.</param>
    /// <returns>True if the header was added, false otherwise.</returns>
    bool TryAddHeader(string key, StringValues values);

    /// <summary>
    ///     Enable rewind, which means the response body can be read multiple times.
    /// </summary>
    void EnableRewind();

    /// <summary>
    ///     Clears any response that might have already been set up.
    /// </summary>
    void ClearResponse();

    /// <summary>
    ///     Writes response body.
    /// </summary>
    /// <param name="body">The response body as byte array.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteAsync(byte[] body, CancellationToken cancellationToken);

    /// <summary>
    ///     Writes response body.
    /// </summary>
    /// <param name="body">The response body as string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WriteAsync(string body, CancellationToken cancellationToken);

    /// <summary>
    ///     Sets a user on the current HttpContext.
    /// </summary>
    /// <param name="principal">The user.</param>
    void SetUser(ClaimsPrincipal principal);

    /// <summary>
    ///     Aborts the current HTTP request / response.
    /// </summary>
    void AbortConnection();
}
