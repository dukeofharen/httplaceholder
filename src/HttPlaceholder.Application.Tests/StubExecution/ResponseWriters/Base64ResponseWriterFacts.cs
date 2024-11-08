using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class Base64ResponseWriterFacts
{
    private readonly Base64ResponseWriter _writer = new();

    [TestMethod]
    public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { Base64 = null } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        var expectedBytes = "TEST!!1!"u8.ToArray();

        var stub = new StubModel { Response = new StubResponseModel { Base64 = "VEVTVCEhMSE=" } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedBytes.SequenceEqual(response.Body));
        Assert.IsFalse(response.BodyIsBinary);
    }

    [DataTestMethod]
    [DataRow("👾🙇💁🙅🙆🙋🙎🙍", "text/plain", false)]
    [DataRow("👾🙇💁🙅🙆🙋🙎🙍", "application/pdf", true)]
    [DataRow("test123", "application/pdf", false)]
    [DataRow("test123", "text/plain", false)]
    public async Task Base64ResponseWriter_WriteToResponseAsync_BinaryCheck(string base64Body, string contentType,
        bool isBinary)
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(base64Body)), ContentType = contentType } };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(isBinary, response.BodyIsBinary);
    }
}
