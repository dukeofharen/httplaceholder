using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class CheckFeatureFacts : BaseClientTest
{
    private const string FeatureResponse = @"{
    ""featureFlag"": 0,
    ""enabled"": true
}";

    [TestMethod]
    public async Task CheckFeature_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/metadata/features/authentication")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.CheckFeatureAsync(FeatureFlagType.Authentication));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task CheckFeature_ShouldReturnFeatureEnabledOrNot()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/metadata/features/authentication")
            .Respond("application/json", FeatureResponse)));

        // Act
        var result = await client.CheckFeatureAsync(FeatureFlagType.Authentication);

        // Assert
        Assert.IsTrue(result);
    }
}
