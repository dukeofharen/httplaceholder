using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class TextResponseWriterFacts
{
    private readonly TextResponseWriter _writer = new();

    [TestMethod]
    public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel {Response = new StubResponseModel {Text = null}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        const string text = "bla123";
        var expectedBody = Encoding.UTF8.GetBytes(text);
        var stub = new StubModel {Response = new StubResponseModel {Text = text}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
        Assert.AreEqual(Constants.TextMime, response.Headers[Constants.ContentType]);
    }

    [TestMethod]
    public async Task
        TextResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
    {
        // arrange
        const string text = "bla123";
        var expectedBody = Encoding.UTF8.GetBytes(text);
        var stub = new StubModel {Response = new StubResponseModel {Text = text}};

        var response = new ResponseModel();
        response.Headers.Add(Constants.ContentType, Constants.XmlTextMime);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
        Assert.AreEqual(Constants.XmlTextMime, response.Headers[Constants.ContentType]);
    }
}
