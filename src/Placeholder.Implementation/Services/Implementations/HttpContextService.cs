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
   }
}
