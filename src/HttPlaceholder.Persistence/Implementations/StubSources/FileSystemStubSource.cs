using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ducode.Essentials.Console;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class FileSystemStubSource : IWritableStubSource
    {
        private readonly IConfigurationService _configurationService;
        private readonly IFileService _fileService;
        private readonly IJsonService _jsonService;

        public FileSystemStubSource(
           IConfigurationService configurationService,
           IFileService fileService,
           IJsonService jsonService)
        {
            _configurationService = configurationService;
            _fileService = fileService;
            _jsonService = jsonService;
        }

        public Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            var path = EnsureAndGetRequestsFolder();
            var filePath = Path.Combine(path, $"{requestResult.CorrelationId}.json");
            string contents = _jsonService.SerializeObject(requestResult);
            _fileService.WriteAllText(filePath, contents);
            return Task.CompletedTask;
        }

        public Task AddStubAsync(StubModel stub)
        {
            var path = EnsureAndGetStubsFolder();
            var filePath = Path.Combine(path, $"{stub.Id}.json");
            string contents = _jsonService.SerializeObject(stub);
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
                var request = _jsonService.DeserializeObject<RequestResultModel>(contents);
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
                var stub = _jsonService.DeserializeObject<StubModel>(contents);
                result.Add(stub);
            }

            return Task.FromResult(result.AsEnumerable());
        }

        public async Task CleanOldRequestResultsAsync()
        {
            var path = EnsureAndGetRequestsFolder();
            var config = _configurationService.GetConfiguration();
            int maxLength = config.GetValue(Constants.ConfigKeys.OldRequestsQueueLengthKey, Constants.DefaultValues.MaxRequestsQueueLength);
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
            var config = _configurationService.GetConfiguration();
            string folder = config[Constants.ConfigKeys.FileStorageLocationKey];
            var path = Path.Combine(folder, "stubs");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }

        private string EnsureAndGetRequestsFolder()
        {
            var config = _configurationService.GetConfiguration();
            string folder = config[Constants.ConfigKeys.FileStorageLocationKey];
            var path = Path.Combine(folder, "requests");
            if (!_fileService.DirectoryExists(path))
            {
                _fileService.CreateDirectory(path);
            }

            return path;
        }
    }
}
