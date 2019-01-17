using System;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    public class Base64ResponseWriter : IResponseWriter
    {
        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (stub.Response?.Base64 != null)
            {
                string base64Body = stub.Response.Base64;
                response.Body = Convert.FromBase64String(base64Body);
                response.BodyIsBinary = true;
                string bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
                executed = true;
            }

            return Task.FromResult(executed);
        }
    }
}