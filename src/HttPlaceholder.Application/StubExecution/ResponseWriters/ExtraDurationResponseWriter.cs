using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to add an extra duration to the total execution time of the request.
/// </summary>
internal class ExtraDurationResponseWriter : IResponseWriter, ISingletonService
{
    private static readonly Random _random = new();
    private readonly IAsyncService _asyncService;

    public ExtraDurationResponseWriter(
        IAsyncService asyncService)
    {
        _asyncService = asyncService;
    }

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        // Simulate sluggish response here, if configured.
        if (stub.Response?.ExtraDuration == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        int duration;
        var parsedDuration = ConversionUtilities.ParseInteger(stub.Response.ExtraDuration);
        if (parsedDuration != null)
        {
            duration = parsedDuration.Value;
        }
        else
        {
            var durationDto = ConversionUtilities.Convert<StubExtraDurationModel>(stub.Response.ExtraDuration);
            var min = durationDto?.Min ?? 0;
            var max = durationDto?.Max ?? (durationDto?.Min ?? 0) + 10000;
            duration = _random.Next(min, max);
        }

        await _asyncService.DelayAsync(duration, cancellationToken);
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
