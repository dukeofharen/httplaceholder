﻿using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class TextResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response.Text == null)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
            response.Headers.AddOrReplaceCaseInsensitive("Content-Type", "text/plain", false);

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
