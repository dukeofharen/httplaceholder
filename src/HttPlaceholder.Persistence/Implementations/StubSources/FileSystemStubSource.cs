using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Persistence.FileSystem;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to store and read data on the file system.
/// </summary>
internal class FileSystemStubSource : IWritableStubSource
{
    private readonly IFileService _fileService;
    private readonly IFileSystemStubCache _fileSystemStubCache;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public FileSystemStubSource(
        IFileService fileService,
        IOptionsMonitor<SettingsModel> options,
        IFileSystemStubCache fileSystemStubCache)
    {
        _fileService = fileService;
        _fileSystemStubCache = fileSystemStubCache;
        _options = options;
    }

    /// <inheritdoc />
    public async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        CancellationToken cancellationToken)
    {
        var requestsFolder = GetRequestsFolder();
        var responsesFolder = GetResponsesFolder();
        await StoreResponseAsync(requestResult, responseModel, cancellationToken, responsesFolder);

        var requestFilePath = Path.Combine(requestsFolder, ConstructRequestFilename(requestResult.CorrelationId));
        var requestContents = JsonConvert.SerializeObject(requestResult);
        await _fileService.WriteAllTextAsync(requestFilePath, requestContents, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddStubAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var path = GetStubsFolder();
        var filePath = Path.Combine(path, ConstructStubFilename(stub.Id));
        var contents = JsonConvert.SerializeObject(stub);
        await _fileService.WriteAllTextAsync(filePath, contents, cancellationToken);
        _fileSystemStubCache.AddOrReplaceStub(stub);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(
        CancellationToken cancellationToken) =>
        (await GetRequestResultsAsync(cancellationToken))
        .Select(r => new RequestOverviewModel
        {
            Method = r.RequestParameters.Method,
            Url = r.RequestParameters.Url,
            CorrelationId = r.CorrelationId,
            StubTenant = r.StubTenant,
            ExecutingStubId = r.ExecutingStubId,
            RequestBeginTime = r.RequestBeginTime,
            RequestEndTime = r.RequestEndTime,
            HasResponse = r.HasResponse
        });

    /// <inheritdoc />
    public async Task<RequestResultModel> GetRequestAsync(string correlationId, CancellationToken cancellationToken)
    {
        var path = GetRequestsFolder();
        var filePath = Path.Combine(path, ConstructRequestFilename(correlationId));
        if (!await _fileService.FileExistsAsync(filePath, cancellationToken))
        {
            return null;
        }

        var contents = await _fileService.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<RequestResultModel>(contents);
    }

    /// <inheritdoc />
    public async Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken)
    {
        var path = GetResponsesFolder();
        var filePath = Path.Combine(path, ConstructResponseFilename(correlationId));
        if (!await _fileService.FileExistsAsync(filePath, cancellationToken))
        {
            return null;
        }

        var contents = await _fileService.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<ResponseModel>(contents);
    }

    /// <inheritdoc />
    public async Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken)
    {
        var requestsPath = GetRequestsFolder();
        var files = await _fileService.GetFilesAsync(requestsPath, "*.json", cancellationToken);
        foreach (var filePath in files)
        {
            await _fileService.DeleteFileAsync(filePath, cancellationToken);
            await DeleteResponseAsync(filePath, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken)
    {
        var requestsPath = GetRequestsFolder();
        var requestFilePath = Path.Combine(requestsPath, ConstructRequestFilename(correlationId));
        if (!await _fileService.FileExistsAsync(requestFilePath, cancellationToken))
        {
            return false;
        }

        await DeleteResponseAsync(ConstructResponseFilename(correlationId), cancellationToken);
        await _fileService.DeleteFileAsync(requestFilePath, cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken)
    {
        var path = GetStubsFolder();
        var filePath = Path.Combine(path, ConstructStubFilename(stubId));
        if (!await _fileService.FileExistsAsync(filePath, cancellationToken))
        {
            return false;
        }

        await _fileService.DeleteFileAsync(filePath, cancellationToken);
        _fileSystemStubCache.DeleteStub(stubId);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(CancellationToken cancellationToken)
    {
        var path = GetRequestsFolder();
        var files = await _fileService.GetFilesAsync(path, "*.json", cancellationToken);
        var result = files
            .Select(filePath => _fileService
                .ReadAllTextAsync(filePath, cancellationToken))
            .ToArray();
        var results = await Task.WhenAll(result);
        return results.Select(JsonConvert.DeserializeObject<RequestResultModel>);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken) =>
        await _fileSystemStubCache.GetOrUpdateStubCacheAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken)
    {
        var stubs = await _fileSystemStubCache.GetOrUpdateStubCacheAsync(cancellationToken);
        return stubs.FirstOrDefault(s => s.Id == stubId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken) =>
        (await GetStubsAsync(cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken)
    {
        var path = GetRequestsFolder();
        var maxLength = _options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
        var filePaths = await _fileService.GetFilesAsync(path, "*.json", cancellationToken);
        var filePathsAndDates = filePaths
            .Select(p => (path: p, lastWriteTime: _fileService.GetLastWriteTime(p)))
            .OrderByDescending(t => t.lastWriteTime)
            .Skip(maxLength);
        foreach (var filePath in filePathsAndDates)
        {
            await _fileService.DeleteFileAsync(filePath.path, cancellationToken);
            await DeleteResponseAsync(filePath.path, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        await CreateDirectoryIfNotExistsAsync(GetRootFolder(), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetRequestsFolder(), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetResponsesFolder(), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetStubsFolder(), cancellationToken);
        await _fileSystemStubCache.GetOrUpdateStubCacheAsync(cancellationToken);
    }

    private string GetRootFolder()
    {
        var folder = _options.CurrentValue.Storage?.FileStorageLocation;
        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new InvalidOperationException("File storage location unexpectedly not set.");
        }

        return folder;
    }

    private string GetStubsFolder() =>
        Path.Combine(GetRootFolder(), FileNames.StubsFolderName);

    private string GetRequestsFolder() =>
        Path.Combine(GetRootFolder(), FileNames.RequestsFolderName);

    private string GetResponsesFolder() =>
        Path.Combine(GetRootFolder(), FileNames.ResponsesFolderName);

    private async Task DeleteResponseAsync(string filePath, CancellationToken cancellationToken)
    {
        var responsesPath = GetResponsesFolder();
        var responseFileName = Path.GetFileName(filePath);
        var responseFilePath = Path.Combine(responsesPath, responseFileName);
        if (await _fileService.FileExistsAsync(responseFilePath, cancellationToken))
        {
            await _fileService.DeleteFileAsync(responseFilePath, cancellationToken);
        }
    }

    private async Task StoreResponseAsync(
        RequestResultModel requestResult,
        ResponseModel responseModel,
        CancellationToken cancellationToken,
        string responsesFolder)
    {
        if (responseModel != null)
        {
            requestResult.HasResponse = true;
            var responseFilePath =
                Path.Combine(responsesFolder, ConstructResponseFilename(requestResult.CorrelationId));
            var responseContents = JsonConvert.SerializeObject(responseModel);
            await _fileService.WriteAllTextAsync(responseFilePath, responseContents, cancellationToken);
        }
    }

    private async Task CreateDirectoryIfNotExistsAsync(string path, CancellationToken cancellationToken)
    {
        if (!await _fileService.DirectoryExistsAsync(path, cancellationToken))
        {
            await _fileService.CreateDirectoryAsync(path, cancellationToken);
        }
    }

    private static string ConstructStubFilename(string stubId) => $"{stubId}.json";

    private static string ConstructRequestFilename(string correlationId) => $"{correlationId}.json";

    private static string ConstructResponseFilename(string correlationId) => $"{correlationId}.json";
}
