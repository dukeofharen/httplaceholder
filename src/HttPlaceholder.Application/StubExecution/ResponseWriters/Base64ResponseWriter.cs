using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

public class Base64ResponseWriter : IResponseWriter
{
    public int Priority => 0;

    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response?.Base64 == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var base64Body = stub.Response.Base64;
        response.Body = Convert.FromBase64String(base64Body);
        response.BodyIsBinary = true;
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}