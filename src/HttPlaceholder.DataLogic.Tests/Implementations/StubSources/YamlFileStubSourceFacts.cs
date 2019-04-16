using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.Services.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.DataLogic.Tests.Implementations.StubSources
{
    [TestClass]
    public class YamlFileStubSourceFacts
    {
        private readonly IDictionary<string, string> _config = new Dictionary<string, string>();
        private readonly Mock<IConfigurationService> _configurationServiceMock = new Mock<IConfigurationService>();
        private readonly Mock<ILogger<YamlFileStubSource>> _loggerMock = new Mock<ILogger<YamlFileStubSource>>();
        private readonly Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
        private readonly HashingService _hashingService = new HashingService();
        private readonly YamlService _yamlService = new YamlService();
        private readonly JsonService _jsonService = new JsonService();
        private YamlFileStubSource _source;

        [TestInitialize]
        public void Initialize()
        {
            _source = new YamlFileStubSource(
                _configurationServiceMock.Object,
                _fileServiceMock.Object,
                _hashingService,
                _jsonService,
                _loggerMock.Object,
                _yamlService);

            _configurationServiceMock
                .Setup(m => m.GetConfiguration())
                .Returns(_config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _configurationServiceMock.VerifyAll();
            _loggerMock.VerifyAll();
            _fileServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory()
        {
            // arrange
            string currentDirectory = @"C:\stubs";
            var files = new[]
            {
                $@"{currentDirectory}\file1.yml",
                $@"{currentDirectory}\file2.yml"
            };

            _fileServiceMock
                .Setup(m => m.GetCurrentDirectory())
                .Returns(currentDirectory);

            _fileServiceMock
                .Setup(m => m.GetFiles(currentDirectory, "*.yml"))
                .Returns(files);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[0]))
                .Returns(TestResources.YamlFile1);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[1]))
                .Returns(TestResources.YamlFile2);

            // act
            var result = await _source.GetStubsAsync();

            // assert
            var ids = result.Select(s => s.Id).ToArray();
            Assert.AreEqual("situation-01", ids[0]);
            Assert.AreEqual("situation-02", ids[1]);
            Assert.AreEqual("situation-post-01", ids[2]);
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory_NoFilesFound_ShouldReturnEmptyList()
        {
            // arrange
            string currentDirectory = @"C:\stubs";
            _fileServiceMock
                .Setup(m => m.GetCurrentDirectory())
                .Returns(currentDirectory);

            _fileServiceMock
                .Setup(m => m.GetFiles(currentDirectory, "*.yml"))
                .Returns(new string[0]);

            // act
            var result = await _source.GetStubsAsync();

            // assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_InputFileSet_ShouldReadFilesFromThatDirectory()
        {
            // arrange
            var files = new[]
            {
                @"C:\stubs\file1.yml",
                @"C:\stubs\file2.yml"
            };
            string inputFile = string.Join("%%", files);
            _config.Add(Constants.ConfigKeys.InputFileKey, inputFile);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[0]))
                .Returns(TestResources.YamlFile1);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[1]))
                .Returns(TestResources.YamlFile2);

            // act
            var result = await _source.GetStubsAsync();

            // assert
            var ids = result.Select(s => s.Id).ToArray();
            Assert.AreEqual("situation-01", ids[0]);
            Assert.AreEqual("situation-02", ids[1]);
            Assert.AreEqual("situation-post-01", ids[2]);
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_InputFileSet_InputFileIsDirectory_ShouldReadFilesFromThatDirectory()
        {
            // arrange
            string inputFile = @"C:\stubs";
            _config.Add(Constants.ConfigKeys.InputFileKey, inputFile);

            var files = new[]
            {
                @"C:\stubs\file1.yml",
                @"C:\stubs\file2.yml"
            };

            _fileServiceMock
                .Setup(m => m.GetFiles(inputFile, "*.yml"))
                .Returns(files);

            _fileServiceMock
                .Setup(m => m.IsDirectory(inputFile))
                .Returns(true);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[0]))
                .Returns(TestResources.YamlFile1);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[1]))
                .Returns(TestResources.YamlFile2);

            // act
            var result = await _source.GetStubsAsync();

            // assert
            var ids = result.Select(s => s.Id).ToArray();
            Assert.AreEqual("situation-01", ids[0]);
            Assert.AreEqual("situation-02", ids[1]);
            Assert.AreEqual("situation-post-01", ids[2]);
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_StubsHaveNoId_IdShouldBeCalculated()
        {
            // arrange
            string inputFile = @"C:\stubs";
            _config.Add(Constants.ConfigKeys.InputFileKey, inputFile);

            var files = new[]
            {
                @"C:\stubs\file3.yml"
            };

            _fileServiceMock
                .Setup(m => m.GetFiles(inputFile, "*.yml"))
                .Returns(files);

            _fileServiceMock
                .Setup(m => m.IsDirectory(inputFile))
                .Returns(true);

            _fileServiceMock
                .Setup(m => m.ReadAllText(files[0]))
                .Returns(TestResources.YamlFile3);

            // act
            var result = await _source.GetStubsAsync();

            // assert
            var ids = result.Select(s => s.Id).ToArray();
            Assert.AreEqual("70c16a10db758f202829b6dcb2f056c9", ids[0]);
            Assert.AreEqual("5936f399c30ef95a76cb5d51e405d115", ids[1]);
        }
    }
}
