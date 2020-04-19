using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class FileSystemStubSource : IWritableStubSource
    {
        private readonly IFileService _fileService;
        private readonly SettingsModel _settings;

        public FileSystemStubSource(
            IFileService fileService,
            IOptions<SettingsModel> options)
        {
            _fileService = fileService;
            _settings = options.Value;
        }

        public Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            var path = EnsureAndGetRequestsFolder();
            var filePath = Path.Combine(path, $"{requestResult.CorrelationId}.json");
            var contents = JsonConvert.SerializeObject(requestResult);
            _fileService.WriteAllText(filePath, contents);
            return Task.CompletedTask;
        }

        public Task AddStubAsync(StubModel stub)
        {
            var path = EnsureAndGetStubsFolder();
            var filePath = Path.Combine(path, $"{stub.Id}.json");
            var contents = JsonConvert.SerializeObject(stub);
            _fileService.WriteAllText(filePath, contents);
            return Task.CompletedTask;
        }

        public Task DeleteAllRequestResultsAsync()
        {
            var path = EnsureAndGetRequestsFolder();
            var files = _fileService.GetFiles(path, "*.json");
            foreach (var filePath in files)
            {
                _fileService.DeleteFile(filePath);
            }

            return Task.CompletedTask;
        }

        public Task<bool> DeleteStubAsync(string stubId)
        {
            var path = EnsureAndGetStubsFolder();
            var filePath = Path.Combine(path, $"{stubId}.json");
            if (!_fileService.FileExists(filePath))
            {
                return Task.FromResult(false);
            }

            _fileService.DeleteFile(filePath);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            var path = EnsureAndGetRequestsFolder();
            var files = _fileService.GetFiles(path, "*.json");
            var result = files
                .Select(filePath => _fileService
                    .ReadAllText(filePath))
                .Select(JsonConvert.DeserializeObject<RequestResultModel>).ToList();

            return Task.FromResult(result.AsEnumerable());
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            var path = EnsureAndGetStubsFolder();
            var files = _fileService.GetFiles(path, "*.json");
            var result = files
                .Select(filePath => _fileService
                    .ReadAllText(filePath))
                .Select(JsonConvert.DeserializeObject<StubModel>).ToList();

            return Task.FromResult(result.AsEnumerable());
        }

        public async Task CleanOldRequestResultsAsync()
        {
            var path = EnsureAndGetRequestsFolder();
            var maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
            var requests = (await GetRequestResultsAsync())
                .OrderByDescending(r => r.RequestEndTime)
                .Skip(maxLength);
            foreach (var request in requests)
            {
                var filePath = Path.Combine(path, $"{request.CorrelationId}.json");
                _fileService.DeleteFile(filePath);
            }
        }

        public Task PrepareStubSourceAsync()
        {
            EnsureAndGetRequestsFolder();
            EnsureAndGetStubsFolder();
            return Task.CompletedTask;
        }

        private string EnsureAndGetStubsFolder()
        {
            var folder = _settings.Storage?.FileStorageLocation;
            var path = Path.Combine(folder, "stubs");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }

        private string EnsureAndGetRequestsFolder()
        {
            var folder = _settings.Storage?.FileStorageLocation;
            var path = Path.Combine(folder, "requests");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }
    }
}
