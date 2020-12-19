using System;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class LineEndingResponseWriter : IResponseWriter
    {
        private readonly ILogger<LineEndingResponseWriter> _logger;

        public LineEndingResponseWriter(ILogger<LineEndingResponseWriter> logger)
        {
            _logger = logger;
        }

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (string.IsNullOrWhiteSpace(stub.Response.LineEndings))
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            var lineEndings = stub.Response.LineEndings;
            var log = string.Empty;
            if (response.BodyIsBinary)
            {
                log = "The response body is binary; cannot replace line endings.";
            }
            else
            {
                if (string.Equals(lineEndings, Constants.UnixLineEndingType, StringComparison.OrdinalIgnoreCase))
                {
                    response.Body = ReplaceLineEndings(response.Body, "\n");
                }
                else if (string.Equals(
                    lineEndings,
                    Constants.WindowsLineEndingType,
                    StringComparison.OrdinalIgnoreCase))
                {
                    response.Body = ReplaceLineEndings(response.Body, "\r\n");
                }
                else
                {
                    log =
                        $"Line ending type '{lineEndings}' is not supported. Options are '{Constants.UnixLineEndingType}' and '{Constants.WindowsLineEndingType}'.";
                }
            }

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name, log));
        }

        public int Priority => -10;

        private static byte[] ReplaceLineEndings(byte[] input, string lineEndingSeparator) =>
            Encoding.UTF8.GetBytes(string.Join(
                lineEndingSeparator,
                Encoding.UTF8.GetString(input).Split(
                    new[] {"\r\n", "\r", "\n"},
                    StringSplitOptions.None
                )));
    }
}
