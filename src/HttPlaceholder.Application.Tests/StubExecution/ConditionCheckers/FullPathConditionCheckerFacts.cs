using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class FullPathConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task FullPathConditionChecker_ValidateAsync_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FullPathConditionChecker>();

        var conditions = new StubConditionsModel { Url = new StubUrlConditionModel { FullPath = null } };

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task FullPathConditionChecker_ValidateAsync_StubsFound_WrongPath_ShouldReturnInvalid()
    {
        // Arrange
        const string path = "/login?success=true";

        var checker = _mocker.CreateInstance<FullPathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions =
            new StubConditionsModel { Url = new StubUrlConditionModel { FullPath = "/login?success=false" } };

        httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.FullPath, out outputForLogging))
            .Returns(false);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task FullPathConditionChecker_ValidateAsync_StubsFound_HappyFlow_CompleteUrl()
    {
        // Arrange
        const string path = "/login?success=true";

        var checker = _mocker.CreateInstance<FullPathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions =
            new StubConditionsModel { Url = new StubUrlConditionModel { FullPath = "/login?success=true" } };

        httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.FullPath, out outputForLogging))
            .Returns(true);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
