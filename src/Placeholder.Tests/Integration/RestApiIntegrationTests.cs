using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Placeholder.DataLogic;
using Placeholder.DataLogic.Implementations.StubSources;
using Placeholder.Models;
using Placeholder.Services;
using YamlDotNet.Serialization;

namespace Placeholder.Tests.Integration
{
   [TestClass]
   public class RestApiIntegrationTests : IntegrationTestBase
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
      public async Task RestApiIntegration_Stub_Add_Yaml()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         string body = @"id: situation-01
conditions:
  method: GET
  url:
    path: /users
    query:
      id: 12
      filter: first_name
response:
  statusCode: 200
  text: |
    {
      ""first_name"": ""John""
    }
  headers:
    Content-Type: application/json
";
         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, "application/x-yaml")
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, _stubSource._stubModels.Count);
            Assert.AreEqual("situation-01", _stubSource._stubModels.First().Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Add_Json()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         string body = @"{
  ""id"": ""situation-01"",
  ""conditions"": {
    ""method"": ""GET"",
    ""url"": {
      ""path"": ""/users"",
      ""query"": {
        ""id"": 12,
        ""filter"": ""first_name""
      }
    }
  },
  ""response"": {
    ""statusCode"": 200,
    ""text"": ""{\n  \""\""first_name\""\"": \""\""John\""\""\n}\n"",
    ""headers"": {
      ""Content-Type"": ""application/json""
    }
  }
}";
         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, "application/json")
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, _stubSource._stubModels.Count);
            Assert.AreEqual("situation-01", _stubSource._stubModels.First().Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_GetAll_Yaml()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var reader = new StringReader(content);
            var deserializer = new Deserializer();
            var stubs = deserializer.Deserialize<IEnumerable<StubModel>>(reader);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-123", stubs.First().Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_GetAll_Json()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var stubs = JsonConvert.DeserializeObject<IEnumerable<StubModel>>(content);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-123", stubs.First().Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Get_Yaml()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var reader = new StringReader(content);
            var deserializer = new Deserializer();
            var stub = deserializer.Deserialize<StubModel>(reader);
            Assert.AreEqual("test-123", stub.Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Get_Json()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var stub = JsonConvert.DeserializeObject<StubModel>(content);
            Assert.AreEqual("test-123", stub.Id);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Get_StubNotFound_ShouldReturn404()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-124",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Delete()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(0, _stubSource._stubModels.Count);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_Delete_NotFound_ShouldReturn404()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-124",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));
         request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("wrong:wrong"))}");

         _config.Add("apiUsername", "correct");
         _config.Add("apiPassword", "correct");

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Stub_GetAll_CredentialsAreCorrect_ShouldContinue()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/stubs";
         _stubSource._stubModels.Add(new StubModel
         {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            NegativeConditions = new StubConditionsModel(),
            Response = new StubResponseModel()
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };
         request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));
         request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

         _config.Add("apiUsername", "correct");
         _config.Add("apiPassword", "correct");

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Request_GetAll()
      {
         // arrange
         string correlation = Guid.NewGuid().ToString();
         string url = $"{TestServer.BaseAddress}ph-api/requests";
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            CorrelationId = correlation
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var requests = JsonConvert.DeserializeObject<IEnumerable<RequestResultModel>>(content);
            Assert.AreEqual(1, requests.Count());
            Assert.AreEqual(correlation, requests.First().CorrelationId);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Request_GetByStubId()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub2"
         });
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub1"
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.IsTrue(response.IsSuccessStatusCode);

            string content = await response.Content.ReadAsStringAsync();
            var requests = JsonConvert.DeserializeObject<IEnumerable<RequestResultModel>>(content);
            Assert.AreEqual(1, requests.Count());
            Assert.AreEqual("stub1", requests.First().ExecutingStubId);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Request_CredentialsAreNeededButIncorrect_ShouldReturn401()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub2"
         });
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub1"
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("wrong:wrong"))}");

         _config.Add("apiUsername", "correct");
         _config.Add("apiPassword", "correct");

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_Request_CredentialsAreCorrect_ShouldContinue()
      {
         // arrange
         string url = $"{TestServer.BaseAddress}ph-api/requests/stub1";
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub2"
         });
         _stubSource._requestResultModels.Add(new RequestResultModel
         {
            ExecutingStubId = "stub1"
         });

         var request = new HttpRequestMessage
         {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url)
         };

         request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes("correct:correct"))}");

         _config.Add("apiUsername", "correct");
         _config.Add("apiPassword", "correct");

         // act / assert
         using (var response = await Client.SendAsync(request))
         {
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
         }
      }

      [TestMethod]
      public async Task RestApiIntegration_SwaggerUi_IsApproachable()
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
      public async Task RestApiIntegration_SwaggerJson_IsApproachable()
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
   }
}
