using System.Collections.Generic;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class HttpRequestToConditionsServiceFacts
{
    private readonly Mock<IRequestToStubConditionsHandler> _handlerMock1 = new();
    private readonly Mock<IRequestToStubConditionsHandler> _handlerMock2 = new();
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use<IEnumerable<IRequestToStubConditionsHandler>>(new[] { _handlerMock1.Object, _handlerMock2.Object });

    [TestMethod]
    public async Task ConvertToConditionsAsync_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpRequestToConditionsService>();
        var request = new HttpRequestModel();

        // Act
        var result = await service.ConvertToConditionsAsync(request, CancellationToken.None);

        // Assert
        _handlerMock1.Verify(m => m.HandleStubGenerationAsync(request, result, It.IsAny<CancellationToken>()));
        _handlerMock2.Verify(m => m.HandleStubGenerationAsync(request, result, It.IsAny<CancellationToken>()));
    }
}
