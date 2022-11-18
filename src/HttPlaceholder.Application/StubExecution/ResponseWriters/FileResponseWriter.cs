using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to search for a file on the OS and return that file to the client.
/// </summary>
internal class FileResponseWriter : IResponseWriter, ISingletonService
{
    private readonly IFileService _fileService;
    private readonly IStubRootPathResolver _stubRootPathResolver;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public FileResponseWriter(
        IFileService fileService,
        IStubRootPathResolver stubRootPathResolver,
        IOptionsMonitor<SettingsModel> options)
    {
        _fileService = fileService;
        _stubRootPathResolver = stubRootPathResolver;
        _options = options;
    }

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var settings = _options.CurrentValue;
        if (stub.Response?.File == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        string finalFilePath = null;
        if (await _fileService.FileExistsAsync(stub.Response.File, cancellationToken))
        {
            finalFilePath = stub.Response.File;
            if (settings.Stub?.AllowGlobalFileSearch == false)
            {
                throw new InvalidOperationException(
                    $"Path '{finalFilePath}' found, but can't be used because setting '{ConfigKeys.AllowGlobalFileSearch}' is turned off. Turn it on with caution.");
            }
        }
        else
        {
            // File doesn't exist, but might exist in the file root folder.
            var stubRootPaths = await _stubRootPathResolver.GetStubRootPathsAsync(cancellationToken);
            foreach (var path in stubRootPaths)
            {
                var tempPath = Path.Combine(path, PathUtilities.CleanPath(stub.Response.File));
                if (!await _fileService.FileExistsAsync(tempPath, cancellationToken))
                {
                    continue;
                }

                finalFilePath = tempPath;
                break;
            }
        }

        if (finalFilePath == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        response.Body = await _fileService.ReadAllBytesAsync(finalFilePath, cancellationToken);
        response.BodyIsBinary = true;

        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
