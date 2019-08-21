using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Persistence.Implementations.StubSources;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Persistence.Tests.Implementations.StubSources
{
    [TestClass]
    public class YamlFileStubSourceFacts
    {
        private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
        private readonly Mock<ILogger<YamlFileStubSource>> _loggerMock = new Mock<ILogger<YamlFileStubSource>>();
        private readonly Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
        private YamlFileStubSource _source;

        [TestInitialize]
        public void Initialize()
        {
            _source = new YamlFileStubSource(
                _fileServiceMock.Object,
                _loggerMock.Object,
                _options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _loggerMock.VerifyAll();
            _fileServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task YamlFileStubSource_GetStubsAsync_NoInputFileSet_ShouldReadFilesFromCurrentDirectory()
        {
            // arrange
            var currentDirectory = @"C:\stubs";
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
            var currentDirectory = @"C:\stubs";
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
            _options.Value.Storage.InputFile = string.Join("%%", files);

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
            _options.Value.Storage.InputFile = inputFile;

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
            var inputFile = @"C:\stubs";
            _options.Value.Storage.InputFile = inputFile;

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
            Assert.AreEqual("94f62b43bdf013c9f75e8786275f13e5", ids[0]);
            Assert.AreEqual("1341ca208eaa83efea41d7043599da8c", ids[1]);
        }
    }
}
