using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.v1.Stubs;
using HttPlaceholder.TestUtilities.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using FullStubDto = HttPlaceholder.Dto.v1.Stubs.FullStubDto;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiStubIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Yaml()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            const string body = @"id: situation-01
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
            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("situation-01"));
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, StubSource.StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.StubModels.Single().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Json()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            const string body = @"{
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
            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("situation-01"));
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, StubSource.StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.StubModels.First().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Json_StubIdAlreadyExistsInReadOnlySource_ShouldReturn409()
        {
            // arrange
            var stub = new StubDto
            {
                Id = "situation-01"
            };

            var existingStub = new StubModel
            {
                Id = "situation-01"
            };
            ReadOnlyStubSource
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[] { existingStub });

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, $"{TestServer.BaseAddress}ph-api/stubs")
            {
                Content = new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, "application/json")
            };
            using var response = await Client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_Yaml()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var reader = new StringReader(content);
            var deserializer = new Deserializer();
            var stubs = deserializer.Deserialize<IEnumerable<FullStubDto>>(reader);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-123", stubs.First().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_Json()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stubs = JsonConvert.DeserializeObject<FullStubDto[]>(content);
            Assert.AreEqual(1, stubs.Length);
            Assert.AreEqual("test-123", stubs.First().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_Yaml()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var reader = new StringReader(content);
            var deserializer = new Deserializer();
            var stub = deserializer.Deserialize<FullStubDto>(reader);
            Assert.AreEqual("test-123", stub.Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_Json()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stub = JsonConvert.DeserializeObject<FullStubDto>(content);
            Assert.AreEqual("test-123", stub.Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_StubNotFound_ShouldReturn404()
        {
            // arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-124",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/stubs/test-123");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Delete()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/stubs/test-123");

            // Assert
            Assert.AreEqual(0, StubSource.StubModels.Count);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Delete_NotFound_ShouldReturn404()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-124",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/stubs/test-123");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/stubs/test-123");
            request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("wrong", "wrong"));
            using var response = await Client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_CredentialsAreCorrect_ShouldContinue()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/stubs/test-123");
            request.Headers.Add("Authorization", HttpUtilities.GetBasicAuthHeaderValue("correct", "correct"));
            using var response = await Client.SendAsync(request);

            // Assert

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_HeadersAreSet()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual("*", response.Headers.Single(h => h.Key == "Access-Control-Allow-Origin").Value.Single());
            Assert.AreEqual("Authorization, Content-Type", response.Headers.Single(h => h.Key == "Access-Control-Allow-Headers").Value.Single());
            Assert.AreEqual("GET, POST, PUT, DELETE, OPTIONS", response.Headers.Single(h => h.Key == "Access-Control-Allow-Methods").Value.Single());
            Assert.AreEqual("no-store, no-cache", response.Headers.Single(h => h.Key == "Cache-Control").Value.Single());
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Options_HeadersAreSet()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Options,
                RequestUri = new Uri(url)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("Origin", "http://localhost:8080");

            // act / assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual("*", response.Headers.Single(h => h.Key == "Access-Control-Allow-Origin").Value.Single());
            Assert.AreEqual("Authorization, Content-Type", response.Headers.Single(h => h.Key == "Access-Control-Allow-Headers").Value.Single());
            Assert.AreEqual("GET, POST, PUT, DELETE, OPTIONS", response.Headers.Single(h => h.Key == "Access-Control-Allow-Methods").Value.Single());
            Assert.AreEqual("no-store, no-cache", response.Headers.Single(h => h.Key == "Cache-Control").Value.Single());
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Then_Update_Yaml()
        {
            // Add
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            var body = @"id: situation-01
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
                Assert.AreEqual(1, StubSource.StubModels.Count);
                Assert.AreEqual("situation-01", StubSource.StubModels.Single().Id);
            }

            // Update
            // Arrange
            url = $"{TestServer.BaseAddress}ph-api/stubs/situation-01";
            body = @"id: NEW-STUB-ID
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
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/x-yaml")
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.AreEqual(1, StubSource.StubModels.Count);
                Assert.AreEqual("NEW-STUB-ID", StubSource.StubModels.Single().Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Then_Update_Json()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            var body = @"{
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
                Assert.AreEqual(1, StubSource.StubModels.Count);
                Assert.AreEqual("situation-01", StubSource.StubModels.First().Id);
            }

            // Update
            // Arrange
            url = $"{TestServer.BaseAddress}ph-api/stubs/situation-01";
            body = @"{
  ""id"": ""NEW-STUB-ID"",
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
            request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.AreEqual(1, StubSource.StubModels.Count);
                Assert.AreEqual("NEW-STUB-ID", StubSource.StubModels.Single().Id);
            }
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_DeleteAll()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            var url = $"{TestServer.BaseAddress}ph-api/stubs";
            var request = new HttpRequestMessage {Method = HttpMethod.Delete, RequestUri = new Uri(url)};

            // Act / Assert
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsFalse(StubSource.StubModels.Any());
        }
    }
}
