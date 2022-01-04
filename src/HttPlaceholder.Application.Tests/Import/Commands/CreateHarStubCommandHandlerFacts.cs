using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Import.Commands.CreateHarStub;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Import.Commands;

[TestClass]
public class CreateHarStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestMethod]
    public async Task Handle_HappyFlow_TenantSet()
    {
        // Arrange
        var generatorMock = _mocker.GetMock<IHarStubGenerator>();
        var handler = _mocker.CreateInstance<CreateHarStubCommandHandler>();

        const string tenant = "tenant1";
        var command = new CreateHarStubCommand("har contents", true, tenant);
        var expectedResult = Array.Empty<FullStubModel>();
        generatorMock
            .Setup(m => m.GenerateHarStubsAsync(command.Har, command.DoNotCreateStub, tenant))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
        ;
    }

    [TestMethod]
    public async Task Handle_HappyFlow_TenantNotSet()
    {
        // Arrange
        var generatorMock = _mocker.GetMock<IHarStubGenerator>();
        var handler = _mocker.CreateInstance<CreateHarStubCommandHandler>();

        var command = new CreateHarStubCommand("har contents", true, null);
        var expectedResult = Array.Empty<FullStubModel>();
        string capturedTenant = null;
        generatorMock
            .Setup(m => m.GenerateHarStubsAsync(command.Har, command.DoNotCreateStub, It.IsAny<string>()))
            .Callback<string, bool, string>((_, _, tenant) => capturedTenant = tenant)
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.IsNotNull(capturedTenant);
        Assert.IsTrue(capturedTenant.StartsWith("har-import-"));
    }
}
