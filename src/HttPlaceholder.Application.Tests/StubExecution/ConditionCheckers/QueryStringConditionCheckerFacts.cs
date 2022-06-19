using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Url = new StubUrlConditionModel {Query = new Dictionary<string, object> {{"q", "2"}, {"y", "3"}}}
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
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, object> {{"q", "2"}, {"y", "3"}}}
        };
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("1", "2", out outputForLogging))
            .Returns(false);

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
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, object> {{"q", "2"}}}
        };

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("1", "2", out outputForLogging))
            .Returns(false);

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
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, object> {{"q", "1"}, {"y", "3"}}}
        };

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("1", "1", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("2", "3", out outputForLogging))
            .Returns(false);

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
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel {Query = new Dictionary<string, object> {{"q", "1"}, {"y", "2"}}}
        };
        var query = new Dictionary<string, string> {{"q", "1"}, {"y", "2"}};

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("1", "1", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("2", "2", out outputForLogging))
            .Returns(true);

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_PresentCheck_BothFail_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var query = new Dictionary<string, string> {{"q", "1"}, {"z", "2"}};
        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Query = new Dictionary<string, object>
                {
                    {"q", Convert(new StubConditionStringCheckingModel {Present = false})},
                    {"y", Convert(new StubConditionStringCheckingModel {Present = true})}
                }
            }
        };

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_PresentCheck_OneFails_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var query = new Dictionary<string, string> {{"q", "1"}, {"z", "2"}};
        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Query = new Dictionary<string, object>
                {
                    {"q", Convert(new StubConditionStringCheckingModel {Present = true})},
                    {"y", Convert(new StubConditionStringCheckingModel {Present = true})}
                }
            }
        };

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void QueryStringConditionChecker_Validate_PresentCheck_BothSucceed_ShouldReturnValid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<QueryStringConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var query = new Dictionary<string, string> {{"q", "1"}, {"z", "2"}};
        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Query = new Dictionary<string, object>
                {
                    {"q", Convert(new StubConditionStringCheckingModel {Present = true})},
                    {"y", Convert(new StubConditionStringCheckingModel {Present = false})}
                }
            }
        };

        // Act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    private static JObject Convert(StubConditionStringCheckingModel input)
    {
        var json = JsonConvert.SerializeObject(input);
        return JObject.Parse(json);
    }
}
