using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class JsonConditionCheckerFacts
{
    private const string PostedObjectJson = """
                                            {
                                              "username": "username",
                                              "subObject": {
                                                "strValue": "stringInput",
                                                "boolValue": true,
                                                "doubleValue": 1.23,
                                                "dateTimeValue": "2021-04-16T21:23:03",
                                                "intValue": 3,
                                                "nullValue": null,
                                                "arrayValue": [
                                                  "val1",
                                                  {
                                                    "subKey1": "subValue1",
                                                    "subKey2": "subValue2"
                                                  }
                                                ]
                                              }
                                            }

                                            """;

    private const string PostedArrayJson = """
                                           [
                                               "val1",
                                               3,
                                               1.46,
                                               "2021-04-17T13:16:54",
                                               {
                                                   "stringVal": "val1",
                                                   "intVal": 55
                                               }
                                           ]
                                           """;

    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_JsonConditionsNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var conditions = new StubConditionsModel { Json = null };

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsObject_ShouldReturnValid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var jsonConditions = CreateObjectStubConditions();
        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(PostedObjectJson);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsObject_BooleanConditionIsRegex_ShouldReturnValid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        var jsonConditions = new Dictionary<object, object> { { "boolValue", "^(true|false)$" } };
        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"boolValue": true}""");

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsObject_JsonIsCorrupt_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        var jsonConditions = CreateObjectStubConditions();
        ((IDictionary<object, object>)jsonConditions["subObject"])["intValue"] = "4";

        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("JSON IS CORRUPT!!!");

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Unexpected character encountered while parsing value: J. Path '', line 0, position 0.",
            result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsObject_JsonIsIncorrect_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        var jsonConditions = CreateObjectStubConditions();
        ((IDictionary<object, object>)jsonConditions["subObject"])["intValue"] = "4";

        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(PostedObjectJson);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsArray_ShouldReturnValid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        var jsonConditions = CreateArrayStubConditions();
        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(PostedArrayJson);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionsArray_JsonIsIncorrect_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();
        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();

        var jsonConditions = CreateArrayStubConditions();
        jsonConditions[1] = "4";

        var conditions = new StubConditionsModel { Json = jsonConditions };

        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(PostedArrayJson);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsNull_ShouldReturnNull()
    {
        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsDictionary_ShouldReturnInputAsIs()
    {
        // Arrange
        var input = new Dictionary<object, object>();

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsList_ShouldReturnInputAsIs()
    {
        // Arrange
        var input = new List<object>();

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsString_ShouldReturnInputAsIs()
    {
        // Arrange
        const string input = "input";

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(input);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsJArray_ShouldConvertCorrectly()
    {
        // Arrange
        var jArray = JArray.Parse("[1, 2, 3]");

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(jArray);

        // Assert
        Assert.AreNotEqual(jArray, result);

        var list = (List<object>)result;
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("1", list[0]);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsJObject_ShouldConvertCorrectly()
    {
        // Arrange
        var jObject = JObject.Parse("""{"key1": "val1", "key2": "val2"}""");

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(jObject);

        // Assert
        Assert.AreNotEqual(jObject, result);

        var dict = (Dictionary<object, object>)result;
        Assert.AreEqual(2, dict.Count);
        Assert.AreEqual("val1", dict["key1"]);
    }

    [TestMethod]
    public void ConvertJsonConditions_InputIsSomethingElse_ShouldConvertToString()
    {
        // Arrange
        const int input = 123;

        // Act
        var result = JsonConditionChecker.ConvertJsonConditions(input);

        // Assert
        Assert.AreNotEqual(input, result);
        Assert.AreEqual("123", result);
    }

    [TestMethod]
    public void CheckSubmittedJson_InputNotSupported_ShouldReturnFalse()
    {
        // Arrange
        var checker = _mocker.CreateInstance<JsonConditionChecker>();

        // Act
        var result = checker.CheckSubmittedJson(123, JToken.Parse("{}"), []);

        // Assert
        Assert.IsFalse(result);
    }

    private static Dictionary<object, object> CreateObjectStubConditions() =>
        new()
        {
            { "username", "^username$" },
            {
                "subObject",
                new Dictionary<object, object>
                {
                    { "strValue", "stringInput" },
                    { "boolValue", "true" },
                    { "doubleValue", "1.23" },
                    { "dateTimeValue", "2021-04-16T21:23:03" },
                    { "intValue", "3" },
                    { "nullValue", null },
                    {
                        "arrayValue",
                        new List<object>
                        {
                            "val1",
                            new Dictionary<object, object>
                            {
                                { "subKey1", "subValue1" }, { "subKey2", "subValue2" }
                            }
                        }
                    }
                }
            }
        };

    private static List<object> CreateArrayStubConditions() =>
    [
        "val1",
        "3",
        "1.46",
        "2021-04-17T13:16:54",
        new Dictionary<object, object> { { "stringVal", "val1" }, { "intVal", "55" } }
    ];
}
