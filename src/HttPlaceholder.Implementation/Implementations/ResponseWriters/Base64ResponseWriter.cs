using System;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation.Implementations.ResponseWriters
{
   public class Base64ResponseWriter : IResponseWriter
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public Base64ResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.Base64 != null)
         {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            string base64Body = stub.Response.Base64;
            response.Body = Convert.FromBase64String(base64Body);
            string bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
            requestLogger.Log($"Found base64 body: {bodyForLogging}");
         }

         return Task.CompletedTask;
      }
   }
}
