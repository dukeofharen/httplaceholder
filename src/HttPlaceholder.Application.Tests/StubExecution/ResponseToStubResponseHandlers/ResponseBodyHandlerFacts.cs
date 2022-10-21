using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class ResponseBodyHandlerFacts
{
    private readonly ResponseBodyHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_Base64()
    {
        // Arrange
        var response = new HttpResponseModel {Content = "bW9p", ContentIsBase64 = true};
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(stubResponse.Html);
        Assert.IsNull(stubResponse.Text);
        Assert.IsNull(stubResponse.Json);
        Assert.IsNull(stubResponse.Xml);
        Assert.AreEqual(response.Content, stubResponse.Base64);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_Json()
    {
        // Arrange
        var response = new HttpResponseModel {Content = "{}"};
        var stubResponse = new StubResponseModel {ContentType = Constants.JsonMime};

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(stubResponse.Html);
        Assert.IsNull(stubResponse.Text);
        Assert.AreEqual(response.Content, stubResponse.Json);
        Assert.IsNull(stubResponse.Xml);
        Assert.IsNull(stubResponse.Base64);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_Html()
    {
        // Arrange
        var response = new HttpResponseModel {Content = "<html>"};
        var stubResponse = new StubResponseModel {ContentType = Constants.HtmlMime};

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(response.Content, stubResponse.Html);
        Assert.IsNull(stubResponse.Text);
        Assert.IsNull(stubResponse.Json);
        Assert.IsNull(stubResponse.Xml);
        Assert.IsNull(stubResponse.Base64);
    }

    [DataTestMethod]
    [DataRow(Constants.XmlApplicationMime)]
    [DataRow(Constants.XmlTextMime)]
    public async Task HandleStubGenerationAsync_Xml(string mimeType)
    {
        // Arrange
        var response = new HttpResponseModel {Content = "<xml>"};
        var stubResponse = new StubResponseModel {ContentType = mimeType};

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(stubResponse.Html);
        Assert.IsNull(stubResponse.Text);
        Assert.IsNull(stubResponse.Json);
        Assert.AreEqual(response.Content, stubResponse.Xml);
        Assert.IsNull(stubResponse.Base64);
    }

    [DataTestMethod]
    [DataRow(Constants.TextMime)]
    [DataRow("application/vnd.ms-excel")]
    [DataRow(null)]
    public async Task HandleStubGenerationAsync_Text(string mimeType)
    {
        // Arrange
        var response = new HttpResponseModel {Content = "plain text"};
        var stubResponse = new StubResponseModel {ContentType = mimeType};

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(stubResponse.Html);
        Assert.AreEqual(response.Content, stubResponse.Text);
        Assert.IsNull(stubResponse.Json);
        Assert.IsNull(stubResponse.Xml);
        Assert.IsNull(stubResponse.Base64);
    }
}
