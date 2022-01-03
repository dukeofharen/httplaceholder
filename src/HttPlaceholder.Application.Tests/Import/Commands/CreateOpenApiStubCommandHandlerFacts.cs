using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Import.Commands;

[TestClass]
public class CreateOpenApiStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var openApiStubGeneratorMock = _mocker.GetMock<IOpenApiStubGenerator>();
        var handler = _mocker.CreateInstance<CreateOpenApiStubCommandHandler>();

        var expectedResult = Array.Empty<FullStubModel>();
        var request = new CreateOpenApiStubCommand("open api input", true);
        openApiStubGeneratorMock
            .Setup(m => m.GenerateOpenApiStubsAsync(request.OpenApi, request.DoNotCreateStub))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
