using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class HeaderConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void Validate_StubsFound_ButNoHeaderConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel {Headers = null};
        var checker = _mocker.CreateInstance<HeaderConditionChecker>();

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void Validate_AllConditionsFail_ShouldReturnInvalid()
    {
        // Arrange
        var conditions = new StubConditionsModel
        {
            Headers = new Dictionary<string, object> {{"Header-1", "Value-1"}, {"Header-2", "Value-2"}}
        };

        var headers = new Dictionary<string, string> {{"Header-1", "bla1"}, {"Header-2", "bla2"}};

        var checker = _mocker.CreateInstance<HeaderConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("bla1", "Value-1", out outputForLogging))
            .Returns(false);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void Validate_1ConditionFails_ShouldReturnInvalid()
    {
        // Arrange
        var conditions = new StubConditionsModel
        {
            Headers = new Dictionary<string, object> {{"Header-1", "Value-1"}, {"Header-2", "Value-2"}}
        };

        var headers = new Dictionary<string, string> {{"Header-1", "bla1"}, {"Header-2", "bla2"}};

        var checker = _mocker.CreateInstance<HeaderConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("bla1", "Value-1", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("bla2", "Value-2", out outputForLogging))
            .Returns(false);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void Validate_AllConditionsSucceed_ShouldReturnValid()
    {
        // Arrange
        var conditions = new StubConditionsModel
        {
            Headers = new Dictionary<string, object> {{"Header-1", "Value-1"}, {"Header-2", "Value-2"}}
        };

        var headers = new Dictionary<string, string> {{"Header-1", "bla1"}, {"Header-2", "bla2"}};

        var checker = _mocker.CreateInstance<HeaderConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("bla1", "Value-1", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("bla2", "Value-2", out outputForLogging))
            .Returns(true);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    public static IEnumerable<object[]> GetPresentData()
    {
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                {"Header-1", TestObjectFactory.CreateStringCheckingModel(true)},
                {"Header-2", TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new Dictionary<string, string> {{"Header-1", "someval"}}, true
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                {"Header-1", TestObjectFactory.CreateStringCheckingModel(true)},
                {"Header-2", TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new Dictionary<string, string> {{"header-1", "someval"}}, true
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                {"Header-1", TestObjectFactory.CreateStringCheckingModel(true)},
                {"Header-2", TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new Dictionary<string, string> {{"Header-1", "someval"}, {"Header-2", "someval"}}, false
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                {"Header-1", TestObjectFactory.CreateStringCheckingModel(true)},
                {"Header-2", TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new Dictionary<string, string> {{"header-1", "someval"}, {"header-2", "someval"}}, false
        };
        yield return new object[]
        {
            new Dictionary<string, object>
            {
                {"Header-1", TestObjectFactory.CreateStringCheckingModel(true)},
                {"Header-2", TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new Dictionary<string, string> {{"header-1", "someval"}, {"header-5", "someval"}}, true
        };
    }

    [DataTestMethod]
    [DynamicData(nameof(GetPresentData), DynamicDataSourceType.Method)]
    public void Validate_Present(
        Dictionary<string, object> headerConditions,
        Dictionary<string, string> headers,
        bool shouldSucceed)
    {
        // Arrange
        var checker = _mocker.CreateInstance<HeaderConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        // Act
        var result =
            checker.Validate(new StubModel {Conditions = new StubConditionsModel {Headers = headerConditions}});

        // Assert
        Assert.AreEqual(shouldSucceed ? ConditionValidationType.Valid : ConditionValidationType.Invalid, result.ConditionValidation);
    }
}
