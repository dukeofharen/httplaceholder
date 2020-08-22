using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    internal class XmlResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.Xml == null)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            var body = stub.Response.Xml;
            response.Body = Encoding.UTF8.GetBytes(body);
            if (!response.Headers.TryGetValue("Content-Type", out _))
            {
                response.Headers.Add("Content-Type", "text/xml");
            }

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
