using System.Collections;
using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class JsonConditionCheckerFacts
    {
        private const string PostedObjectJson = @"{
  ""username"": ""username"",
  ""subObject"": {
    ""strValue"": ""stringInput"",
    ""boolValue"": true,
    ""doubleValue"": 1.23,
    ""dateTimeValue"": ""2021-04-16T21:23:03"",
    ""intValue"": 3,
    ""nullValue"": null,
    ""arrayValue"": [
      ""val1"",
      {
        ""subKey1"": ""subValue1"",
        ""subKey2"": ""subValue2""
      }
    ]
  }
}
";

        private const string PostedArrayJson = @"[
    ""val1"",
    3,
    1.46,
    ""2021-04-17T13:16:54"",
    {
        ""stringVal"": ""val1"",
        ""intVal"": 55
    }
]";

        private readonly Mock<IHttpContextService> _mockHttpContextService = new();
        private JsonConditionChecker _checker;

        [TestInitialize]
        public void Initialize() => _checker = new JsonConditionChecker(_mockHttpContextService.Object);

        [TestCleanup]
        public void Cleanup() => _mockHttpContextService.VerifyAll();

        [TestMethod]
        public void Validate_JsonConditionsNotSet_ShouldReturnNotExecuted()
        {
            // Arrange
            var conditions = new StubConditionsModel {Json = null};

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_ConditionsObject_ShouldReturnValid()
        {
            // Arrange
            var jsonConditions = CreateObjectStubConditions();
            var conditions = new StubConditionsModel {Json = jsonConditions};

            _mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns(PostedObjectJson);

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
        }

        [TestMethod]
        public void Validate_ConditionsObject_JsonIsCorrupt_ShouldReturnInvalid()
        {
            // Arrange
            var jsonConditions = CreateObjectStubConditions();
            ((IDictionary<object, object>)jsonConditions["subObject"])["intValue"] = "4";

            var conditions = new StubConditionsModel {Json = jsonConditions};

            _mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns("JSON IS CORRUPT!!!");

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
            Assert.AreEqual("Unexpected character encountered while parsing value: J. Path '', line 0, position 0.", result.Log);
        }

        [TestMethod]
        public void Validate_ConditionsObject_JsonIsIncorrect_ShouldReturnInvalid()
        {
            // Arrange
            var jsonConditions = CreateObjectStubConditions();
            ((IDictionary<object, object>)jsonConditions["subObject"])["intValue"] = "4";

            var conditions = new StubConditionsModel {Json = jsonConditions};

            _mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns(PostedObjectJson);

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_ConditionsArray_ShouldReturnValid()
        {
            // Arrange
            var jsonConditions = CreateArrayStubConditions();
            var conditions = new StubConditionsModel {Json = jsonConditions};

            _mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns(PostedArrayJson);

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
        }

        [TestMethod]
        public void Validate_ConditionsArray_JsonIsIncorrect_ShouldReturnInvalid()
        {
            // Arrange
            var jsonConditions = CreateArrayStubConditions();
            jsonConditions[1] = "4";

            var conditions = new StubConditionsModel {Json = jsonConditions};

            _mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns(PostedArrayJson);

            // Act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        private static IDictionary<object, object> CreateObjectStubConditions() =>
            new Dictionary<object, object>
            {
                {"username", "^username$"},
                {
                    "subObject",
                    new Dictionary<object, object>
                    {
                        {"strValue", "stringInput"},
                        {"boolValue", "true"},
                        {"doubleValue", "1.23"},
                        {"dateTimeValue", "2021-04-16T21:23:03"},
                        {"intValue", "3"},
                        {"nullValue", null},
                        {
                            "arrayValue",
                            new List<object>
                            {
                                "val1",
                                new Dictionary<object, object>
                                {
                                    {"subKey1", "subValue1"}, {"subKey2", "subValue2"}
                                }
                            }
                        }
                    }
                }
            };

        private static IList<object> CreateArrayStubConditions() =>
            new List<object>
            {
                "val1",
                "3",
                "1.46",
                "2021-04-17T13:16:54",
                new Dictionary<object, object> {{"stringVal", "val1"}, {"intVal", "55"}}
            };
    }
}
