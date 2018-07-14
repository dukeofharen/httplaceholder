using System;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   public class Base64ResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.Base64 != null)
         {
            string base64Body = stub.Response.Base64;
            response.Body = Convert.FromBase64String(base64Body);
            string bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
         }

         return Task.CompletedTask;
      }
   }
}
