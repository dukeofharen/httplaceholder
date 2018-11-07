using System.Collections.Generic;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ConditionCheckers
{
    [TestClass]
    public class QueryStringConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock;
        private QueryStringConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _checker = new QueryStringConditionChecker(
               _httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_ButNoQueryStringConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = null
                }
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_AllQueryStringsIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = new Dictionary<string, string>
                  {
                     {"q", "2"},
                     {"y", "3"}
                  }
                }
            };
            var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };

            _httpContextServiceMock
               .Setup(m => m.GetQueryStringDictionary())
               .Returns(query);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_OneQueryStringValueMissing_ShouldReturnInvalid()
        {
            // arrange
            var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = new Dictionary<string, string>
               {
                  {"q", "2"}
               }
                }
            };

            _httpContextServiceMock
               .Setup(m => m.GetQueryStringDictionary())
               .Returns(query);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_OnlyOneQueryStringCorrect_ShouldReturnInvalid()
        {
            // arrange
            var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = new Dictionary<string, string>
               {
                  {"q", "1"},
                  {"y", "3"}
               }
                }
            };

            _httpContextServiceMock
               .Setup(m => m.GetQueryStringDictionary())
               .Returns(query);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_HappyFlow_FullText()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = new Dictionary<string, string>
               {
                  {"q", "1"},
                  {"y", "2"}
               }
                }
            };
            var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };

            _httpContextServiceMock
               .Setup(m => m.GetQueryStringDictionary())
               .Returns(query);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void QueryStringConditionChecker_Validate_StubsFound_HappyFlow_Regex()
        {
            // arrange
            var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Query = new Dictionary<string, string>
                  {
                     {"q", "1"},
                     {"y", "2"}
                  }
                }
            };

            _httpContextServiceMock
               .Setup(m => m.GetQueryStringDictionary())
               .Returns(query);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}