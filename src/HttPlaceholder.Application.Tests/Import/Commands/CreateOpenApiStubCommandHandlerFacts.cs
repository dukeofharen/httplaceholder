﻿using HttPlaceholder.Application.Import.Commands;
using HttPlaceholder.Application.StubExecution;

namespace HttPlaceholder.Application.Tests.Import.Commands;

[TestClass]
public class CreateOpenApiStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow_TenantSet()
    {
        // Arrange
        var openApiStubGeneratorMock = _mocker.GetMock<IOpenApiStubGenerator>();
        var handler = _mocker.CreateInstance<CreateOpenApiStubCommandHandler>();

        const string tenant = "tenant1";
        var expectedResult = Array.Empty<FullStubModel>();
        var request = new CreateOpenApiStubCommand("open api input", true, tenant, "prefix");
        openApiStubGeneratorMock
            .Setup(m => m.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant, "prefix",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task Handle_HappyFlow_TenantNotSet()
    {
        // Arrange
        var openApiStubGeneratorMock = _mocker.GetMock<IOpenApiStubGenerator>();
        var handler = _mocker.CreateInstance<CreateOpenApiStubCommandHandler>();

        var expectedResult = Array.Empty<FullStubModel>();
        var request = new CreateOpenApiStubCommand("open api input", true, null, "prefix");
        string capturedTenant = null;
        openApiStubGeneratorMock
            .Setup(m => m.GenerateStubsAsync(request.Input, request.DoNotCreateStub, It.IsAny<string>(), "prefix",
                It.IsAny<CancellationToken>()))
            .Callback<string, bool, string, string, CancellationToken>((_, _, tenant, _, _) => capturedTenant = tenant)
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.IsNotNull(capturedTenant);
        Assert.IsTrue(capturedTenant.StartsWith("openapi-import-"));
    }
}
