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
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var generatorMock = _mocker.GetMock<IHarStubGenerator>();
        var handler = _mocker.CreateInstance<CreateHarStubCommandHandler>();

        var command = new CreateHarStubCommand("har contents", true);
        var expectedResult = Array.Empty<FullStubModel>();
        generatorMock
            .Setup(m => m.GenerateHarStubsAsync(command.Har, command.DoNotCreateStub))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
;    }
}
