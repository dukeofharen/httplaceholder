using HttPlaceholder.Application.Interfaces;
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
    }
}
