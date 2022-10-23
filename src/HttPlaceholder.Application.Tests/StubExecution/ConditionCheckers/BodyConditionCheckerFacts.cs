using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class BodyConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task BodyConditionChecker_ValidateAsync_StubsFound_ButNoBodyConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var conditions = new StubConditionsModel {Body = null};

        // act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task BodyConditionChecker_ValidateAsync_StubsFound_AllBodyConditionsIncorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel {Body = new[] {@"\bthat\b", @"\btree\b"}};

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task BodyConditionChecker_ValidateAsync_StubsFound_OnlyOneBodyConditionCorrect_ShouldReturnInvalid()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Body = new[] {@"\bthis\b", @"\btree\b"}};

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(0), out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(1), out outputForLogging))
            .Returns(false);

        // act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task BodyConditionChecker_ValidateAsync_StubsFound_HappyFlow_FullText()
    {
        // arrange
        const string body = "this is a test";

        var checker = _mocker.CreateInstance<BodyConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel {Body = new[] {"this is a test"}};

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(body, conditions.Body.ElementAt(0), out outputForLogging))
            .Returns(true);

        // act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
