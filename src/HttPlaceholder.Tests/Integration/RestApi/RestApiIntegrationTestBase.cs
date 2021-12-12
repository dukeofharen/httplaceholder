using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Persistence.Implementations.StubSources;
using Moq;

namespace HttPlaceholder.Tests.Integration.RestApi;

public abstract class RestApiIntegrationTestBase : IntegrationTestBase
{
    protected readonly Mock<IClientDataResolver> ClientDataResolverMock = new();
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

    protected override void AfterTestServerStart()
    {
    }
}