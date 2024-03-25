using System.Linq;
using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetStubsByTenantFacts : BaseClientTest
{
    private const string GetStubsResponse = """
                                            [
                                                {
                                                    "stub": {
                                                        "id": "situation-fallback",
                                                        "conditions": {
                                                            "method": "GET",
                                                            "url": {
                                                                "path": "/users",
                                                                "query": {
                                                                    "id": "^[0-9]+$"
                                                                }
                                                            }
                                                        },
                                                        "response": {
                                                            "statusCode": 404,
                                                            "headers": {
                                                                "Content-Type": "application/json"
                                                            }
                                                        },
                                                        "priority": 0,
                                                        "tenant": "01-get",
                                                        "enabled": true
                                                    },
                                                    "metadata": {
                                                        "readOnly": false
                                                    }
                                                },
                                                {
                                                    "stub": {
                                                        "id": "situation-02",
                                                        "conditions": {
                                                            "method": "GET",
                                                            "url": {
                                                                "path": "/users",
                                                                "query": {
                                                                    "id": "14",
                                                                    "filter": "last_name"
                                                                }
                                                            }
                                                        },
                                                        "response": {
                                                            "statusCode": 200,
                                                            "text": "{\n  \"last_name\": \"Jackson\"\n}\n",
                                                            "headers": {
                                                                "Content-Type": "application/json"
                                                            }
                                                        },
                                                        "priority": 0,
                                                        "tenant": "01-get",
                                                        "enabled": true
                                                    },
                                                    "metadata": {
                                                        "readOnly": false
                                                    }
                                                }
                                            ]
                                            """;

    [TestMethod]
    public async Task GetStubsByTenantAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string tenant = "01-get";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/tenants/{tenant}/stubs")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetStubsByTenantAsync(tenant));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetStubsByTenantAsync_ShouldReturnStubs()
    {
        // Arrange
        const string tenant = "01-get";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/tenants/{tenant}/stubs")
            .Respond("application/json", GetStubsResponse)));

        // Act
        var result = (await client.GetStubsByTenantAsync(tenant)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual("situation-fallback", result[0].Stub.Id);
        Assert.AreEqual("situation-02", result[1].Stub.Id);
    }
}
