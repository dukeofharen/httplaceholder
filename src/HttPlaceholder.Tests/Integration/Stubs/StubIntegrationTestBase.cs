using System;
using System.IO;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Files.Interfaces;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    public abstract class StubIntegrationTestBase : IntegrationTestBase
    {
        private const string InputFilePath = @"D:\tmp\input.yml";
        protected Mock<IClientIpResolver> ClientIpResolverMock;
        protected Mock<IFileService> FileServiceMock;
        internal YamlFileStubSource StubSource;
        protected Mock<IWritableStubSource> WritableStubSourceMock;

        public void InitializeStubIntegrationTest(string yamlFileName)
        {
            // Load the integration YAML file here.
            string path = Path.Combine(AssemblyHelper.GetCallingAssemblyRootPath(), yamlFileName);
            string integrationYml = File.ReadAllText(path);

            FileServiceMock = new Mock<IFileService>();
            FileServiceMock
               .Setup(m => m.ReadAllText(InputFilePath))
               .Returns(integrationYml);
            FileServiceMock
               .Setup(m => m.FileExists(InputFilePath))
               .Returns(true);

            ClientIpResolverMock = new Mock<IClientIpResolver>();
            Settings.Storage.InputFile = InputFilePath;

            StubSource = new YamlFileStubSource(
                FileServiceMock.Object,
                new Mock<ILogger<YamlFileStubSource>>().Object,
                Options);
            WritableStubSourceMock = new Mock<IWritableStubSource>();

            InitializeIntegrationTest(new (Type, object)[]
            {
                ( typeof(IClientIpResolver), ClientIpResolverMock.Object ),
                ( typeof(IFileService), FileServiceMock.Object )
            }, new IStubSource[]
            {
            StubSource,
            WritableStubSourceMock.Object
            });
        }
    }
}
