using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class BasicAuthenticationHandlerFacts
{
    private readonly BasicAuthenticationHandler _handler = new();

    [TestMethod]
    public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_AuthorizationHeaderNotSet_ShouldReturnFalse()
    {
        // Arrange
        var conditions = new StubConditionsModel();
        var request = new HttpRequestModel { Headers = new Dictionary<string, string>() };

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.BasicAuthentication);
    }

    [TestMethod]
    public async Task
        BasicAuthenticationHandler_HandleStubGenerationAsync_AuthorizationWithout2Parts_ShouldReturnFalse()
    {
        // Arrange
        var conditions = new StubConditionsModel();
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string>
            {
                {
                    "Authorization", "Basic " + Convert.ToBase64String("user:pass:rubble"u8.ToArray())
                }
            }
        };

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.BasicAuthentication);
    }

    [TestMethod]
    public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_AuthorizationIsBearer_ShouldReturnFalse()
    {
        // Arrange
        var conditions = new StubConditionsModel();
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string>
            {
                {
                    "Authorization",
                    "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
                }
            }
        };

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.BasicAuthentication);
    }

    [TestMethod]
    public async Task BasicAuthenticationHandler_HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        const string username = "httplaceholder";
        const string password = "secret";
        var auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        var conditions =
            new StubConditionsModel { Headers = new Dictionary<string, object> { { "Authorization", auth } } };
        var request = new HttpRequestModel { Headers = new Dictionary<string, string> { { "Authorization", auth } } };

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(username, conditions.BasicAuthentication.Username);
        Assert.AreEqual(password, conditions.BasicAuthentication.Password);
        Assert.IsFalse(conditions.Headers.Any());
    }
}
