using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.StubSources;

namespace HttPlaceholder.Tests.Integration.RestApi;

public abstract class RestApiIntegrationTestBase : IntegrationTestBase
{
    protected readonly Mock<IClientDataResolver> ClientDataResolverMock = new();
    protected Mock<IStubSource> ReadOnlyStubSource;
    internal InMemoryStubSource StubSource;

    protected void InitializeRestApiIntegrationTest()
    {
        StubSource = new InMemoryStubSource(Options);
        ReadOnlyStubSource = new Mock<IStubSource>();

        InitializeIntegrationTest(
            [(typeof(IClientDataResolver), ClientDataResolverMock.Object)],
            new[] { StubSource, ReadOnlyStubSource.Object });
    }

    protected void CleanupRestApiIntegrationTest() => CleanupIntegrationTest();

    protected override void AfterTestServerStart()
    {
    }
}
