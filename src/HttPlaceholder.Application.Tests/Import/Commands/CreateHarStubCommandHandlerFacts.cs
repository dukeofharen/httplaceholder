using HttPlaceholder.Application.Import.Commands;
using HttPlaceholder.Application.StubExecution;

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
        var command = new CreateHarStubCommand("har contents", true, tenant, "prefix");
        var expectedResult = Array.Empty<FullStubModel>();
        generatorMock
            .Setup(m => m.GenerateStubsAsync(command.Input, command.DoNotCreateStub, tenant, "prefix",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task Handle_HappyFlow_TenantNotSet()
    {
        // Arrange
        var generatorMock = _mocker.GetMock<IHarStubGenerator>();
        var handler = _mocker.CreateInstance<CreateHarStubCommandHandler>();

        var command = new CreateHarStubCommand("har contents", true, null, "prefix");
        var expectedResult = Array.Empty<FullStubModel>();
        string capturedTenant = null;
        generatorMock
            .Setup(m => m.GenerateStubsAsync(command.Input, command.DoNotCreateStub, It.IsAny<string>(), "prefix",
                It.IsAny<CancellationToken>()))
            .Callback<string, bool, string, string, CancellationToken>((_, _, tenant, _, _) => capturedTenant = tenant)
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
        Assert.IsNotNull(capturedTenant);
        Assert.IsTrue(capturedTenant.StartsWith("har-import-"));
    }
}
