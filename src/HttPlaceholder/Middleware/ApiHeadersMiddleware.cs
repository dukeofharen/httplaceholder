using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ApiHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains("ph-api/"))
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Content-Type");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Cache-Control", "no-store, no-cache");
                context.Response.Headers.Add("Expires", "-1");
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