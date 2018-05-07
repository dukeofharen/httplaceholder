using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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
      public async Task StubIntegration_RegularGet_StubNotFound_ShouldReturn500()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}locatieserver/v3/suggest?q=9752EX";

         // act
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

         // act
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

         // act
         using (var response = await Client.GetAsync(url))
         {
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
            JObject.Parse(content);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.ToString());
         }
      }
   }
}
