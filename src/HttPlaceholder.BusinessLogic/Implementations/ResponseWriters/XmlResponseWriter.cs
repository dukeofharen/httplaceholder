using HttPlaceholder.Models;
using HttPlaceholder.Services;
using System.Text;
using System.Threading.Tasks;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class XmlResponseWriter : IResponseWriter
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public XmlResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.Xml != null)
         {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            string jsonBody = stub.Response.Xml;
            response.Body = Encoding.UTF8.GetBytes(jsonBody);
            string bodyForLogging = jsonBody.Length > 10 ? jsonBody.Substring(0, 10) : jsonBody;
            requestLogger.Log($"Found XML body: {bodyForLogging}");
            if (!response.Headers.TryGetValue("Content-Type", out string contentType))
            {
               response.Headers.Add("Content-Type", "text/xml");
            }
         }

         return Task.CompletedTask;
      }
   }
}
