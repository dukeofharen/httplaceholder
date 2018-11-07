using System.Collections.Generic;
using Ducode.Essentials.Assembly.Interfaces;
using Ducode.Essentials.Files.Interfaces;
using HttPlaceholder.DataLogic.Implementations;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.DataLogic.Tests.Implementations
{
    [TestClass]
    public class StubRootPathResolverFacts
    {
        private Mock<IAssemblyService> _assemblyServiceMock;
        private Mock<IConfigurationService> _configurationServiceMock;
        private Mock<IFileService> _fileServiceMock;
        private IDictionary<string, string> _config;
        private StubRootPathResolver _resolver;

        [TestInitialize]
        public void Initialize()
        {
            _assemblyServiceMock = new Mock<IAssemblyService>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            _fileServiceMock = new Mock<IFileService>();
            _resolver = new StubRootPathResolver(
               _assemblyServiceMock.Object,
               _configurationServiceMock.Object,
               _fileServiceMock.Object);

            _config = new Dictionary<string, string>();
            _configurationServiceMock
               .Setup(m => m.GetConfiguration())
               .Returns(_config);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _assemblyServiceMock.VerifyAll();
            _configurationServiceMock.VerifyAll();
            _fileServiceMock.VerifyAll();
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPath_InputFileSet_InputFileIsDirectory_ShouldReturnInputFileAsIs()
        {
            // arrange
            string inputFile = @"C:\stubs";
            _config.Add(Constants.ConfigKeys.InputFileKey, inputFile);

            _fileServiceMock
               .Setup(m => m.IsDirectory(inputFile))
               .Returns(true);

            // act
            string result = _resolver.GetStubRootPath();

            // assert
            Assert.AreEqual(inputFile, result);
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPath_InputFileSet_InputFileIsFile_ShouldReturnInputFileFolder()
        {
            // arrange
            string inputFilePath = @"C:\stubs";
            string inputFile = $@"{inputFilePath}\stubs.yml";
            _config.Add(Constants.ConfigKeys.InputFileKey, inputFile);

            _fileServiceMock
               .Setup(m => m.IsDirectory(inputFile))
               .Returns(false);

            // act
            string result = _resolver.GetStubRootPath();

            // assert
            Assert.AreEqual(inputFilePath, result);
        }

        [TestMethod]
        public void StubRootPathResolver_GetStubRootPath_InputFileNotSet_ShouldReturnAssemblyPath()
        {
            // arrange
            string assemblyPath = @"C:\stubs\bin";

            _assemblyServiceMock
               .Setup(m => m.GetEntryAssemblyRootPath())
               .Returns(assemblyPath);

            // act
            string result = _resolver.GetStubRootPath();

            // assert
            Assert.AreEqual(assemblyPath, result);
        }
    }
}