using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class RedirectResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.TemporaryRedirect != null)
         {
            string url = stub.Response.TemporaryRedirect;
            response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
            response.Headers.Add("Location", url);
         }

         if (stub.Response?.PermanentRedirect != null)
         {
            string url = stub.Response.PermanentRedirect;
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.Headers.Add("Location", url);
         }

         return Task.CompletedTask;
      }
   }
}
