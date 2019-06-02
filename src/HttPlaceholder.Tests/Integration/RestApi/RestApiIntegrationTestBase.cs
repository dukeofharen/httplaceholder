using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Moq;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    public abstract class RestApiIntegrationTestBase : IntegrationTestBase
    {
        internal InMemoryStubSource _stubSource;
        protected Mock<IStubSource> _readOnlyStubSource;

        public void InitializeRestApiIntegrationTest()
        {
            _stubSource = new InMemoryStubSource(Options);
            _readOnlyStubSource = new Mock<IStubSource>();

            InitializeIntegrationTest(stubSources: new[]
            {
            _stubSource,
            _readOnlyStubSource.Object
            });
        }

        public void CleanupRestApiIntegrationTest()
        {
            CleanupIntegrationTest();
        }

        protected IHttPlaceholderClientFactory GetFactory(string username = null, string password = null)
        {
            var options = Microsoft.Extensions.Options.Options.Create(new HttPlaceholderClientSettings
            {
                BaseUrl = Client.BaseAddress.OriginalString,
                Username = username,
                Password = password
            });
            return new HttPlaceholderClientFactory(Client, options);
        }

        protected override void AfterTestServerStart()
        {
            
        }
    }
}
