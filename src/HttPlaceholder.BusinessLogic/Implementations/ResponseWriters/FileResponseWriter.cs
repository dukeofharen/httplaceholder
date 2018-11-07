using System.IO;
using System.Threading.Tasks;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Models;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    public class FileResponseWriter : IResponseWriter
    {
        private readonly IFileService _fileService;
        private readonly IStubContainer _stubContainer;
        private readonly IStubRootPathResolver _stubRootPathResolver;

        public FileResponseWriter(
           IFileService fileService,
           IStubContainer stubContainer,
           IStubRootPathResolver stubRootPathResolver)
        {
            _fileService = fileService;
            _stubContainer = stubContainer;
            _stubRootPathResolver = stubRootPathResolver;
        }

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
                    executed = true;
                }
            }

            return Task.FromResult(executed);
        }
    }
}