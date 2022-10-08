using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to search for a file on the OS and return that file to the client.
/// </summary>
internal class FileResponseWriter : IResponseWriter
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

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response?.File == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        string finalFilePath = null;
        if (await  _fileService.FileExistsAsync(stub.Response.File))
        {
            finalFilePath = stub.Response.File;
        }
        else
        {
            // File doesn't exist, but might exist in the file root folder.
            var stubRootPaths = _stubRootPathResolver.GetStubRootPaths();
            foreach (var path in stubRootPaths)
            {

                var tempPath = Path.Combine(path, stub.Response.File);
                if (await _fileService.FileExistsAsync(tempPath))
                {
                    finalFilePath = tempPath;
                    break;
                }
            }
        }

        if (finalFilePath == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        response.Body = await _fileService.ReadAllBytesAsync(finalFilePath);
        response.BodyIsBinary = true;

        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
