using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Configuration;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Moq;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    public abstract class RestApiIntegrationTestBase : IntegrationTestBase
    {
        protected readonly Mock<IClientDataResolver> ClientDataResolverMock = new Mock<IClientDataResolver>();
        internal InMemoryStubSource StubSource;
        protected Mock<IStubSource> ReadOnlyStubSource;

        protected void InitializeRestApiIntegrationTest()
        {
            StubSource = new InMemoryStubSource(Options);
            ReadOnlyStubSource = new Mock<IStubSource>();

            InitializeIntegrationTest(
                new (Type, object)[] {(typeof(IClientDataResolver), ClientDataResolverMock.Object)},
                new[] {StubSource, ReadOnlyStubSource.Object});
        }

        protected void CleanupRestApiIntegrationTest() => CleanupIntegrationTest();

        protected IHttPlaceholderClientFactory GetFactory(string username = null, string password = null)
        {
            var options = Microsoft.Extensions.Options.Options.Create(new HttPlaceholderClientSettings
            {
                BaseUrl = Client.BaseAddress.OriginalString, Username = username, Password = password
            });
            return new HttPlaceholderClientFactory(Client, options);
        }

        protected override void AfterTestServerStart()
        {
        }
    }
}
