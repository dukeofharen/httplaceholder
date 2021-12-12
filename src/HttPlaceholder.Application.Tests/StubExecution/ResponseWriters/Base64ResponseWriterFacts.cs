using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class Base64ResponseWriterFacts
{
    private readonly Base64ResponseWriter _writer = new();

    [TestMethod]
    public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Base64 = null
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
    public async Task Base64ResponseWriter_WriteToResponseAsync_HappyFlow()
    {
        // arrange
        var expectedBytes = Encoding.UTF8.GetBytes("TEST!!1!");

        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Base64 = "VEVTVCEhMSE="
            }
        };

        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(expectedBytes.SequenceEqual(response.Body));
    }
}