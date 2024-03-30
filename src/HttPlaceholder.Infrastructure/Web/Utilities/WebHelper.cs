using System;
using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Infrastructure.Web.Utilities;

/// <summary>
///     A static class with several web related utilities.
/// </summary>
public static class WebHelper
{
    /// <summary>
    ///     Returns the HttpContext. It always makes sure both the HTTP context accessor and HTTP context are set.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <returns>The <see cref="HttpContext"/>.</returns>
    public static HttpContext GetHttpContext(this IHttpContextAccessor httpContextAccessor) =>
        ThrowHelper.ThrowIfNull<HttpContext, InvalidOperationException>(httpContextAccessor?.HttpContext, "HttpContext not set.");
}
