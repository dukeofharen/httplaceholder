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
            var executed = false;
            if (stub.Response?.Base64 != null)
            {
                var base64Body = stub.Response.Base64;
                response.Body = Convert.FromBase64String(base64Body);
                response.BodyIsBinary = true;
                var bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}
