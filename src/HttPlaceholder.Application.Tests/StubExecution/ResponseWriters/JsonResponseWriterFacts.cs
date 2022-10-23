using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class JsonResponseWriterFacts
{
    private readonly JsonResponseWriter _writer = new();

    [TestMethod]
    public async Task JsonResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel {Response = new StubResponseModel {Json = null}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task JsonResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        const string responseText = "{}";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel {Response = new StubResponseModel {Json = responseText}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(Constants.JsonMime, response.Headers[HeaderKeys.ContentType]);
    }

    [TestMethod]
    public async Task
        JsonResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
    {
        // arrange
        const string responseText = "{}";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel {Response = new StubResponseModel {Json = responseText}};

        var response = new ResponseModel();
        response.Headers.Add(HeaderKeys.ContentType, Constants.TextMime);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(Constants.TextMime, response.Headers[HeaderKeys.ContentType]);
    }
}
