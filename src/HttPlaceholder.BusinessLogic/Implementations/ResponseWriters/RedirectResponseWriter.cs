using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class RedirectResponseWriter : IResponseWriter
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public RedirectResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         if (stub.Response?.TemporaryRedirect != null)
         {
            string url = stub.Response.TemporaryRedirect;
            requestLogger.Log($"Performing temporary redirect to '{url}'.");
            response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
            response.Headers.Add("Location", url);
         }

         if (stub.Response?.PermanentRedirect != null)
         {
            string url = stub.Response.PermanentRedirect;
            requestLogger.Log($"Performing permanent redirect to '{url}'.");
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.Headers.Add("Location", url);
         }

         return Task.CompletedTask;
      }
   }
}
