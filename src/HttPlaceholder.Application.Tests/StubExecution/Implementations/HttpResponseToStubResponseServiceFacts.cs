using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class HttpResponseToStubResponseServiceFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly Mock<IResponseToStubResponseHandler> _handlerMock1 = new();
    private readonly Mock<IResponseToStubResponseHandler> _handlerMock2 = new();

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use<IEnumerable<IResponseToStubResponseHandler>>(new[]
        {
            _handlerMock1.Object, _handlerMock2.Object
        });

    [TestMethod]
    public async Task ConvertToResponseAsync_HappyFlow()
    {
        // Arrange
        var service = _mocker.CreateInstance<HttpResponseToStubResponseService>();
        var response = new HttpResponseModel();

        // Act
        var result = await service.ConvertToResponseAsync(response);

        // Assert
        _handlerMock1.Verify(m => m.HandleStubGenerationAsync(response, result));
        _handlerMock2.Verify(m => m.HandleStubGenerationAsync(response, result));
    }
}
