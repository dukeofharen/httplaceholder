using System.IO;
using System.Runtime.InteropServices;
using HttPlaceholder.Common;
using HttPlaceholder.Configuration;
using HttPlaceholder.Persistence.Implementations;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Persistence.Tests.Implementations
{
    [TestClass]
    public class StubRootPathResolverFacts
    {
        private readonly IOptions<SettingsModel> _options = MockSettingsFactory.GetSettings();
        private readonly Mock<IAssemblyService> _assemblyServiceMock = new Mock<IAssemblyService>();
        private readonly Mock<IFileService> _fileServiceMock = new Mock<IFileService>();
        private StubRootPathResolver _resolver;

        [TestInitialize]
        public void Initialize() =>
            _resolver = new StubRootPathResolver(
                _assemblyServiceMock.Object,
                _fileServiceMock.Object,
                _options);

        [TestCleanup]
        public void Cleanup()
        {
            _assemblyServiceMock.VerifyAll();
            _fileServiceMock.VerifyAll();
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPaths_InputFileSet_InputFileIsDirectory_ShouldReturnInputFileAsIs()
        {
            // arrange
            const string inputFile = @"C:\stubs";
            _options.Value.Storage.InputFile = inputFile;

            _fileServiceMock
               .Setup(m => m.IsDirectory(inputFile))
               .Returns(true);

            // act
            var result = _resolver.GetStubRootPaths();

            // assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(inputFile, result[0]);
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPaths_InputFileSet_InputFileIsFile_ShouldReturnInputFileFolder()
        {
            // arrange
            var inputFilePath =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\stubs" : "/opt/httplaceholder";

            var inputFile = Path.Combine($@"{inputFilePath}", "stubs.yml");
            _options.Value.Storage.InputFile = inputFile;

            _fileServiceMock
               .Setup(m => m.IsDirectory(inputFile))
               .Returns(false);

            // act
            var result = _resolver.GetStubRootPaths();

            // assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(inputFilePath, result[0]);
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPaths_InputFileSet_MultiplePaths_ShouldReturnMultiplePaths()
        {
            // arrange
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var path1 = isWindows ? @"C:\stubs1" : "/opt/httplaceholder/stubs1";
            var path2 = isWindows ? @"C:\stubs2\stub.yml" : "/opt/httplaceholder/stubs2/stub.yml";
            var inputFilePath = $"{path1}%%{path2}";

            _options.Value.Storage.InputFile = inputFilePath;

            _fileServiceMock
                .Setup(m => m.IsDirectory(path1))
                .Returns(true);
            _fileServiceMock
                .Setup(m => m.IsDirectory(path2))
                .Returns(false);

            // act
            var result = _resolver.GetStubRootPaths();

            // assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(path1, result[0]);
            Assert.AreEqual(Path.GetDirectoryName(path2), result[1]);
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPath_InputFileNotSet_ShouldReturnAssemblyPath()
        {
            // arrange
            var assemblyPath = Path.Combine(@"C:\stubs\bin");

            _assemblyServiceMock
               .Setup(m => m.GetEntryAssemblyRootPath())
               .Returns(assemblyPath);

            // act
            var result = _resolver.GetStubRootPaths();

            // assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(assemblyPath, result[0]);
        }
    }
}
