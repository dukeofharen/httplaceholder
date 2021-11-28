using System;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters
{
    public class LineEndingResponseWriter : IResponseWriter
    {
        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var lineEndings = stub.Response.LineEndings;
            if (lineEndings is null or LineEndingType.NotSet)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            var log = string.Empty;
            if (response.BodyIsBinary)
            {
                log = "The response body is binary; cannot replace line endings.";
            }
            else
            {
                switch (lineEndings)
                {
                    case LineEndingType.Unix:
                        response.Body = ReplaceLineEndings(response.Body, "\n");
                        break;
                    case LineEndingType.Windows:
                        response.Body = ReplaceLineEndings(response.Body, "\r\n");
                        break;
                    default:
                        log =
                            $"Line ending type '{lineEndings}' is not supported. Options are '{LineEndingType.Unix}' and '{LineEndingType.Windows}'.";
                        break;
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
