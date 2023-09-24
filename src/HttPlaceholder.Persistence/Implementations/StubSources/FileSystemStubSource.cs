using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.FileSystem;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to store and read data on the file system.
/// </summary>
internal class FileSystemStubSource : BaseWritableStubSource
{
    private readonly IDateTime _dateTime;
    private readonly IFileService _fileService;
    private readonly IFileSystemStubCache _fileSystemStubCache;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public FileSystemStubSource(
        IFileService fileService,
        IOptionsMonitor<SettingsModel> options,
        IFileSystemStubCache fileSystemStubCache,
        IDateTime dateTime)
    {
        _fileService = fileService;
        _fileSystemStubCache = fileSystemStubCache;
        _dateTime = dateTime;
        _options = options;
    }

    /// <inheritdoc />
    public override async Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureDirectoriesExist(distributionKey, cancellationToken);

        var requestsFolder = GetRequestsFolder(distributionKey);
        var responsesFolder = GetResponsesFolder(distributionKey);
        await StoreResponseAsync(requestResult, responseModel, cancellationToken, responsesFolder);

        var requestFilePath = Path.Combine(requestsFolder, ConstructRequestFilename(requestResult.CorrelationId));
        var requestContents = JsonConvert.SerializeObject(requestResult);
        await _fileService.WriteAllTextAsync(requestFilePath, requestContents, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task AddStubAsync(StubModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureDirectoriesExist(distributionKey, cancellationToken);

        var path = GetStubsFolder(distributionKey);
        var filePath = Path.Combine(path, ConstructStubFilename(stub.Id));
        var contents = JsonConvert.SerializeObject(stub);
        await _fileService.WriteAllTextAsync(filePath, contents, cancellationToken);
        if (string.IsNullOrWhiteSpace(distributionKey))
        {
            _fileSystemStubCache.AddOrReplaceStub(stub);
        }
    }

    /// <inheritdoc />
    public override async Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var requestFilePath = await FindRequestFilenameAsync(correlationId, distributionKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(requestFilePath))
        {
            return null;
        }

        var contents = await _fileService.ReadAllTextAsync(requestFilePath, cancellationToken);
        return JsonConvert.DeserializeObject<RequestResultModel>(contents);
    }

    /// <inheritdoc />
    public override async Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetResponsesFolder(distributionKey);
        var filePath = Path.Combine(path, ConstructResponseFilename(correlationId));
        if (!await _fileService.FileExistsAsync(filePath, cancellationToken))
        {
            return null;
        }

