using System.Text;
using System.Threading.Tasks;
using Budgetkar.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations.ResponseWriters
{
   public class TextResponseWriter : IResponseWriter
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public TextResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         if(stub.Response.Text != null)
         {
            response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
            requestLogger.Log($"Found body '{stub.Response?.Text}'");
         }

         return Task.CompletedTask;
      }
   }
}
