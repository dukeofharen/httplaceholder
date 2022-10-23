using System.Net;
using System.Net.Http;
using System.Text;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubXpathConditionsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularPost_SoapXml_ValidateXPath_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}InStock";
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>GOOG</m:StockName>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, "application/soap+xml"),
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("<result>OK</result>", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.XmlTextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_SoapXml_ValidateXPath_NoNamespacesDefined_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}InStock";
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>SJAAK</m:StockName>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, "application/soap+xml"),
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("<result>OK</result>", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.XmlTextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_SoapXml_ValidateXPath_StubNotFound()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}InStock";
        const string body = @"<?xml version=""1.0""?>
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:m=""http://www.example.org/stock/Reddy"">
  <soap:Header>
  </soap:Header>
  <soap:Body>
    <m:GetStockPrice>
      <m:StockName>GOOGL</m:StockName>
    </m:GetStockPrice>
  </soap:Body>
</soap:Envelope>";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, "application/soap+xml"),
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_RegularXml_ValidateXPath_HappyFlow_XmlResponseWriter()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}xml";
        const string body = @"<?xml version=""1.0""?>
<object>
	<a>TEST</a>
	<b>TEST2</b>
</object>";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, "application/soap+xml"),
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("<result>OK</result>", content);
        Assert.AreEqual(MimeTypes.XmlTextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_RegularXml_ValidateXPath_StubNotFound()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}InStock";
        const string body = @"<?xml version=""1.0""?>
<object>
	<a>TEST!</a>
	<b>TEST2</b>
</object>";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body, Encoding.UTF8, "application/soap+xml"),
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
