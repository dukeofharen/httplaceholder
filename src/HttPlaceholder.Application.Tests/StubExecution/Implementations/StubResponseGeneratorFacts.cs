using System.Collections.Generic;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.TestUtilities.Options;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StubResponseGeneratorFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly Mock<IRequestLoggerFactory> _requestLoggerFactoryMock = new();
    private readonly Mock<IRequestLogger> _requestLoggerMock = new();
    private readonly Mock<IResponseWriter> _responseWriterMock1 = new();
    private readonly Mock<IResponseWriter> _responseWriterMock2 = new();
    private readonly SettingsModel _settings = new() { Storage = new StorageSettingsModel() };

    [TestInitialize]
    public void Initialize()
    {
        _requestLoggerFactoryMock
            .Setup(m => m.GetRequestLogger())
            .Returns(_requestLoggerMock.Object);
        _mocker.Use(_requestLoggerFactoryMock.Object);
        _mocker.Use<IEnumerable<IResponseWriter>>(new[] { _responseWriterMock1.Object, _responseWriterMock2.Object });
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
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
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<StubModel, ResponseModel, CancellationToken>((_, r, _) => r.StatusCode = 401)
            .ReturnsAsync(writeResult1);

        var writeResult2 = StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        _responseWriterMock2
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<StubModel, ResponseModel, CancellationToken>((_, r, _) => r.Headers.Add("X-Api-Key", "12345"))
            .ReturnsAsync(writeResult2);

        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // act
        var result = await generator.GenerateResponseAsync(stub, CancellationToken.None);

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
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<StubModel, ResponseModel, CancellationToken>((_, r, _) => r.StatusCode = 401)
            .ReturnsAsync(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        _responseWriterMock1
            .Setup(m => m.Priority)
            .Returns(10);

        _responseWriterMock2
            .Setup(m => m.WriteToResponseAsync(stub, It.IsAny<ResponseModel>(), It.IsAny<CancellationToken>()))
            .Callback<StubModel, ResponseModel, CancellationToken>((_, r, _) => r.StatusCode = 404)
            .ReturnsAsync(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        _responseWriterMock2
            .Setup(m => m.Priority)
            .Returns(-10);

        var generator = _mocker.CreateInstance<StubResponseGenerator>();

        // act
        var result = await generator.GenerateResponseAsync(stub, CancellationToken.None);

        // assert
        Assert.AreEqual(404, result.StatusCode);
    }
}
