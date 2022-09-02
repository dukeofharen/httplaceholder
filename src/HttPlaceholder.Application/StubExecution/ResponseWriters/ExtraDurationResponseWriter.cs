using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to add an extra duration to the total execution time of the request.
/// </summary>
internal class ExtraDurationResponseWriter : IResponseWriter
{
    private static Random _random = new();
    private readonly IAsyncService _asyncService;

    public ExtraDurationResponseWriter(
        IAsyncService asyncService)
    {
        _asyncService = asyncService;
    }

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        // Simulate sluggish response here, if configured.
        if (stub.Response?.ExtraDuration == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        int duration;
        if (stub.Response.ExtraDuration is int extraDuration)
        {
            duration = extraDuration;
        }
        else
        {
            var durationDto = ConversionUtilities.Convert<StubExtraDurationModel>(stub.Response.ExtraDuration);
            var min = durationDto?.Min ?? 0;
            var max = durationDto?.Max ?? 10000;
            duration = _random.Next(min, max);
        }

        await _asyncService.DelayAsync(duration);
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
