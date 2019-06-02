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
            bool executed = false;
            if (stub.Response?.Xml != null)
            {
                string body = stub.Response.Xml;
                response.Body = Encoding.UTF8.GetBytes(body);
                string bodyForLogging = body.Length > 10 ? body.Substring(0, 10) : body;
                if (!response.Headers.TryGetValue("Content-Type", out string contentType))
                {
                    response.Headers.Add("Content-Type", "text/xml");
                }

                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}