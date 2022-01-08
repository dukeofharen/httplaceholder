using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class QueryStringConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_ButNoQueryStringConditions_ShouldReturnNotExecuted()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var conditions = new StubConditionsModel {Url = new StubUrlConditionModel {Query = null}};

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_NoMatchingQueryStringKeys_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "2"}, {"y", "3"}}}
        };
        var query = new Dictionary<string, string> {{"x", "1"}, {"z", "2"}};

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_AllQueryStringsIncorrect_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "2"}, {"y", "3"}}}
        };
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_OneQueryStringValueMissing_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "2"}}}
        };

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_OnlyOneQueryStringCorrect_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "1"}, {"y", "3"}}}
        };

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_HappyFlow_FullText()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}}}
        };
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_StubsFound_HappyFlow_Regex()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}}}
        };

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
