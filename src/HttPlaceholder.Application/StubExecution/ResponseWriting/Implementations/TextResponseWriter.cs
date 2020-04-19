using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class TextResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response.Text == null)
            {
                return Task.FromResult(false);
            }

            response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
            if (!response.Headers.TryGetValue("Content-Type", out _))
            {
                response.Headers.Add("Content-Type", "text/plain");
            }

            return Task.FromResult(true);
        }
    }
}
