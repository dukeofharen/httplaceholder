using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters
{
    public class DynamicResponseWriter : IResponseWriter
    {
        private readonly IResponseVariableParser _responseVariableParser;

        public DynamicResponseWriter(IResponseVariableParser responseVariableParser)
        {
            _responseVariableParser = responseVariableParser;
        }

        public int Priority => -10;

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response.EnableDynamicMode != true)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }


            // Try to parse and replace the variables in the body.
            if (!response.BodyIsBinary && response.Body != null)
            {
                var parsedBody = _responseVariableParser.Parse(Encoding.UTF8.GetString(response.Body));
                response.Body = Encoding.UTF8.GetBytes(parsedBody);
            }

            // Try to parse and replace the variables in the header values.
            var keys = response.Headers.Keys.ToArray();
            foreach (var key in keys)
            {
                response.Headers[key] = _responseVariableParser.Parse(response.Headers[key]);
            }

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
