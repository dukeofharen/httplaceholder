using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class CreateStubsFacts : BaseClientTest
    {
        private const string CreateStubResponse = @"[
    {
        ""stub"": {
            ""id"": ""test-situation1"",
            ""conditions"": {
                ""method"": ""GET"",
                ""url"": {
                    ""path"": ""/testtesttest1"",
                    ""query"": {
                        ""id"": ""13""
                    }
                }
            },
            ""response"": {
                ""statusCode"": 200,
                ""text"": ""OK my dude! 1""
            },
            ""priority"": 0,
            ""enabled"": true
        },
        ""metadata"": {
            ""readOnly"": false
        }
    },
    {
        ""stub"": {
            ""id"": ""test-situation2"",
            ""conditions"": {
                ""method"": ""GET"",
                ""url"": {
                    ""path"": ""/testtesttest2"",
                    ""query"": {
                        ""id"": ""13""
                    }
                }
            },
            ""response"": {
                ""statusCode"": 200,
                ""text"": ""OK my dude! 2""
            },
            ""priority"": 0,
            ""enabled"": true
        },
        ""metadata"": {
            ""readOnly"": false
        }
    }
]";

        [TestMethod]
        public async Task CreateStubsAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs/multiple")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.CreateStubsAsync(new StubDto()));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task CreateStubsAsync_ShouldCreateStub()
        {
            // Arrange
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs/multiple")
                .WithPartialContent("test-situation1")
                .WithPartialContent("test-situation2")
                .Respond("application/json", CreateStubResponse)));
            var input = new[]
            {
                new StubDto
                {
                    Id = "test-situation1",
                    Tenant = "01-get",
                    Conditions = new StubConditionsDto
                    {
                        Method = "GET",
                        Url = new StubUrlConditionDto
                        {
                            Path = "/testtesttest",
                            Query = new Dictionary<string, string> {{"id", "13"}}
                        }
                    },
                    Response = new StubResponseDto {StatusCode = 200, Text = "OK my dude!"}
                },
                new StubDto
                {
                    Id = "test-situation2",
                    Tenant = "01-get",
                    Conditions = new StubConditionsDto
                    {
                        Method = "GET",
                        Url = new StubUrlConditionDto
                        {
                            Path = "/testtesttest",
                            Query = new Dictionary<string, string> {{"id", "13"}}
                        }
                    },
                    Response = new StubResponseDto {StatusCode = 200, Text = "OK my dude!"}
                }
            };

            // Act
            var result = (await client.CreateStubsAsync(input)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("test-situation1", result[0].Stub.Id);
            Assert.AreEqual("test-situation2", result[1].Stub.Id);
        }

        [TestMethod]
        public async Task CreateStubsAsync_Builder_ShouldCreateStub()
        {
            // Arrange
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Post, $"{BaseUrl}ph-api/stubs/multiple")
                .WithPartialContent("test-situation1")
                .WithPartialContent("test-situation2")
                .Respond("application/json", CreateStubResponse)));
            var input = new[]
            {
                StubBuilder.Begin().WithId("test-situation1"), StubBuilder.Begin().WithId("test-situation2")
            };

            // Act
            var result = (await client.CreateStubsAsync(input)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("test-situation1", result[0].Stub.Id);
            Assert.AreEqual("test-situation2", result[1].Stub.Id);
        }
    }
}
