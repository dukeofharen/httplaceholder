using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation.Implementations.ResponseWriters
{
   public class FileResponseWriter : IResponseWriter
   {
      private readonly IFileService _fileService;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IStubContainer _stubContainer;
      private readonly IStubRootPathResolver _stubRootPathResolver;

      public FileResponseWriter(
         IFileService fileService,
         IRequestLoggerFactory requestLoggerFactory,
         IStubContainer stubContainer,
         IStubRootPathResolver stubRootPathResolver)
      {
         _fileService = fileService;
         _requestLoggerFactory = requestLoggerFactory;
         _stubContainer = stubContainer;
         _stubRootPathResolver = stubRootPathResolver;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.File != null)
         {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            string finalFilePath = null;
            if (_fileService.FileExists(stub.Response.File))
            {
               finalFilePath = stub.Response.File;
            }
            else
            {
               // File doesn't exist, but might exist in the file root folder.
               string yamlFilePath = _stubRootPathResolver.GetStubRootPath();
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

         return Task.CompletedTask;
      }
   }
}
