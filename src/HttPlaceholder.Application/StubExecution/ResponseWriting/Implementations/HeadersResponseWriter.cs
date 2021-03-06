﻿using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class HeadersResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var stubResponseHeaders = stub.Response?.Headers;
            if (stubResponseHeaders == null || stubResponseHeaders?.Any() != true)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            foreach (var header in stubResponseHeaders)
            {
                response.Headers.Add(header.Key, header.Value);
            }

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
