using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Import.Commands;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Import.Commands;

[TestClass]
public class CreateCurlStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var curlStubGeneratorMock = _mocker.GetMock<ICurlStubGenerator>();
        var handler = _mocker.CreateInstance<CreateCurlStubCommandHandler>();

        var request = new CreateCurlStubCommand("curl bladibla", true);
        var expectedResult = new[] { new FullStubModel() };
        curlStubGeneratorMock
            .Setup(m => m.GenerateCurlStubsAsync(request.CurlCommand, request.DoNotCreateStub))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}