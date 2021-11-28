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
    public class QueryStringConditionCheckerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private QueryStringConditionChecker _checker;

        [TestInitialize]
        public void Initialize() =>
            _checker = new QueryStringConditionChecker(
                _httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

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
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
