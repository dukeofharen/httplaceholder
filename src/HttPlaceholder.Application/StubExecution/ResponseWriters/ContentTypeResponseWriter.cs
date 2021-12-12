using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

public class ContentTypeResponseWriter : IResponseWriter
{
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (string.IsNullOrWhiteSpace(stub.Response?.ContentType))
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", stub.Response.ContentType);
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    public int Priority => -11;
}
