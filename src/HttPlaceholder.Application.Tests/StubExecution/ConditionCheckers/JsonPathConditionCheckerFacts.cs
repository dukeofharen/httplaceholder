using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class JsonPathConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_JsonPathConditionsNull_ShouldReturnDefaultResult()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();

        var conditions = new StubConditionsModel { JsonPath = null };

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_JsonPathConditionsEmpty_ShouldReturnDefaultResult()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();

        var conditions = new StubConditionsModel { JsonPath = Array.Empty<object>() };

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_OnlyTextBasedConditions_CorrectJson_ShouldPass()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new[] { "$.people[0].firstName", "$.people[0].achievements[?(@.name=='Man of the year')]" }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""achievements"": [
    {
      ""name"": ""Man of the year""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_OnlyTextBasedConditions_IncorrectJson_ShouldFail()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new[] { "$.people[0].firstName", "$.people[0].achievements[?(@.name=='Man of the year')]" }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""achievements"": [
    {
      ""name"": ""Just an average guy""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_TextAndObjectBased_CorrectJson_ShouldPass()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new object[]
            {
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].firstName" }, { "expectedValue", "John" }
                },
                "$.people[0].achievements[?(@.name=='Man of the year')]"
            }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""achievements"": [
    {
      ""name"": ""Man of the year""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_TextAndObjectBased_IncorrectJson_TextFails_ShouldFail()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new object[]
            {
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].firstName" }, { "expectedValue", "John" }
                },
                "$.people[0].achievements[?(@.name=='Man of the year')]"
            }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""achievements"": [
    {
      ""name"": ""Just an average guy""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_TextAndObjectBased_IncorrectJson_ObjectFails_ShouldFail()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new object[]
            {
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].firstName" }, { "expectedValue", "John" }
                },
                "$.people[0].achievements[?(@.name=='Man of the year')]"
            }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""Marc"",
  ""achievements"": [
    {
      ""name"": ""Man of the year""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ObjectBased_CorrectJson_ShouldPass()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new object[]
            {
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].firstName" }, { "expectedValue", "John" }
                },
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].achievements[0].name" }, { "expectedValue", "Man of the year" }
                },
                new Dictionary<object, object> { { "query", "$.people[0].age" } }
            }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""age"": 29,
  ""achievements"": [
    {
      ""name"": ""Man of the year""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ObjectBased_IncorrectJson_ShouldFail()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
        var httpMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel
        {
            JsonPath = new object[]
            {
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].firstName" }, { "expectedValue", "John" }
                },
                new Dictionary<object, object>
                {
                    { "query", "$.people[0].achievements[0].name" }, { "expectedValue", "Man of the year" }
                },
                new Dictionary<object, object> { { "query", "$.people[0].age" } }
            }
        };

        const string json = @"{""people"": [{
  ""firstName"": ""John"",
  ""age"": 29,
  ""achievements"": [
    {
      ""name"": ""Just an average guy""
    }
  ]
}]}";
        httpMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void ConvertJsonPathCondition_InputIsJObject_ShouldReturnStubJsonPathModelCorrectly()
    {
        // Arrange
        var input = JObject.FromObject(new { query = "jpath query", expectedValue = "expected value" });

        // Act
        var result = JsonPathConditionChecker.ConvertJsonPathCondition("stubId", input);

        // Assert
        Assert.AreEqual("jpath query", result.Query);
        Assert.AreEqual("expected value", result.ExpectedValue);
    }

    [TestMethod]
    public void ConvertJsonPathCondition_InputIsObjectDictionary_ShouldReturnStubJsonPathModelCorrectly()
    {
        // Arrange
        var input = new Dictionary<object, object>
        {
            { "query", "jpath query" }, { "expectedValue", "expected value" }
        };

        // Act
        var result = JsonPathConditionChecker.ConvertJsonPathCondition("stubId", input);

        // Assert
        Assert.AreEqual("jpath query", result.Query);
        Assert.AreEqual("expected value", result.ExpectedValue);
    }

    [TestMethod]
    public void ConvertJsonPathCondition_InputIsStringDictionary_ShouldReturnStubJsonPathModelCorrectly()
    {
        // Arrange
        var input = new Dictionary<string, string>
        {
            { "query", "jpath query" }, { "expectedValue", "expected value" }
        };

        // Act
        var result = JsonPathConditionChecker.ConvertJsonPathCondition("stubId", input);

        // Assert
        Assert.AreEqual("jpath query", result.Query);
        Assert.AreEqual("expected value", result.ExpectedValue);
    }

    [TestMethod]
    public void ConvertJsonPathCondition_InputIsUnknown_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = new object();

        // Act
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            JsonPathConditionChecker.ConvertJsonPathCondition("stubId", input));

        // Assert
        Assert.AreEqual("Can't determine the type of the JSONPath condition for stub with ID 'stubId'.",
            exception.Message);
    }

    [TestMethod]
    public void ConvertJsonPathCondition_QueryIsNotSet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = new Dictionary<object, object> { { "query", string.Empty }, { "expectedValue", "expected value" } };

        // Act
        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            JsonPathConditionChecker.ConvertJsonPathCondition("stubId", input));

        // Assert
        Assert.AreEqual("Value 'query' not set for JSONPath condition for stub with ID 'stubId'.",
            exception.Message);
    }
}
