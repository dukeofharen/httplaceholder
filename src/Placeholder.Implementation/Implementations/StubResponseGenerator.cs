using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubResponseGenerator : IStubResponseGenerator
   {
      private readonly IAsyncService _asyncService;
      private readonly IFileService _fileService;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IStubContainer _stubContainer;

      public StubResponseGenerator(
         IAsyncService asyncService,
         IFileService fileService,
         IRequestLoggerFactory requestLoggerFactory,
         IStubContainer stubContainer)
      {
         _asyncService = asyncService;
         _fileService = fileService;
         _requestLoggerFactory = requestLoggerFactory;
         _stubContainer = stubContainer;
      }

      public async Task<ResponseModel> GenerateResponseAsync(StubModel stub)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         requestLogger.Log($"Stub with ID '{stub.Id}' found; returning response.");
         var response = new ResponseModel
         {
            StatusCode = 200
         };

         response.StatusCode = stub.Response?.StatusCode ?? 200;
         requestLogger.Log($"Found HTTP status code '{response.StatusCode}'.");

         if (stub.Response?.Text != null)
         {
            response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
            requestLogger.Log($"Found body '{stub.Response?.Text}'");
         }
         else if (stub.Response?.Base64 != null)
         {
            string base64Body = stub.Response.Base64;
            response.Body = Convert.FromBase64String(base64Body);
            string bodyForLogging = base64Body.Length > 10 ? base64Body.Substring(0, 10) : base64Body;
            requestLogger.Log($"Found base64 body: {bodyForLogging}");
         }
         else if (stub.Response?.File != null)
         {
            string finalFilePath = null;
            if (_fileService.FileExists(stub.Response.File))
            {
               finalFilePath = stub.Response.File;
            }
            else
            {
               // File doesn't exist, but might exist in the .yml file folder.
               string yamlFilePath = _stubContainer.GetStubFileDirectory();
               string tempPath = Path.Combine(yamlFilePath, stub.Response.File);
               if (_fileService.FileExists(tempPath))
               {
                  finalFilePath = tempPath;
               }
               else
               {
                  requestLogger.Log($"File '{stub.Response.File}' not found.");
               }
            }

            if (finalFilePath != null)
            {
               requestLogger.Log($"File '{finalFilePath}' found, returning it.");
               response.Body = _fileService.ReadAllBytes(finalFilePath);
            }
         }

         var stubResponseHeaders = stub.Response?.Headers;
         if (stubResponseHeaders != null)
         {
            foreach (var header in stubResponseHeaders)
            {
               requestLogger.Log($"Found header '{header.Key}' with value '{header.Value}'.");
               response.Headers.Add(header.Key, header.Value);
            }
         }

         // Simulate sluggish response here, if configured.
         if (stub.Response?.ExtraDuration.HasValue == true)
         {
            int duration = stub.Response.ExtraDuration.Value;
            requestLogger.Log($"Waiting '{duration}' extra milliseconds.");
            await _asyncService.DelayAsync(duration);
         }

         return response;
      }
   }
}
