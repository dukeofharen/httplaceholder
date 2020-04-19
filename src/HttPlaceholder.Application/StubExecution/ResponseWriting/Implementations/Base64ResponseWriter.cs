using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class Base64ResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.Base64 == null)
            {
                return Task.FromResult(false);
            }

            var base64Body = stub.Response.Base64;
            response.Body = Convert.FromBase64String(base64Body);
            response.BodyIsBinary = true;
            return Task.FromResult(true);
        }
    }
}
