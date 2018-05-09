using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Placeholder.Implementation.Services;

namespace Placeholder.Tests.Integration
{
   [TestClass]
   public class StubIntegrationTests : IntegrationTestBase
   {
      private const string InputFilePath = @"D:\tmp\input.yml";
      private Mock<IConfiguration> _configurationMock;
      private Mock<IFileService> _fileServiceMock;

      [TestInitialize]
      public void Initialize()
      {
         _fileServiceMock = new Mock<IFileService>();
         _fileServiceMock
            .Setup(m => m.ReadAllText(InputFilePath))
            .Returns(IntegrationTestResources.TestYamlFile);
         _fileServiceMock
            .Setup(m => m.FileExists(InputFilePath))
            .Returns(true);

         _configurationMock = new Mock<IConfiguration>();
         _configurationMock
            .Setup(m => m["inputFile"])
            .Returns(InputFilePath);

         InitializeIntegrationTest(new Dictionary<Type, object>
         {
            { typeof(IConfiguration), _configurationMock.Object },
            { typeof(IFileService), _fileServiceMock.Object }
         });
      }

      [TestCleanup]
      public void Cleanup()
      {
         CleanupIntegrationTest();
      }

      [TestMethod]
      public async Task StubIntegration_ReturnsXPlaceholderCorrelationHeader()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}bla";

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            var header = response.Headers.First(h => h.Key == "X-Placeholder-Correlation").Value.ToArray();
            Assert.AreEqual(1, header.Length);
            Assert.IsFalse(string.IsNullOrWhiteSpace(header.First()));
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularGet_StubNotFound_ShouldReturn500()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}locatieserver/v3/suggest?q=9752EX";

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularGet_ReturnsJson_Scenario1()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}locatieserver/v3/suggest?q=9761BP";

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
            JObject.Parse(content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.ToString());
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularGet_ReturnsJson_Scenario2()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}locatieserver/v3/suggest?q=9752EM";

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
            JObject.Parse(content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.ToString());
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_ValidatePostBody_HappyFlow()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}api/users";
         string body = @"{""username"": ""john""}";
         var request = new HttpRequestMessage
         {
            Content = new StringContent(body),
            Headers =
            {
               { "X-Api-Key", "123abc" },
               { "X-Another-Secret", "sjaaaaaak 123" },
               { "X-Another-Code", "Two Memories" }
            },
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
            JObject.Parse(content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.ToString());
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_ValidatePostBody_StubNotFound()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}api/users";
         string body = @"{""username"": ""jack""}";
         var request = new HttpRequestMessage
         {
            Content = new StringContent(body),
            Headers =
            {
               { "X-Api-Key", "123abc" },
               { "X-Another-Secret", "sjaaaaaak 123" }
            },
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_SoapXml_ValidateXPath_HappyFlow()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}InStock";
         string body = @"<?xml version=""1.0""?>
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
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("<result>OK</result>", content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/xml", response.Content.Headers.ContentType.ToString());
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_SoapXml_ValidateXPath_StubNotFound()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}InStock";
         string body = @"<?xml version=""1.0""?>
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
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_RegularXml_ValidateXPath_HappyFlow()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}InStock";
         string body = @"<?xml version=""1.0""?>
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
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("<result>OK</result>", content);
            Assert.AreEqual("text/xml", response.Content.Headers.ContentType.ToString());
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPost_RegularXml_ValidateXPath_StubNotFound()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}InStock";
         string body = @"<?xml version=""1.0""?>
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
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(string.IsNullOrEmpty(content));
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPut_Json_ValidateJsonPath_HappyFlow()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}users";
         string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""iPhone"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
         var request = new HttpRequestMessage
         {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.IsTrue(string.IsNullOrEmpty(content));
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularPut_Json_ValidateJsonPath_StubNotFound()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}users";
         string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""Android"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
         var request = new HttpRequestMessage
         {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
            Method = HttpMethod.Put,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(string.IsNullOrEmpty(content));
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularGet_Base64Content_HappyFlow()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}image.jpg";

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            var content = await response.Content.ReadAsByteArrayAsync();
            Assert.AreEqual(75583, content.Length);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task StubIntegration_RegularGet_File_HappyFlow()
      {
         // arrange
         string fileContents = "File contents yo!";
         string url = $"{TestServer.BaseAddress}text.txt";

         _fileServiceMock
            .Setup(m => m.FileExists("text.txt"))
            .Returns(true);
         _fileServiceMock
            .Setup(m => m.ReadAllBytes("text.txt"))
            .Returns(Encoding.UTF8.GetBytes(fileContents));

         // act / assert
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(fileContents, content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         }
      }
   }
}
