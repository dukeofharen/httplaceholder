﻿using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to set the HTTP status code.
/// </summary>
public class StatusCodeResponseWriter : IResponseWriter
{
    public int Priority => 0;

    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (response.StatusCode != 0)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        response.StatusCode = stub.Response?.StatusCode ?? 200;
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
