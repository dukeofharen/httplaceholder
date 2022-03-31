using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StubResponseGeneratorFacts
{
    private readonly Mock<IRequestLogger> _requestLoggerMock = new();
    private readonly Mock<IRequestLoggerFactory> _requestLoggerFactoryMock = new();
    private readonly Mock<IResponseWriter> _responseWriterMock1 = new();
    private readonly Mock<IResponseWriter> _responseWriterMock2 = new();
    private readonly SettingsModel _settings = new() {Storage = new StorageSettingsModel()};
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize()
    {
        _requestLoggerFactoryMock
            .Setup(m => m.GetRequestLogger())
            .Returns(_requestLoggerMock.Object);
        _mocker.Use(_requestLoggerFactoryMock.Object);
        _mocker.Use<IEnumerable<IResponseWriter>>(new[] {_responseWriterMock1.Object, _responseWriterMock2.Object});
        _mocker.Use(Options.Create(_settings));
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task StubResponseGenerator_GenerateResponseAsync_HappyFlow()
    {
        // arrange
        var stub = new StubModel();

        var writeResult1 = StubResponseWriterResultModel.IsExecuted(GetType().Name);
        _responseWriterMock1
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((_, r) => r.StatusCode = 401)
            .ReturnsAsync(writeResult1);

        var writeResult2 = StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        _responseWriterMock2
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((_, r) => r.Headers.Add("X-Api-Key", "12345"))
            .ReturnsAsync(writeResult2);

        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // act
        var result = await generator.GenerateResponseAsync(stub);

        // assert
        Assert.AreEqual(401, result.StatusCode);
        Assert.AreEqual("12345", result.Headers["X-Api-Key"]);

        _requestLoggerMock.Verify(m => m.SetResponseWriterResult(writeResult1));
        _requestLoggerMock.Verify(m => m.SetResponseWriterResult(writeResult2), Times.Never);
    }

    [TestMethod]
    public async Task StubResponseGenerator_GenerateResponseAsync_HappyFlow_ResponseWriterPriority()
    {
        // arrange
        var stub = new StubModel();

        _responseWriterMock1
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((_, r) => r.StatusCode = 401)
            .ReturnsAsync(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        _responseWriterMock1
            .Setup(m => m.Priority)
            .Returns(10);

        _responseWriterMock2
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>()))
            .Callback<StubModel, ResponseModel>((_, r) => r.StatusCode = 404)
            .ReturnsAsync(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        _responseWriterMock2
            .Setup(m => m.Priority)
            .Returns(-10);

        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // act
        var result = await generator.GenerateResponseAsync(stub);

        // assert
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public async Task GenerateResponseAsync_StoreResponsesFalse_ShouldNotStoreResponses()
    {
        // Arrange
        var stub = new StubModel();
        _settings.Storage.StoreResponses = false;

        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // Act
        var result = await generator.GenerateResponseAsync(stub);

        // Assert
        stubContextMock.Verify(m => m.SaveResponseAsync(result), Times.Never);
    }

    [TestMethod]
    public async Task GenerateResponseAsync_StoreResponsesTrue_ShouldStoreResponses()
    {
        // Arrange
        var stub = new StubModel();
        _settings.Storage.StoreResponses = true;

        var stubContextMock = _mocker.GetMock<IStubContext>();
        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // Act
        var result = await generator.GenerateResponseAsync(stub);

        // Assert
        stubContextMock.Verify(m => m.SaveResponseAsync(result));
    }
}
