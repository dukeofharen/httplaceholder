using HttPlaceholder.Application.StubExecution.ResponseWriters;

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
        response.Headers.Add(HeaderKeys.ContentType, MimeTypes.TextMime);
        var stub = new StubModel { Response = new StubResponseModel { ContentType = string.Empty } };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Arrange
        Assert.IsFalse(result.Executed);
        Assert.AreEqual(MimeTypes.TextMime, response.Headers[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ValueSetInStub_ShouldExecuteResponseWriter()
    {
        // Arrange
        var response = new ResponseModel();
        response.Headers.Add(HeaderKeys.ContentType, MimeTypes.TextMime);
        var stub = new StubModel { Response = new StubResponseModel { ContentType = "text/csv" } };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Arrange
        Assert.IsTrue(result.Executed);
        Assert.AreEqual("text/csv", response.Headers[HeaderKeys.ContentType]);
    }
}
