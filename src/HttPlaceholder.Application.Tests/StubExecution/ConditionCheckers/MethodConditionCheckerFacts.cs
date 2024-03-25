using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class MethodConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task MethodConditionChecker_ValidateAsync_StubsFound_ButNoMethodConditions_ShouldReturnNotExecuted()
    {
        // Arrange
        var conditions = new StubConditionsModel { Method = null };
        var checker = _mocker.CreateInstance<MethodConditionChecker>();

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task MethodConditionChecker_ValidateAsync_StubsFound_WrongMethod_ShouldReturnInvalid()
    {
        // Arrange
        var conditions = new StubConditionsModel { Method = "POST" };
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var checker = _mocker.CreateInstance<MethodConditionChecker>();

        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task MethodConditionChecker_ValidateAsync_StubsFound_HappyFlow()
    {
        // Arrange
        var conditions = new StubConditionsModel { Method = "GET" };
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var checker = _mocker.CreateInstance<MethodConditionChecker>();

        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [DataTestMethod]
    [DataRow("GET", true)]
    [DataRow("get", true)]
    [DataRow("POST", true)]
    [DataRow("post", true)]
    [DataRow("PUT", false)]
    [DataRow("put", false)]
    public async Task MethodConditionChecker_ValidateAsync_MultipleMethods(string method, bool shouldPass)
    {
        // Arrange
        var conditions = new StubConditionsModel { Method = new List<object> { "GET", "POST" } };
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var checker = _mocker.CreateInstance<MethodConditionChecker>();

        httpContextServiceMock
            .Setup(m => m.Method)
            .Returns(method);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(shouldPass ? ConditionValidationType.Valid : ConditionValidationType.Invalid,
            result.ConditionValidation);
    }
}
