using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

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

      public string GetBody()
      {
         using (var bodyStream = new MemoryStream())
         using (var reader = new StreamReader(bodyStream))
         {
            _httpContext.Request.Body.CopyTo(bodyStream);
            _httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            bodyStream.Seek(0, SeekOrigin.Begin);
            var body = reader.ReadToEnd();
            return body;
         }
      }

      public IDictionary<string, string> GetQueryStringDictionary()
      {
         return _httpContext.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToString());
      }
   }
}
