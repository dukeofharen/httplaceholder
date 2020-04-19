using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class DynamicResponseWriter : IResponseWriter
    {
        private readonly IVariableParser _variableParser;

        public DynamicResponseWriter(IVariableParser variableParser)
        {
            _variableParser = variableParser;
        }

        public int Priority => -10;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response.EnableDynamicMode != true)
            {
                return Task.FromResult(false);
            }


            // Try to parse and replace the variables in the body.
            if (!response.BodyIsBinary && response.Body != null)
            {
                var parsedBody = _variableParser.Parse(Encoding.UTF8.GetString(response.Body));
                response.Body = Encoding.UTF8.GetBytes(parsedBody);
            }

            // Try to parse and replace the variables in the header values.
            var keys = response.Headers.Keys.ToArray();
            foreach (var key in keys)
            {
                response.Headers[key] = _variableParser.Parse(response.Headers[key]);
            }

            return Task.FromResult(true);
        }
    }
}
