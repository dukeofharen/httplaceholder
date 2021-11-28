using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters
{
    internal class HtmlResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.Html == null)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            var body = stub.Response.Html;
            response.Body = Encoding.UTF8.GetBytes(body);
            response.Headers.AddOrReplaceCaseInsensitive("Content-Type", "text/html", false);

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
