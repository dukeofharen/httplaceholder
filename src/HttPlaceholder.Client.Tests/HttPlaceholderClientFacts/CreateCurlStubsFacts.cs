using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class CreateCurlStubsFacts : BaseClientTest
    {
        private const string CreateCurlStubsResult = @"[
    {
        ""stub"": {
            ""id"": ""generated-7ed80fbc7e90f8a6b40acb2505aff4c7"",
            ""conditions"": {
                ""method"": ""POST"",
                ""url"": {
                    ""path"": ""/api/v1/users/authenticate"",
                    ""query"": {},
                    ""isHttps"": true
                },
                ""body"": [
                    ""{}""
                ],
                ""headers"": {
                    ""User-Agent"": ""Mozilla/5\\.0\\ \\(X11;\\ Ubuntu;\\ Linux\\ x86_64;\\ rv:94\\.0\\)\\ Gecko/20100101\\ Firefox/94\\.0"",
                    ""Accept"": ""application/json,\\ text/plain,\\ \\*/\\*"",
                    ""Accept-Language"": ""en-US,en;q=0\\.5"",
                    ""Accept-Encoding"": ""deflate,\\ gzip,\\ br"",
                    ""Content-Type"": ""application/json;charset=utf-8"",
                    ""Origin"": ""https://site\\.com"",
                    ""Connection"": ""keep-alive"",
                    ""Sec-Fetch-Dest"": ""empty"",
                    ""Sec-Fetch-Mode"": ""cors"",
                    ""Sec-Fetch-Site"": ""same-site"",
                    ""DNT"": ""1"",
                    ""Sec-GPC"": ""1"",
                    ""TE"": ""trailers""
                },
                ""basicAuthentication"": {
                    ""username"": ""duco"",
                    ""password"": ""bladibla""
                },
                ""host"": ""api.site.com""
            },
            ""response"": {
                ""text"": ""OK!"",
                ""headers"": {}
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
        public async Task CreateCurlStubs_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            const string commands = "curl commands;";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Post, $"{BaseUrl}ph-api/import/curl")
                .WithQueryString("doNotCreateStub", "False")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                    client.CreateCurlStubsAsync(commands, false));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task CreateCurlStubs_ShouldCreateStubs(bool doNotCreateStub)
        {
            // Arrange
            const string commands = "curl commands;";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Post, $"{BaseUrl}ph-api/import/curl")
                .WithQueryString("doNotCreateStub", doNotCreateStub.ToString())
                .WithContent(commands)
                .Respond("application/json", CreateCurlStubsResult)));

            // Act
            var result = (await client.CreateCurlStubsAsync(commands, doNotCreateStub)).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("generated-7ed80fbc7e90f8a6b40acb2505aff4c7", result[0].Stub.Id);
            Assert.AreEqual("POST", result[0].Stub.Conditions.Method);
        }
    }
}
