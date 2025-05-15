using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Tests.Integration.RestApi;

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
        const string body = """
                            id: situation-01
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
                                  "first_name": "John"
                                }
                              headers:
                                Content-Type: application/json

                            """;
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
        Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
        Assert.AreEqual("situation-01", StubSource.GetCollection(null).StubModels.Single().Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Add_Json()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        const string body = """
                            {
                              "id": "situation-01",
                              "conditions": {
                                "method": "GET",
                                "url": {
                                  "path": "/users",
                                  "query": {
                                    "id": 12,
                                    "filter": "first_name"
                                  }
                                }
                              },
                              "response": {
                                "statusCode": 200,
                                "text": "{\n  \"\"first_name\"\": \"\"John\"\"\n}\n",
                                "headers": {
                                  "Content-Type": "application/json"
                                }
                              }
                            }
                            """;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("situation-01"));
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
        Assert.AreEqual("situation-01", StubSource.GetCollection(null).StubModels.First().Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_AddMultiple_Json()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs/multiple";
        const string body = """
                            [
                                {
                                    "id": "test-situation1",
                                    "conditions": {
                                        "method": "GET",
                                        "url": {
                                            "path": "/testtesttest1",
                                            "query": {
                                                "id": "13"
                                            }
                                        }
                                    },
                                    "response": {
                                        "statusCode": 200,
                                        "text": "OK my dude! 1"
                                    }
                                },
                                {
                                    "id": "test-situation2",
                                    "conditions": {
                                        "method": "GET",
                                        "url": {
                                            "path": "/testtesttest2",
                                            "query": {
                                                "id": "13"
                                            }
                                        }
                                    },
                                    "response": {
                                        "statusCode": 200,
                                        "text": "OK my dude! 2"
                                    }
                                }
                            ]
                            """;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("test-situation1"));
        Assert.IsTrue(content.Contains("test-situation2"));
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.AreEqual(2, StubSource.GetCollection(null).StubModels.Count);
        Assert.AreEqual("test-situation1", StubSource.GetCollection(null).StubModels.ElementAt(0).Id);
        Assert.AreEqual("test-situation2", StubSource.GetCollection(null).StubModels.ElementAt(1).Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Add_ValidationError_ShouldReturn400()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        const string body = """
                            {
                              "response": {
                                "statusCode": 200,
                                "text": "{\n  \"\"first_name\"\": \"\"John\"\"\n}\n",
                                "headers": {
                                  "Content-Type": "application/json"
                                }
                              }
                            }
                            """;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var errors = JsonConvert.DeserializeObject<string[]>(content);
        Assert.AreEqual(1, errors.Length);
        Assert.AreEqual("The Id field is required.", errors[0]);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Add_Json_StubIdAlreadyExistsInReadOnlySource_ShouldReturn409()
    {
        // arrange
        var stub = new StubDto { Id = "situation-01", Response = new StubResponseDto() };

        var existingStub = new StubModel { Id = "situation-01" };
        ReadOnlyStubSource
            .Setup(m => m.GetStubsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { (existingStub, new Dictionary<string, string>()) });

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestServer.BaseAddress}ph-api/stubs")
        {
            Content = new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, MimeTypes.JsonMime)
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
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-yaml"));

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var reader = new StringReader(content);
        var deserializer = new Deserializer();
        var stubs = deserializer.Deserialize<IEnumerable<FullStubDto>>(reader).ToArray();
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test-123", stubs.First().Stub.Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_GetAll_Json()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.JsonMime));

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var stubs = JsonConvert.DeserializeObject<FullStubDto[]>(content);
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test-123", stubs.First().Stub.Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_GetOverview()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs/overview";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.JsonMime));

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var stubs = JsonConvert.DeserializeObject<FullStubOverviewDto[]>(content);
        Assert.AreEqual(1, stubs.Length);
        Assert.AreEqual("test-123", stubs.First().Stub.Id);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Get_Yaml()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs/test-123";
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
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
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var request = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(url) };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypes.JsonMime));

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
        StubSource.GetCollection(null).StubModels.Add(new StubModel
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
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        // Act
        using var response = await Client.DeleteAsync($"{TestServer.BaseAddress}ph-api/stubs/test-123");

        // Assert
        Assert.AreEqual(0, StubSource.GetCollection(null).StubModels.Count);
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Delete_NotFound_ShouldReturn404()
    {
        // Arrange
        StubSource.GetCollection(null).StubModels.Add(new StubModel
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
    public async Task RestApiIntegration_Stub_Add_Then_Update_Yaml()
    {
        // Add
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        var body = """
                   id: situation-01
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
                         "first_name": "John"
                       }
                     headers:
                       Content-Type: application/json

                   """;
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
            Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.GetCollection(null).StubModels.Single().Id);
        }

        // Update
        // Arrange
        url = $"{TestServer.BaseAddress}ph-api/stubs/situation-01";
        body = """
               id: NEW-STUB-ID
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
                     "first_name": "John"
                   }
                 headers:
                   Content-Type: application/json

               """;
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
            Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
            Assert.AreEqual("NEW-STUB-ID", StubSource.GetCollection(null).StubModels.Single().Id);
        }
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_Add_Then_Update_Json()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        var body = """
                   {
                     "id": "situation-01",
                     "conditions": {
                       "method": "GET",
                       "url": {
                         "path": "/users",
                         "query": {
                           "id": 12,
                           "filter": "first_name"
                         }
                       }
                     },
                     "response": {
                       "statusCode": 200,
                       "text": "{\n  \"\"first_name\"\": \"\"John\"\"\n}\n",
                       "headers": {
                         "Content-Type": "application/json"
                       }
                     }
                   }
                   """;
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime)
        };

        // act / assert
        using (var response = await Client.SendAsync(request))
        {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
            Assert.AreEqual("situation-01", StubSource.GetCollection(null).StubModels.First().Id);
        }

        // Update
        // Arrange
        url = $"{TestServer.BaseAddress}ph-api/stubs/situation-01";
        body = """
               {
                 "id": "NEW-STUB-ID",
                 "conditions": {
                   "method": "GET",
                   "url": {
                     "path": "/users",
                     "query": {
                       "id": 12,
                       "filter": "first_name"
                     }
                   }
                 },
                 "response": {
                   "statusCode": 200,
                   "text": "{\n  \"\"first_name\"\": \"\"John\"\"\n}\n",
                   "headers": {
                     "Content-Type": "application/json"
                   }
                 }
               }
               """;
        request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, MimeTypes.JsonMime)
        };

        // act / assert
        using (var response = await Client.SendAsync(request))
        {
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(1, StubSource.GetCollection(null).StubModels.Count);
            Assert.AreEqual("NEW-STUB-ID", StubSource.GetCollection(null).StubModels.Single().Id);
        }
    }

    [TestMethod]
    public async Task RestApiIntegration_Stub_DeleteAll()
    {
        // Arrange
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-123",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });
        StubSource.GetCollection(null).StubModels.Add(new StubModel
        {
            Id = "test-456",
            Conditions = new StubConditionsModel(),
            Response = new StubResponseModel()
        });

        var url = $"{TestServer.BaseAddress}ph-api/stubs";
        var request = new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = new Uri(url) };

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.IsTrue(response.IsSuccessStatusCode);
        Assert.IsFalse(StubSource.GetCollection(null).StubModels.Any());
    }
}
