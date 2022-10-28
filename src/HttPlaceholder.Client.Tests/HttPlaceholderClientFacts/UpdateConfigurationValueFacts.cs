using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Configuration;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class UpdateConfigurationValueFacts : BaseClientTest
{
    [TestMethod]
    public async Task UpdateConfigurationValueAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Patch, $"{BaseUrl}ph-api/configuration")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.UpdateConfigurationValueAsync(
                    new UpdateConfigurationValueInputDto {ConfigurationKey = "storeResponses", NewValue = "false"}));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task UpdateConfigurationValueAsync_ShouldUpdateConfiguration()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Patch, $"{BaseUrl}ph-api/configuration")
            .WithPartialContent("storeResponses")
            .WithPartialContent("false")
            .Respond(HttpStatusCode.NoContent)));

        // Act / Assert
        await client.UpdateConfigurationValueAsync(
            new UpdateConfigurationValueInputDto {ConfigurationKey = "storeResponses", NewValue = "false"});
    }
}
