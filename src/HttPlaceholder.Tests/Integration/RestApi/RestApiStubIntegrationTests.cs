using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using FullStubDto = HttPlaceholder.Dto.Stubs.FullStubDto;

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
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, StubSource.StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.StubModels.First().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Client()
        {
            // Arrange
            var request = new StubDto
            {
                Id = "situation-01",
                Conditions = new StubConditionsDto
                {
                    Method = "GET",
                    Url = new StubUrlConditionDto
                    {
                        Path = "/users",
                        Query = new Dictionary<string, string>
                            {
                                { "id", "12" },
                                { "filter", "first_name" }
                            }
                    }
                },
                Response = new StubResponseDto
                {
                    StatusCode = 200,
                    Text = @"{""first_name"": ""John""}",
                    Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json" }
                        }
                }
            };

            // Act
            await GetFactory()
                .StubClient
                .AddAsync(request);

            // Assert
            Assert.AreEqual(1, StubSource.StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.StubModels.First().Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Add_Json_StubIdAlreadyExistsInReadOnlySource_ShouldReturn409()
        {
            // arrange
            var request = new StubDto
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
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory()
                .StubClient
                .AddAsync(request));

            // Assert
            Assert.AreEqual(409, exception.StatusCode);
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
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stubs = JsonConvert.DeserializeObject<IEnumerable<Client.FullStubDto>>(content);
            Assert.AreEqual(1, stubs.Count());
            Assert.AreEqual("test-123", stubs.First().Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_Client()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            var result = await GetFactory()
                .StubClient
                .GetAllAsync();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test-123", result.First().Stub.Id);
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
            using var response = await Client.SendAsync(request);
            Assert.IsTrue(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var stub = JsonConvert.DeserializeObject<Client.FullStubDto>(content);
            Assert.AreEqual("test-123", stub.Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_Client()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            var result = await GetFactory()
                .StubClient
                .GetAsync("test-123");

            // Assert
            Assert.AreEqual("test-123", result.Stub.Id);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Get_StubNotFound_ShouldReturn404()
        {
            // arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-124",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory()
            .StubClient
            .GetAsync("test-123"));

            // Assert
            Assert.AreEqual(404, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_Delete()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            await GetFactory()
                .StubClient
                .DeleteAsync("test-123");

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
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException>(() => GetFactory()
                .StubClient
                .DeleteAsync("test-123"));

            // Assert
            Assert.AreEqual(404, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_CredentialsAreNeededButIncorrect_ShouldReturn401()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var exception = await Assert.ThrowsExceptionAsync<SwaggerException<ProblemDetails>>(() => GetFactory("wrong", "wrong")
                .StubClient
                .GetAllAsync());

            // Assert
            Assert.AreEqual(401, exception.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Stub_GetAll_CredentialsAreCorrect_ShouldContinue()
        {
            // Arrange
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-123",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });

            Settings.Authentication.ApiUsername = "correct";
            Settings.Authentication.ApiPassword = "correct";

            // Act
            var result = await GetFactory("correct", "correct")
                .StubClient
                .GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
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
                NegativeConditions = new StubConditionsModel(),
                Response = new StubResponseModel()
            });
            StubSource.StubModels.Add(new StubModel
            {
                Id = "test-456",
                Conditions = new StubConditionsModel(),
                NegativeConditions = new StubConditionsModel(),
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
