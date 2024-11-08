﻿using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to enforce the response to use specific line endings (Windows or UNIX).
/// </summary>
internal class LineEndingResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var lineEndings = stub.Response.LineEndings;
        if (lineEndings is null or LineEndingType.NotSet)
        {
            return IsNotExecuted(GetType().Name).AsTask();
        }

        if (response.BodyIsBinary)
        {
            return IsNotExecuted(GetType().Name, StubResources.LineEndingBodyIsBinary).AsTask();
        }

        switch (lineEndings)
        {
            case LineEndingType.Unix:
                response.Body = ReplaceLineEndings(response.Body, "\n");
                break;
            case LineEndingType.Windows:
                response.Body = ReplaceLineEndings(response.Body, "\r\n");
                break;
            default:
                return IsNotExecuted(GetType().Name,
                        string.Format(StubResources.LineEndingNotSupported, lineEndings,
                            string.Join(',', new[] { LineEndingType.Unix, LineEndingType.Windows })))
                    .AsTask();
        }

        return IsExecuted(GetType().Name).AsTask();
    }

    /// <inheritdoc />
    public int Priority => -10;

    private static byte[] ReplaceLineEndings(byte[] input, string lineEndingSeparator) =>
        Encoding.UTF8.GetBytes(string.Join(
            lineEndingSeparator,
            Encoding.UTF8.GetString(input).SplitNewlines()));
}
