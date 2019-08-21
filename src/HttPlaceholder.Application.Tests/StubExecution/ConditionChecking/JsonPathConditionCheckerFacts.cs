using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class JsonPathConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private JsonPathConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _checker = new JsonPathConditionChecker(
               _httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void JsonPathConditionChecker_Validate_StubsFound_ButNoJsonPathConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                JsonPath = null
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void JsonPathConditionChecker_Validate_StubsFound_AllXPathConditionsIncorrect_ShouldReturnInvalid()
        {
            // arrange
            string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""iPhone"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
            var conditions = new StubConditionsModel
            {
                JsonPath = new[]
                  {
                  "$.phoneNumbers[?(@.type=='Android')]",
                  "$.phoneNumbers[?(@.type=='Office')]"
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetBody())
               .Returns(body);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void JsonPathConditionChecker_Validate_StubsFound_OnlyOneJsonPathConditionCorrect_ShouldReturnInvalid()
        {
            // arrange
            string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""iPhone"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
            var conditions = new StubConditionsModel
            {
                JsonPath = new[]
                  {
                  "$.phoneNumbers[?(@.type=='iPhone')]",
                  "$.phoneNumbers[?(@.type=='Office')]"
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetBody())
               .Returns(body);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void JsonPathConditionChecker_Validate_StubsFound_HappyFlow_WithNamespaces()
        {
            // arrange
            string body = @"{
  ""firstName"": ""John"",
  ""lastName"" : ""doe"",
  ""age""      : 26,
  ""address""  : {
    ""streetAddress"": ""naist street"",
    ""city""         : ""Nara"",
    ""postalCode""   : ""630-0192""
  },
  ""phoneNumbers"": [
    {
      ""type""  : ""iPhone"",
      ""number"": ""0123-4567-8888""
    },
    {
      ""type""  : ""home"",
      ""number"": ""0123-4567-8910""
    }
  ]
}";
            var conditions = new StubConditionsModel
            {
                JsonPath = new[]
                  {
                  "$.phoneNumbers[?(@.type=='iPhone')]",
                  "$.phoneNumbers[?(@.type=='home')]"
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetBody())
               .Returns(body);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
