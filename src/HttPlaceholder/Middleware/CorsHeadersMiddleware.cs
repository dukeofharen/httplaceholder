using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Middleware
{
    public class CorsHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains("ph-api/"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Content-Type");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                if (context.Request.Headers.ContainsKey("Origin") && context.Request.Method.Equals("OPTIONS"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}