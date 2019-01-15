﻿using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    public class HeadersResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            var stubResponseHeaders = stub.Response?.Headers;
            if (stubResponseHeaders != null)
            {
                foreach (var header in stubResponseHeaders)
                {
                    response.Headers.Add(header.Key, header.Value);
                }

                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}