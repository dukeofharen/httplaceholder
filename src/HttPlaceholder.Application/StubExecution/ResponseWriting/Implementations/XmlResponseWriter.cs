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
            var executed = false;
            if (stub.Response?.Xml != null)
            {
                var body = stub.Response.Xml;
                response.Body = Encoding.UTF8.GetBytes(body);
                if (!response.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    response.Headers.Add("Content-Type", "text/xml");
                }

                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}
