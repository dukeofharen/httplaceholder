using HttPlaceholder.DataLogic;
using HttPlaceholder.DataLogic.Implementations.StubSources;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttPlaceholder.Tests.Integration
{
   [TestClass]
   public class GenericIntegrationTests : IntegrationTestBase
   {
      private Dictionary<string, string> _config;
      private InMemoryStubSource _stubSource;
      private Mock<IConfigurationService> _configurationServiceMock;

      [TestInitialize]
      public void Initialize()
      {
         _configurationServiceMock = new Mock<IConfigurationService>();
         _stubSource = new InMemoryStubSource(_configurationServiceMock.Object);
         _config = new Dictionary<string, string>();
         _configurationServiceMock
            .Setup(m => m.GetConfiguration())
            .Returns(_config);

         InitializeIntegrationTest(new Dictionary<Type, object>
         {
            { typeof(IConfigurationService), _configurationServiceMock.Object },
            { typeof(IStubSource), _stubSource }
         });
      }

      [TestCleanup]
      public void Cleanup()
      {
         CleanupIntegrationTest();
      }

      [TestMethod]
      public async Task GenericIntegration_SwaggerUi_IsApproachable()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}swagger/index.html";

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);
         }
      }

      [TestMethod]
      public async Task GenericIntegration_SwaggerJson_IsApproachable()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}swagger/v1/swagger.json";

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);
         }
      }

      [TestMethod]
      public async Task GenericIntegration_Ui_Returns404()
      {
         // Check if a call to the HttPlaceholder returns a, HTTP 404 instead of 500.
         // This way, we know the calls aren't intercepted by the stub logic.

         // arrange
         string url = $"{TestServer.BaseAddress}ph-ui";

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
         }
      }
   }
}
