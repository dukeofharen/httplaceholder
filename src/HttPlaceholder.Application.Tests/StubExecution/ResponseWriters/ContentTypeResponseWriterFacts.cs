using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ContentTypeResponseWriterFacts
{
    private readonly ContentTypeResponseWriter _writer = new();

    [TestMethod]
    public async Task WriteToResponseAsync_NoValueSetInStub_ShouldNotExecuteResponseWriter()
    {
        // Arrange
        var response = new ResponseModel();
        response.Headers.Add("Content-Type", "text/plain");
        var stub = new StubModel {Response = new StubResponseModel {ContentType = string.Empty}};

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // Arrange
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ValueSetInStub_ShouldExecuteResponseWriter()
    {
        // Arrange
        var response = new ResponseModel();
        response.Headers.Add("content-Type", "text/plain");
        var stub = new StubModel {Response = new StubResponseModel {ContentType = "text/csv"}};

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // Arrange
        Assert.IsTrue(result.Executed);
        Assert.AreEqual("text/csv", response.Headers["Content-Type"]);
    }
}