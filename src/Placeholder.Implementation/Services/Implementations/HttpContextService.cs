using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Placeholder.Implementation.Services.Implementations
{
   internal class HttpContextService : IHttpContextService
   {
      private readonly HttpContext _httpContext;

      public HttpContextService(IHttpContextAccessor httpContextAccessor)
      {
         _httpContext = httpContextAccessor.HttpContext;
      }

      public string Method => _httpContext.Request.Method;

      public string Path => _httpContext.Request.Path;

      public IDictionary<string, string> GetQueryStringDictionary()
      {
         return _httpContext.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());
      }
   }
}
