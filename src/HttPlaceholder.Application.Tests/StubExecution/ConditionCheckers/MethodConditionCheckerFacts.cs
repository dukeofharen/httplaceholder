using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class MethodConditionCheckerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new();
    private MethodConditionChecker _checker;

    [TestInitialize]
    public void Initialize() =>
        _checker = new MethodConditionChecker(
            _httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public void MethodConditionChecker_Validate_StubsFound_ButNoMethodConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            Method = null
        };

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void MethodConditionChecker_Validate_StubsFound_WrongMethod_ShouldReturnInvalid()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            Method = "POST"
        };

        _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void MethodConditionChecker_Validate_StubsFound_HappyFlow()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            Method = "GET"
        };

        _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

        // act
        var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}