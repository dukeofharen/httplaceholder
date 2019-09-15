using System;
using System.IO;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    public abstract class StubIntegrationTestBase : IntegrationTestBase
    {
        private const string InputFilePath = @"D:\tmp\input.yml";
        protected Mock<IClientDataResolver> ClientIpResolverMock;
        protected Mock<IFileService> FileServiceMock;
        internal YamlFileStubSource StubSource;
        protected Mock<IWritableStubSource> WritableStubSourceMock;
        protected Mock<IDateTime> DateTimeMock;

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

            DateTimeMock = new Mock<IDateTime>();
            DateTimeMock
                .Setup(m => m.Now)
                .Returns(() => DateTime.Now);
            DateTimeMock
                .Setup(m => m.UtcNow)
                .Returns(() => DateTime.UtcNow);

            ClientIpResolverMock = new Mock<IClientDataResolver>();
            Settings.Storage.InputFile = InputFilePath;

            StubSource = new YamlFileStubSource(
                FileServiceMock.Object,
                new Mock<ILogger<YamlFileStubSource>>().Object,
                Options);
            WritableStubSourceMock = new Mock<IWritableStubSource>();

            InitializeIntegrationTest(
                new (Type, object)[]
                {
                    (typeof(IClientDataResolver), ClientIpResolverMock.Object),
                    (typeof(IFileService), FileServiceMock.Object), (typeof(IDateTime), DateTimeMock.Object)
                }, new IStubSource[] {StubSource, WritableStubSourceMock.Object});
        }
    }
}
