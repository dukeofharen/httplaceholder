using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
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

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response?.File == null)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            string finalFilePath = null;
            if (_fileService.FileExists(stub.Response.File))
            {
                finalFilePath = stub.Response.File;
            }
            else
            {
                // File doesn't exist, but might exist in the file root folder.
                var yamlFilePath = _stubRootPathResolver.GetStubRootPaths();
                var tempPath = Path.Combine(yamlFilePath, stub.Response.File);
                if (_fileService.FileExists(tempPath))
                {
                    finalFilePath = tempPath;
                }
            }

            if (finalFilePath == null)
            {
                return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            }

            response.Body = _fileService.ReadAllBytes(finalFilePath);
            response.BodyIsBinary = true;

            return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }
    }
}
