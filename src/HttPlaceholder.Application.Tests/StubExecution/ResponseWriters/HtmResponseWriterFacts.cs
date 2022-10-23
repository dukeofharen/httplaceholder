using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class HtmlResponseWriterFacts
{
    private readonly HtmlResponseWriter _writer = new();

    [TestMethod]
    public async Task HtmlResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel {Response = new StubResponseModel {Html = null}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task HtmlResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        const string responseText = "<html>";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel {Response = new StubResponseModel {Html = responseText}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(MimeTypes.HtmlMime, response.Headers[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public async Task
        HtmlResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
    {
        // arrange
        const string responseText = "<html>";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel {Response = new StubResponseModel {Html = responseText}};

        var response = new ResponseModel();
        response.Headers.Add(HeaderKeys.ContentType, MimeTypes.TextMime);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(MimeTypes.TextMime, response.Headers[HeaderKeys.ContentType]);
    }
}
