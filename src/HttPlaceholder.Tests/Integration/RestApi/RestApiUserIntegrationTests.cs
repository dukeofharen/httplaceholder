using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiUserIntegrationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameDoesntMatch_ShouldReturn403()
    {
        // Arrange
        Settings.Authentication.ApiUsername = "correct";
        Settings.Authentication.ApiPassword = "correct";

        // Act
        var request =
            new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/users/wrong");
        request.Headers.Add(HeaderKeys.Authorization, HttpUtilities.GetBasicAuthHeaderValue("correct", "correct"));
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task RestApiIntegration_User_Get_CredentialsAreCorrect_UsernameMatches_ShouldReturn200()
    {
        // Arrange
        Settings.Authentication.ApiUsername = "correct";
        Settings.Authentication.ApiPassword = "correct";

        // Act
        var request =
            new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/users/correct");
        request.Headers.Add(HeaderKeys.Authorization, HttpUtilities.GetBasicAuthHeaderValue("correct", "correct"));
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task RestApiIntegration_User_Get_NoAuthenticationConfigured_ShouldReturn200()
    {
        // Act
        var request =
            new HttpRequestMessage(HttpMethod.Get, $"{TestServer.BaseAddress}ph-api/users/correct");
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
