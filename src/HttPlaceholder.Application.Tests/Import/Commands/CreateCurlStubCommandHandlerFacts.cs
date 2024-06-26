﻿using HttPlaceholder.Application.Import.Commands;
using HttPlaceholder.Application.StubExecution;

namespace HttPlaceholder.Application.Tests.Import.Commands;

[TestClass]
public class CreateCurlStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow_TenantSet()
    {
        // Arrange
        var curlStubGeneratorMock = _mocker.GetMock<ICurlStubGenerator>();
        var handler = _mocker.CreateInstance<CreateCurlStubCommandHandler>();

        const string tenant = "tenant1";
        var request = new CreateCurlStubCommand("curl bladibla", true, tenant, "prefix");
        var expectedResult = new[] { new FullStubModel() };
        curlStubGeneratorMock
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
        var curlStubGeneratorMock = _mocker.GetMock<ICurlStubGenerator>();
        var handler = _mocker.CreateInstance<CreateCurlStubCommandHandler>();

        var request = new CreateCurlStubCommand("curl bladibla", true, null, "prefix");
        var expectedResult = new[] { new FullStubModel() };
        string capturedTenant = null;
        curlStubGeneratorMock
            .Setup(m => m.GenerateStubsAsync(request.Input, request.DoNotCreateStub, It.IsAny<string>(), "prefix",
                It.IsAny<CancellationToken>()))
            .Callback<string, bool, string, string, CancellationToken>((_, _, tenant, _, _) => capturedTenant = tenant)
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.IsNotNull(capturedTenant);
        Assert.IsTrue(capturedTenant.StartsWith("curl-import-"));
    }
}
