using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class HttpRequestToConditionsServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly Mock<IRequestToStubConditionsHandler> _handlerMock1 = new();
    private readonly Mock<IRequestToStubConditionsHandler> _handlerMock2 = new();

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use<IEnumerable<IRequestToStubConditionsHandler>>(new[]
        {
            _handlerMock1.Object, _handlerMock2.Object
        });

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
