using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class RedirectResponseWriterFacts
{
    private readonly RedirectResponseWriter _writer = new();

    [TestMethod]
    public async Task RedirectResponseWriter_WriteToResponseAsync_NoRedirectSet_ShouldContinue()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel {PermanentRedirect = null, TemporaryRedirect = null}
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual(0, response.StatusCode);
    }

    [TestMethod]
    public async Task RedirectResponseWriter_WriteToResponseAsync_TempRedirect()
    {
        // arrange
        var stub = new StubModel {Response = new StubResponseModel {TemporaryRedirect = "https://google.com"}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(307, response.StatusCode);
        Assert.AreEqual("https://google.com", response.Headers[HeaderKeys.Location]);
    }

    [TestMethod]
    public async Task RedirectResponseWriter_WriteToResponseAsync_PermanentRedirect()
    {
        // arrange
        var stub = new StubModel {Response = new StubResponseModel {PermanentRedirect = "https://google.com"}};

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(301, response.StatusCode);
        Assert.AreEqual("https://google.com", response.Headers[HeaderKeys.Location]);
    }
}
