using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class XmlResponseWriterFacts
{
    private readonly XmlResponseWriter _writer = new();

    [TestMethod]
    public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Xml = null
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        const string responseText = "<xml>";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Xml = responseText
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(Constants.XmlMime, response.Headers["Content-Type"]);
    }

    [TestMethod]
    public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
    {
        // arrange
        const string responseText = "<xml>";
        var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Xml = responseText
            }
        };

        var response = new ResponseModel();
        response.Headers.Add("Content-Type", Constants.TextMime);

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
        Assert.AreEqual(Constants.TextMime, response.Headers["Content-Type"]);
    }
}
