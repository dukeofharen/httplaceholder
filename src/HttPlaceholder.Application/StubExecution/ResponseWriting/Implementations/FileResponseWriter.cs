using System.IO;
using System.Threading.Tasks;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class FileResponseWriter : IResponseWriter
    {
        private readonly IFileService _fileService;
        private readonly IStubRootPathResolver _stubRootPathResolver;

        public FileResponseWriter(
           IFileService fileService,
           IStubRootPathResolver stubRootPathResolver)
        {
            _fileService = fileService;
            _stubRootPathResolver = stubRootPathResolver;
        }

        public int Priority => 0;

        public Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (stub.Response?.File != null)
            {
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
                }

                if (finalFilePath != null)
                {
                    response.Body = _fileService.ReadAllBytes(finalFilePath);
                    response.BodyIsBinary = true;
                    executed = true;
                }
            }

            return Task.FromResult(executed);
        }
    }
}
