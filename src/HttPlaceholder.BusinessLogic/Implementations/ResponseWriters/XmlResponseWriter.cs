﻿using HttPlaceholder.Models;
using System.Text;
using System.Threading.Tasks;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class XmlResponseWriter : IResponseWriter
   {
      public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         bool executed = false;
         if (stub.Response?.Xml != null)
         {
            string jsonBody = stub.Response.Xml;
            response.Body = Encoding.UTF8.GetBytes(jsonBody);
            string bodyForLogging = jsonBody.Length > 10 ? jsonBody.Substring(0, 10) : jsonBody;
            if (!response.Headers.TryGetValue("Content-Type", out string contentType))
            {
               response.Headers.Add("Content-Type", "text/xml");
            }

            executed = true;
         }

         return Task.FromResult(executed);
      }
   }
}
