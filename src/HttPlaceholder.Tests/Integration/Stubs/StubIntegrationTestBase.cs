using System;
using System.Collections.Generic;
using System.IO;
using Ducode.Essentials.Assembly;
using Ducode.Essentials.Files.Interfaces;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.DataLogic;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using HttPlaceholder.Services.Implementations;
using HttPlaceholder.Utilities;
using Microsoft.Extensions.Logging;
using Moq;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    public abstract class StubIntegrationTestBase : IntegrationTestBase
    {
        private const string InputFilePath = @"D:\tmp\input.yml";
        protected Mock<IClientIpResolver> _clientIpResolverMock;
        protected Mock<IConfigurationService> _configurationServiceMock;
        protected Mock<IFileService> _fileServiceMock;
        internal YamlFileStubSource _stubSource;
        protected Mock<IWritableStubSource> _writableStubSourceMock;

        public void InitializeStubIntegrationTest(string yamlFileName)
        {
            // Load the integration YAML file here.
            string path = Path.Combine(AssemblyHelper.GetExecutingAssemblyRootPath(), yamlFileName);
            string integrationYml = File.ReadAllText(path);

            _fileServiceMock = new Mock<IFileService>();
            _fileServiceMock
               .Setup(m => m.ReadAllText(InputFilePath))
               .Returns(integrationYml);
            _fileServiceMock
               .Setup(m => m.FileExists(InputFilePath))
               .Returns(true);

            _clientIpResolverMock = new Mock<IClientIpResolver>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            var config = new Dictionary<string, string>
         {
            { Constants.ConfigKeys.InputFileKey, InputFilePath }
         };
            _configurationServiceMock
               .Setup(m => m.GetConfiguration())
               .Returns(config);

            _stubSource = new YamlFileStubSource(
               _configurationServiceMock.Object,
               _fileServiceMock.Object,
               new Mock<ILogger<YamlFileStubSource>>().Object,
               new YamlService());
            _writableStubSourceMock = new Mock<IWritableStubSource>();

            InitializeIntegrationTest(new(Type, object)[]
            {
                ( typeof(IClientIpResolver), _clientIpResolverMock.Object ),
                ( typeof(IConfigurationService), _configurationServiceMock.Object ),
                ( typeof(IFileService), _fileServiceMock.Object )
            }, new IStubSource[]
            {
            _stubSource,
            _writableStubSourceMock.Object
            });
        }
    }
}
