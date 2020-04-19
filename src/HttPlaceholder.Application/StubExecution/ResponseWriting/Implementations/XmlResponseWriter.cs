using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    internal class XmlResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.Xml == null)
            {
                return Task.FromResult(false);
            }

            var body = stub.Response.Xml;
            response.Body = Encoding.UTF8.GetBytes(body);
            if (!response.Headers.TryGetValue("Content-Type", out _))
            {
                response.Headers.Add("Content-Type", "text/xml");
            }

            return Task.FromResult(true);
        }
    }
}
