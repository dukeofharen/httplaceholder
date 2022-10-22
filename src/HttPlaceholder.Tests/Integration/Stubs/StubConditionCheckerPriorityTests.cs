using System.Linq;
using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.Stubs;

/// <summary>
///     Tests whether the "simple" (high priority) condition checkers get executed before the more complex ones.
/// </summary>
[TestClass]
public class StubConditionCheckerPriorityTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task
        StubIntegration_ConditionCheckerPriority_MethodIsIncorrect_ShouldNotExecuteMoreComplexChecker()
    {
        // Arrange
        var url = $"{BaseAddress}url";
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        // Act
        using var response = await Client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);

        Assert.AreEqual(1, Requests.Count);
        var executedRequest = Requests.Single();

        var stubExecution =
            executedRequest.StubExecutionResults.Single(r => r.StubId == "condition-priority-check");
        Assert.AreEqual(1, stubExecution.Conditions.Count());
        var condition = stubExecution.Conditions.Single();
        Assert.AreEqual("MethodConditionChecker", condition.CheckerName);
    }

    [TestMethod]
    public async Task
        StubIntegration_ConditionCheckerPriority_RequestIsCorrect_ShouldAlsoExecuteTheMoreComplexConditionCheckers()
    {
        // Arrange
        var url = $"{BaseAddress}url?query1=val1&query2=val2";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act
        using var response = await Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();

        Assert.AreEqual(1, Requests.Count);
        var executedRequest = Requests.Single();

        var stubExecution =
            executedRequest.StubExecutionResults.Single(r => r.StubId == "condition-priority-check");
        var conditions = stubExecution.Conditions.ToArray();
        Assert.AreEqual(3, conditions.Length);
        Assert.AreEqual("MethodConditionChecker", conditions[0].CheckerName);
        Assert.AreEqual("PathConditionChecker", conditions[1].CheckerName);
        Assert.AreEqual("QueryStringConditionChecker", conditions[2].CheckerName);
    }
}
