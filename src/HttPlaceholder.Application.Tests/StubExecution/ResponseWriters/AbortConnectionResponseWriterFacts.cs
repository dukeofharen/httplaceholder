using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class AbortConnectionResponseWriterFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task WriteToResponseAsync_AbortConnectionNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var writer = _mocker.CreateInstance<AbortConnectionResponseWriter>();

        var stub = new StubModel {Response = new StubResponseModel {AbortConnection = false}};

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsFalse(result.Executed);
        httpContextServiceMock.Verify(m => m.AbortConnection(), Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_AbortConnectionSet_ShouldReturnExecuted()
    {
        // Arrange
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var writer = _mocker.CreateInstance<AbortConnectionResponseWriter>();

        var stub = new StubModel {Response = new StubResponseModel {AbortConnection = true}};

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsTrue(result.Executed);
        httpContextServiceMock.Verify(m => m.AbortConnection());
    }
}
