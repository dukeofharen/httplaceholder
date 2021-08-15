using System;
using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class JsonPathConditionCheckerFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public void Validate_JsonPathConditionsNull_ShouldReturnDefaultResult()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel {JsonPath = null};

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_JsonPathConditionsEmpty_ShouldReturnDefaultResult()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel {JsonPath = Array.Empty<object>()};

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_OnlyTextBasedConditions_CorrectJson_ShouldPass()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new[] {"$.people[0].firstName", "$.people[0].achievements[?(@.name=='Man of the year')]"}
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_OnlyTextBasedConditions_IncorrectJson_ShouldFail()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new[] {"$.people[0].firstName", "$.people[0].achievements[?(@.name=='Man of the year')]"}
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_TextAndObjectBased_CorrectJson_ShouldPass()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new object[]
                {
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].firstName"},
                        {"expectedValue", "John"}
                    }, "$.people[0].achievements[?(@.name=='Man of the year')]"
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_TextAndObjectBased_IncorrectJson_TextFails_ShouldFail()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new object[]
                {
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].firstName"},
                        {"expectedValue", "John"}
                    }, "$.people[0].achievements[?(@.name=='Man of the year')]"
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_TextAndObjectBased_IncorrectJson_ObjectFails_ShouldFail()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new object[]
                {
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].firstName"},
                        {"expectedValue", "John"}
                    }, "$.people[0].achievements[?(@.name=='Man of the year')]"
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_ObjectBased_CorrectJson_ShouldPass()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new object[]
                {
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].firstName"},
                        {"expectedValue", "John"}
                    },
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].achievements[0].name"},
                        {"expectedValue", "Man of the year"}
                    },
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].age"}
                    }
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_ObjectBased_IncorrectJson_ShouldFail()
        {
            // Arrange
            var checker = _mocker.CreateInstance<JsonPathConditionChecker>();
            var httpMock = _mocker.GetMock<IHttpContextService>();

            var stubId = "stub1";
            var conditions = new StubConditionsModel
            {
                JsonPath = new object[]
                {
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].firstName"},
                        {"expectedValue", "John"}
                    },
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].achievements[0].name"},
                        {"expectedValue", "Man of the year"}
                    },
                    new Dictionary<object, object>
                    {
                        {"query", "$.people[0].age"}
                    }
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
                .Setup(m => m.GetBody())
                .Returns(json);

            // Act
            var result = checker.Validate(stubId, conditions);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }
    }
}
