using System;
using System.IO;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Files.Interfaces;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    public abstract class StubIntegrationTestBase : IntegrationTestBase
    {
        private const string InputFilePath = @"D:\tmp\input.yml";
        protected Mock<IClientIpResolver> _clientIpResolverMock;
        protected Mock<IFileService> _fileServiceMock;
        internal YamlFileStubSource _stubSource;
        protected Mock<IWritableStubSource> _writableStubSourceMock;

        public void InitializeStubIntegrationTest(string yamlFileName)
        {
            // Load the integration YAML file here.
            string path = Path.Combine(AssemblyHelper.GetCallingAssemblyRootPath(), yamlFileName);
            string integrationYml = File.ReadAllText(path);

            _fileServiceMock = new Mock<IFileService>();
            _fileServiceMock
               .Setup(m => m.ReadAllText(InputFilePath))
               .Returns(integrationYml);
            _fileServiceMock
               .Setup(m => m.FileExists(InputFilePath))
               .Returns(true);

            _clientIpResolverMock = new Mock<IClientIpResolver>();
            Settings.Storage.InputFile = InputFilePath;

            _stubSource = new YamlFileStubSource(
                _fileServiceMock.Object,
                new Mock<ILogger<YamlFileStubSource>>().Object,
                Options);
            _writableStubSourceMock = new Mock<IWritableStubSource>();

            InitializeIntegrationTest(new(Type, object)[]
            {
                ( typeof(IClientIpResolver), _clientIpResolverMock.Object ),
                ( typeof(IFileService), _fileServiceMock.Object )
            }, new IStubSource[]
            {
            _stubSource,
            _writableStubSourceMock.Object
            });
        }
    }
}
