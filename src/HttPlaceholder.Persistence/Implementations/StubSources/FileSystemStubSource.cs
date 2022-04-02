using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel)
    {
        var path = GetRequestsFolder();
        var filePath = Path.Combine(path, $"{requestResult.CorrelationId}.json");
        var contents = JsonConvert.SerializeObject(requestResult);
        _fileService.WriteAllText(filePath, contents);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddStubAsync(StubModel stub)
    {
        var path = GetStubsFolder();
        var filePath = Path.Combine(path, $"{stub.Id}.json");
        var contents = JsonConvert.SerializeObject(stub);
        _fileService.WriteAllText(filePath, contents);
        _fileSystemStubCache.ClearStubCache();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync()
    {
        // This method is not optimized right now.
        var requests = await GetRequestResultsAsync();
        return requests.Select(r => new RequestOverviewModel
        {
            Method = r.RequestParameters.Method,
            Url = r.RequestParameters.Url,
            CorrelationId = r.CorrelationId,
            StubTenant = r.StubTenant,
            ExecutingStubId = r.ExecutingStubId,
            RequestBeginTime = r.RequestBeginTime,
            RequestEndTime = r.RequestEndTime
        }).ToArray();
    }

    /// <inheritdoc />
    public Task<RequestResultModel> GetRequestAsync(string correlationId)
    {
        var path = GetRequestsFolder();
        var filePath = Path.Combine(path, $"{correlationId}.json");
        if (!_fileService.FileExists(filePath))
        {
            return Task.FromResult((RequestResultModel)null);
        }

        var contents = _fileService.ReadAllText(filePath);
        return Task.FromResult(JsonConvert.DeserializeObject<RequestResultModel>(contents));
    }

    /// <inheritdoc />
    public Task DeleteAllRequestResultsAsync()
    {
        var path = GetRequestsFolder();
        var files = _fileService.GetFiles(path, "*.json");
        foreach (var filePath in files)
        {
            _fileService.DeleteFile(filePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> DeleteRequestAsync(string correlationId)
    {
        var path = GetRequestsFolder();
        var filePath = Path.Combine(path, $"{correlationId}.json");
        if (!_fileService.FileExists(filePath))
        {
            return Task.FromResult(false);
        }

        _fileService.DeleteFile(filePath);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> DeleteStubAsync(string stubId)
    {
        var path = GetStubsFolder();
        var filePath = Path.Combine(path, $"{stubId}.json");
        if (!_fileService.FileExists(filePath))
        {
            return Task.FromResult(false);
        }

        _fileService.DeleteFile(filePath);
        _fileSystemStubCache.ClearStubCache();
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
    {
        var path = GetRequestsFolder();
        var files = _fileService.GetFiles(path, "*.json");
        var result = files
            .Select(filePath => _fileService
                .ReadAllText(filePath))
            .Select(JsonConvert.DeserializeObject<RequestResultModel>).ToList();

        return Task.FromResult(result.AsEnumerable());
    }

    /// <inheritdoc />
    public Task<IEnumerable<StubModel>> GetStubsAsync() =>
        Task.FromResult(_fileSystemStubCache.GetOrUpdateStubCache());

    /// <inheritdoc />
    public Task<StubModel> GetStubAsync(string stubId)
    {
        var stubs = _fileSystemStubCache.GetOrUpdateStubCache();
        return Task.FromResult(stubs.FirstOrDefault(s => s.Id == stubId));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync() =>
        (await GetStubsAsync())
        .Select(s => new StubOverviewModel { Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled })
        .ToArray();

    /// <inheritdoc />
    public Task CleanOldRequestResultsAsync()
    {
        // TODO make this thread safe. What if multiple instances of HttPlaceholder are running?
        var path = GetRequestsFolder();
        var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
        var filePaths = _fileService.GetFiles(path, "*.json");
        var filePathsAndDates = filePaths
            .Select(p => (path: p, lastWriteTime: _fileService.GetLastWriteTime(p)))
            .OrderByDescending(t => t.lastWriteTime)
            .Skip(maxLength);
        foreach (var filePath in filePathsAndDates)
        {
            _fileService.DeleteFile(filePath.path);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task PrepareStubSourceAsync()
    {
        var requestsFolder = GetRequestsFolder();
        if (!_fileService.DirectoryExists(requestsFolder))
        {
            _fileService.CreateDirectory(requestsFolder);
        }

        var stubsFolder = GetStubsFolder();
        if (!_fileService.DirectoryExists(stubsFolder))
        {
            _fileService.CreateDirectory(stubsFolder);
        }

        _fileSystemStubCache.GetOrUpdateStubCache();
        return Task.CompletedTask;
    }

    private string GetStubsFolder() =>
        Path.Combine(_settings.Storage?.FileStorageLocation, Constants.StubsFolderName);

    private string GetRequestsFolder() =>
        Path.Combine(_settings.Storage?.FileStorageLocation, Constants.RequestsFolderName);
}
