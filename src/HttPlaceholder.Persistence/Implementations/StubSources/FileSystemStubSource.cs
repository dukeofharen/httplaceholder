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
/// A stub source that is used to store and read data on the file system.
/// </summary>
internal class FileSystemStubSource : IWritableStubSource
{
    private readonly IFileService _fileService;
    private readonly SettingsModel _settings;
    private readonly IFileSystemStubCache _fileSystemStubCache;

    public FileSystemStubSource(
        IFileService fileService,
        IOptions<SettingsModel> options,
        IFileSystemStubCache fileSystemStubCache)
    {
        _fileService = fileService;
        _fileSystemStubCache = fileSystemStubCache;
        _settings = options.Value;
    }

    /// <inheritdoc />
    public async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel, CancellationToken cancellationToken)
    {
        var requestsFolder = GetRequestsFolder();
        var responsesFolder = GetResponsesFolder();
        if (responseModel != null)
        {
            requestResult.HasResponse = true;
            var responseFilePath =
                Path.Combine(responsesFolder, ConstructResponseFilename(requestResult.CorrelationId));
            var responseContents = JsonConvert.SerializeObject(responseModel);
            await _fileService.WriteAllTextAsync(responseFilePath, responseContents, cancellationToken);
        }

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
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(CancellationToken cancellationToken)
    {
        // This method is not optimized right now.
        var requests = await GetRequestResultsAsync(cancellationToken);
        return requests.Select(r => new RequestOverviewModel
        {
            Method = r.RequestParameters.Method,
            Url = r.RequestParameters.Url,
            CorrelationId = r.CorrelationId,
            StubTenant = r.StubTenant,
            ExecutingStubId = r.ExecutingStubId,
            RequestBeginTime = r.RequestBeginTime,
            RequestEndTime = r.RequestEndTime,
            HasResponse = r.HasResponse
        }).ToArray();
    }

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
        var responsesPath = GetResponsesFolder();
        var requestFilePath = Path.Combine(requestsPath, ConstructRequestFilename(correlationId));
        if (!await _fileService.FileExistsAsync(requestFilePath, cancellationToken))
        {
            return false;
        }

        var responseFilePath = Path.Combine(responsesPath, ConstructResponseFilename(correlationId));
        if (await _fileService.FileExistsAsync(responseFilePath, cancellationToken))
        {
            await _fileService.DeleteFileAsync(responseFilePath, cancellationToken);
        }

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
        // TODO make this thread safe. What if multiple instances of HttPlaceholder are running?
        var path = GetRequestsFolder();
        var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
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
        var rootFolder = GetRootFolder();
        if (!await _fileService.DirectoryExistsAsync(rootFolder, cancellationToken))
        {
            await _fileService.CreateDirectoryAsync(rootFolder, cancellationToken);
        }

        var requestsFolder = GetRequestsFolder();
        if (!await _fileService.DirectoryExistsAsync(requestsFolder, cancellationToken))
        {
            await _fileService.CreateDirectoryAsync(requestsFolder, cancellationToken);
        }

        var responsesFolder = GetResponsesFolder();
        if (!await _fileService.DirectoryExistsAsync(responsesFolder, cancellationToken))
        {
            await _fileService.CreateDirectoryAsync(responsesFolder, cancellationToken);
        }

        var stubsFolder = GetStubsFolder();
        if (!await _fileService.DirectoryExistsAsync(stubsFolder, cancellationToken))
        {
            await _fileService.CreateDirectoryAsync(stubsFolder, cancellationToken);
        }

        await _fileSystemStubCache.GetOrUpdateStubCacheAsync(cancellationToken);
    }

    private string GetRootFolder()
    {
        var folder = _settings.Storage?.FileStorageLocation;
        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new InvalidOperationException("File storage location unexpectedly not set.");
        }

        return folder;
    }

    private string GetStubsFolder() =>
        Path.Combine(GetRootFolder(), Constants.StubsFolderName);

    private string GetRequestsFolder() =>
        Path.Combine(GetRootFolder(), Constants.RequestsFolderName);

    private string GetResponsesFolder() =>
        Path.Combine(GetRootFolder(), Constants.ResponsesFolderName);

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

    private static string ConstructStubFilename(string stubId) => $"{stubId}.json";

    private static string ConstructRequestFilename(string correlationId) => $"{correlationId}.json";

    private static string ConstructResponseFilename(string correlationId) => $"{correlationId}.json";
}
