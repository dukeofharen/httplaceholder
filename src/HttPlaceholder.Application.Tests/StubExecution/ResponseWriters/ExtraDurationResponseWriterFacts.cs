using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ExtraDurationResponseWriterFacts
{
    private readonly Mock<IAsyncService> _asyncServiceMock = new();
    private ExtraDurationResponseWriter _writer;

    [TestInitialize]
    public void Initialize() =>
        _writer = new ExtraDurationResponseWriter(
            _asyncServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _asyncServiceMock.VerifyAll();

    [TestMethod]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = null
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsFalse(result.Executed);
        _asyncServiceMock.Verify(m => m.DelayAsync(It.IsAny<int>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(10, 10)]
    [DataRow("10", 10)]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow(object input, int valuePassedToDelay)
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = input
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        _asyncServiceMock.Verify(m => m.DelayAsync(valuePassedToDelay), Times.Once);
    }

    [TestMethod]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow_Dto()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = new StubExtraDurationModel
                {
                    Min = 10000,
                    Max = 20000
                }
            }
        };

        var response = new ResponseModel();
        int? capturedDuration = null;
        _asyncServiceMock
            .Setup(m => m.DelayAsync(It.IsAny<int>()))
            .Callback<int>(d => capturedDuration = d)
            .Returns(Task.CompletedTask);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsNotNull(capturedDuration);
        Assert.IsTrue(capturedDuration is >= 10000 and <= 20000);
    }

    [TestMethod]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow_Dto_MinNotSet()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = new StubExtraDurationModel
                {
                    Max = 20000
                }
            }
        };

        var response = new ResponseModel();
        int? capturedDuration = null;
        _asyncServiceMock
            .Setup(m => m.DelayAsync(It.IsAny<int>()))
            .Callback<int>(d => capturedDuration = d)
            .Returns(Task.CompletedTask);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsNotNull(capturedDuration);
        Assert.IsTrue(capturedDuration is >= 0 and <= 20000, $"Actual duration: {capturedDuration}");
    }

    [TestMethod]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow_Dto_MaxNotSet()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = new StubExtraDurationModel
                {
                    Min = 15000
                }
            }
        };

        var response = new ResponseModel();
        int? capturedDuration = null;
        _asyncServiceMock
            .Setup(m => m.DelayAsync(It.IsAny<int>()))
            .Callback<int>(d => capturedDuration = d)
            .Returns(Task.CompletedTask);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsNotNull(capturedDuration);
        Assert.IsTrue(capturedDuration is >= 15000 and <= 25000);
    }
}
