using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Application.Interfaces.Persistence;
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
            string contents = JsonConvert.SerializeObject(requestResult);
            _fileService.WriteAllText(filePath, contents);
            return Task.CompletedTask;
        }

        public Task AddStubAsync(StubModel stub)
        {
            var path = EnsureAndGetStubsFolder();
            var filePath = Path.Combine(path, $"{stub.Id}.json");
            string contents = JsonConvert.SerializeObject(stub);
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
            var result = new List<RequestResultModel>();
            foreach (var filePath in files)
            {
                var contents = _fileService.ReadAllText(filePath);
                var request = JsonConvert.DeserializeObject<RequestResultModel>(contents);
                result.Add(request);
            }

            return Task.FromResult(result.AsEnumerable());
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            var path = EnsureAndGetStubsFolder();
            var files = _fileService.GetFiles(path, "*.json");
            var result = new List<StubModel>();
            foreach (var filePath in files)
            {
                var contents = _fileService.ReadAllText(filePath);
                var stub = JsonConvert.DeserializeObject<StubModel>(contents);
                result.Add(stub);
            }

            return Task.FromResult(result.AsEnumerable());
        }

        public async Task CleanOldRequestResultsAsync()
        {
            var path = EnsureAndGetRequestsFolder();
            int maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
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
            string folder = _settings.Storage?.FileStorageLocation;
            string path = Path.Combine(folder, "stubs");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }

        private string EnsureAndGetRequestsFolder()
        {
            string folder = _settings.Storage?.FileStorageLocation;
            string path = Path.Combine(folder, "requests");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }
    }
}
