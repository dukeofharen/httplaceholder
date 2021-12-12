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

    [TestMethod]
    public async Task ExtraDurationResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                ExtraDuration = 10
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        _asyncServiceMock.Verify(m => m.DelayAsync(stub.Response.ExtraDuration.Value), Times.Once);
    }
}