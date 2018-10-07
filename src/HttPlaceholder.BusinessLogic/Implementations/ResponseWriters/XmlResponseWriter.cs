using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    internal class XmlResponseWriter : IResponseWriter
    {
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