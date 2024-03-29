﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to search for a file on the OS and return that file to the client.
/// </summary>
internal class FileResponseWriter(
    IFileService fileService,
    IStubRootPathResolver stubRootPathResolver,
    IOptionsMonitor<SettingsModel> options,
    ILogger<FileResponseWriter> logger,
    IMimeService mimeService)
    : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var settings = options.CurrentValue;
        if (stub.Response?.File == null && stub.Response?.TextFile == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var file = stub.Response?.File ?? stub.Response?.TextFile;
        string finalFilePath = null;
        if (await fileService.FileExistsAsync(file, cancellationToken))
        {
            finalFilePath = file;
            if (settings.Stub?.AllowGlobalFileSearch == false)
            {
                throw new InvalidOperationException(
                    $"Path '{finalFilePath}' found, but can't be used because setting '{ConfigKeys.AllowGlobalFileSearch}' is turned off. Turn it on with caution. Use paths relative to the .yml stub files or the file storage location as specified in the configuration.");
            }

            logger.LogInformation($"Path '{finalFilePath}' found.");
        }
        else
        {
            // File doesn't exist, but might exist in the file root folder.
            var stubRootPaths = await stubRootPathResolver.GetStubRootPathsAsync(cancellationToken);
            foreach (var path in stubRootPaths)
            {
                var tempPath = Path.Combine(path, PathUtilities.CleanPath(file));
                if (!await fileService.FileExistsAsync(tempPath, cancellationToken))
                {
                    logger.LogInformation($"Path '{tempPath}' not found.");
                    continue;
                }

                logger.LogInformation($"Path '{tempPath}' found.");
                finalFilePath = tempPath;
                break;
            }
        }

        if (finalFilePath == null)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        response.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, mimeService.GetMimeType(finalFilePath));
        response.Body = await fileService.ReadAllBytesAsync(finalFilePath, cancellationToken);
        response.BodyIsBinary = string.IsNullOrWhiteSpace(stub.Response.TextFile);
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
