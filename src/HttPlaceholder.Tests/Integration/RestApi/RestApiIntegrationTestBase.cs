using System;
using System.Collections.Generic;
using System.Text;
using HttPlaceholder.DataLogic;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Services;
using Moq;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    public abstract class RestApiIntegrationTestBase : IntegrationTestBase
    {
        protected Dictionary<string, string> _config;
        internal InMemoryStubSource _stubSource;
        protected Mock<IStubSource> _readOnlyStubSource;
        protected Mock<IConfigurationService> _configurationServiceMock;

        public void InitializeRestApiIntegrationTest()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();
            _stubSource = new InMemoryStubSource(_configurationServiceMock.Object);
            _config = new Dictionary<string, string>();
            _configurationServiceMock
               .Setup(m => m.GetConfiguration())
               .Returns(_config);
            _readOnlyStubSource = new Mock<IStubSource>();

            InitializeIntegrationTest(new(Type, object)[]
            {
            ( typeof(IConfigurationService), _configurationServiceMock.Object )
            }, new[]
            {
            _stubSource,
            _readOnlyStubSource.Object
            });
        }

        public void CleanupRestApiIntegrationTest()
        {
            CleanupIntegrationTest();
        }
    }
}