        var contents = await _fileService.ReadAllTextAsync(filePath, cancellationToken);
        return JsonConvert.DeserializeObject<ResponseModel>(contents);
    }

    /// <inheritdoc />
    public override async Task DeleteAllRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var requestsPath = GetRequestsFolder(distributionKey);
        var files = await _fileService.GetFilesAsync(requestsPath, "*.json", cancellationToken);
        foreach (var filePath in files)
        {
            await _fileService.DeleteFileAsync(filePath, cancellationToken);
            await DeleteResponseAsync(filePath, distributionKey, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var requestFilePath = await FindRequestFilenameAsync(correlationId, distributionKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(requestFilePath))
        {
            return false;
        }

        await DeleteResponseAsync(ConstructResponseFilename(correlationId), distributionKey, cancellationToken);
        await _fileService.DeleteFileAsync(requestFilePath, cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetStubsFolder(distributionKey);
        var filePath = Path.Combine(path, ConstructStubFilename(stubId));
        if (!await _fileService.FileExistsAsync(filePath, cancellationToken))
        {
            return false;
        }

        await _fileService.DeleteFileAsync(filePath, cancellationToken);
        if (string.IsNullOrWhiteSpace(distributionKey))
        {
            _fileSystemStubCache.DeleteStub(stubId);
        }

        return true;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetRequestsFolder(distributionKey);
        var files = (await _fileService.GetFilesAsync(path, "*.json", cancellationToken))
            .OrderByDescending(f => f)
            .ToArray();
        if (pagingModel != null)
        {
            IEnumerable<string> filesQuery = files;
            if (!string.IsNullOrWhiteSpace(pagingModel.FromIdentifier))
            {
                var index = files
                    .Select((file, index) => new {file, index})
                    .Where(f => f.file.Contains(pagingModel.FromIdentifier))
                    .Select(f => f.index)
                    .FirstOrDefault();
                filesQuery = files
                    .Skip(index);
            }

            if (pagingModel.ItemsPerPage.HasValue)
            {
                filesQuery = filesQuery.Take(pagingModel.ItemsPerPage.Value);
            }

            files = filesQuery.ToArray();
        }

        var result = files
            .Select(filePath => _fileService
                .ReadAllTextAsync(filePath, cancellationToken))
            .ToArray();
        var results = await Task.WhenAll(result);
        return results.Select(JsonConvert.DeserializeObject<RequestResultModel>);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<StubModel>> GetStubsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureDirectoriesExist(distributionKey, cancellationToken);
        return await _fileSystemStubCache.GetOrUpdateStubCacheAsync(distributionKey,
            cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<StubModel> GetStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureDirectoriesExist(distributionKey, cancellationToken);
        var stubs = await _fileSystemStubCache.GetOrUpdateStubCacheAsync(distributionKey,
            cancellationToken);
        return stubs.FirstOrDefault(s => s.Id == stubId);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public override async Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default)
    {
        var path = GetRequestsFolder();
        var folders = await _fileService.GetDirectoriesAsync(path, cancellationToken);
        await HandleCleaningOfOldRequests(path, null, cancellationToken);
        foreach (var folder in folders)
        {
            await HandleCleaningOfOldRequests(folder, new DirectoryInfo(folder).Name, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task<ScenarioStateModel> GetScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetScenariosFolder(distributionKey);
        var scenarioPath = Path.Combine(path, ConstructScenarioFilename(scenario));
        if (!await _fileService.FileExistsAsync(scenarioPath, cancellationToken))
        {
            return null;
        }

        var contents = await _fileService.ReadAllTextAsync(scenarioPath, cancellationToken);
        return JsonConvert.DeserializeObject<ScenarioStateModel>(contents);
    }

    /// <inheritdoc />
    public override async Task<ScenarioStateModel> AddScenarioAsync(string scenario,
        ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetScenariosFolder(distributionKey);
        var scenarioPath = Path.Combine(path, ConstructScenarioFilename(scenario));
        if (await _fileService.FileExistsAsync(scenarioPath, cancellationToken))
        {
            throw new InvalidOperationException($"Scenario state with key '{scenario}' already exists.");
        }

        await _fileService.WriteAllTextAsync(scenarioPath, JsonConvert.SerializeObject(scenarioStateModel),
            cancellationToken);
        return scenarioStateModel;
    }

    /// <inheritdoc />
    public override async Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetScenariosFolder(distributionKey);
        var scenarioPath = Path.Combine(path, ConstructScenarioFilename(scenario));
        if (!await _fileService.FileExistsAsync(scenarioPath, cancellationToken))
        {
            throw new InvalidOperationException($"Scenario state with key '{scenario}' not found.");
        }

        await _fileService.WriteAllTextAsync(scenarioPath, JsonConvert.SerializeObject(scenarioStateModel),
            cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetScenariosFolder(distributionKey);
        var files = await _fileService.GetFilesAsync(path, "*.json", cancellationToken);
        var result = new List<ScenarioStateModel>();
        foreach (var file in files)
        {
            var contents = await _fileService.ReadAllTextAsync(file, cancellationToken);
            result.Add(JsonConvert.DeserializeObject<ScenarioStateModel>(contents));
        }

        return result;
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return false;
        }

        var path = GetScenariosFolder(distributionKey);
        var scenarioPath = Path.Combine(path, ConstructScenarioFilename(scenario));
        if (!await _fileService.FileExistsAsync(scenarioPath, cancellationToken))
        {
            return false;
        }

        await _fileService.DeleteFileAsync(scenarioPath, cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public override async Task DeleteAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var path = GetScenariosFolder(distributionKey);
        var files = await _fileService.GetFilesAsync(path, "*.json", cancellationToken);
        foreach (var file in files)
        {
            await _fileService.DeleteFileAsync(file, cancellationToken);
        }
    }

    private async Task HandleCleaningOfOldRequests(string path, string distributionKey,
        CancellationToken cancellationToken)
    {
        var maxLength = _options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
        var filePaths = await _fileService.GetFilesAsync(path, "*.json", cancellationToken);
        var filePathsAndDates = filePaths
            .Select(p => (path: p, lastWriteTime: _fileService.GetLastWriteTime(p)))
            .OrderByDescending(t => t.lastWriteTime)
            .Skip(maxLength);
        foreach (var filePath in filePathsAndDates)
        {
            await _fileService.DeleteFileAsync(filePath.path, cancellationToken);
            await DeleteResponseAsync(filePath.path, distributionKey, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task PrepareStubSourceAsync(CancellationToken cancellationToken)
    {
        await CreateDirectoryIfNotExistsAsync(GetRootFolder(), cancellationToken);
        await EnsureDirectoriesExist(null, cancellationToken);
        await _fileSystemStubCache.GetOrUpdateStubCacheAsync(null, cancellationToken);
    }

    private async Task EnsureDirectoriesExist(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        await CreateDirectoryIfNotExistsAsync(GetRequestsFolder(distributionKey), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetResponsesFolder(distributionKey), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetStubsFolder(distributionKey), cancellationToken);
        await CreateDirectoryIfNotExistsAsync(GetScenariosFolder(distributionKey), cancellationToken);
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

    private string GetStubsFolder(string distributionKey = null) =>
        GetFolderPath(distributionKey, FileNames.StubsFolderName);

    private string GetRequestsFolder(string distributionKey = null) =>
        GetFolderPath(distributionKey, FileNames.RequestsFolderName);

    private string GetResponsesFolder(string distributionKey = null) =>
        GetFolderPath(distributionKey, FileNames.ResponsesFolderName);

    private string GetScenariosFolder(string distributionKey = null) =>
        GetFolderPath(distributionKey, FileNames.ScenariosFolderName);

    private string GetFolderPath(string distributionKey, string folderName) =>
        !string.IsNullOrWhiteSpace(distributionKey)
            ? Path.Combine(GetRootFolder(), distributionKey, folderName)
            : Path.Combine(GetRootFolder(), folderName);

    private async Task DeleteResponseAsync(string filePath, string distributionKey,
        CancellationToken cancellationToken = default)
    {
        var responsesPath = GetResponsesFolder(distributionKey);
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

    private static string ConstructStubFilename(string stubId) => PathUtilities.CleanPath($"{stubId}.json");

    private static string ConstructScenarioFilename(string scenario) =>
        PathUtilities.CleanPath($"scenario-{scenario.ToLower()}.json");

    private static string ConstructOldRequestFilename(string correlationId) => $"{correlationId}.json";

    private string ConstructRequestFilename(string correlationId)
    {
        var unix = _dateTime.UtcNowUnix;
        return $"{unix}-{correlationId}.json";
    }

    internal async Task<string> FindRequestFilenameAsync(string correlationId, string distributionKey,
        CancellationToken cancellationToken)
    {
        var requestsFolder = GetRequestsFolder(distributionKey);
        var oldRequestFilename = ConstructOldRequestFilename(correlationId);
        var oldRequestPath = Path.Join(requestsFolder, oldRequestFilename);
        if (await _fileService.FileExistsAsync(oldRequestPath, cancellationToken))
        {
            return oldRequestPath;
        }

        var files = await _fileService.GetFilesAsync(requestsFolder, $"*-{correlationId}.json", cancellationToken);
        return !files.Any() ? null : files[0];
    }

    private static string ConstructResponseFilename(string correlationId) => $"{correlationId}.json";
}
