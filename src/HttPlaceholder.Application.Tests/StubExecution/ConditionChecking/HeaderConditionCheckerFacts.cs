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
    public class HeaderConditionCheckerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private HeaderConditionChecker _checker;

        [TestInitialize]
        public void Initialize() =>
            _checker = new HeaderConditionChecker(
                _httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void HeaderConditionCheckerValidateAsync_StubsFound_ButNoQueryStringConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Headers = null
            };

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void HeaderConditionCheckerValidateAsync_StubsFound_AllHeadersIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "2"},
                  {"X-Another-Secret", "3"}
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetHeaders())
               .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void HeaderConditionCheckerValidateAsync_StubsFound_OneHeaderValueMissing_ShouldReturnInvalid()
        {
            // arrange
            var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "2"}
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetHeaders())
               .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void HeaderConditionCheckerValidateAsync_StubsFound_OnlyOneHeaderCorrect_ShouldReturnInvalid()
        {
            // arrange
            var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
            var conditions = new StubConditionsModel
            {
                Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "1"},
                  {"X-Another-Secret", "3"}
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetHeaders())
               .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void HeaderConditionCheckerValidateAsync_StubsFound_HappyFlow()
        {
            // arrange
            var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "123abc" },
            { "X-Another-Secret", "blaaaaah 123" }
         };
            var conditions = new StubConditionsModel
            {
                Headers = new Dictionary<string, string>
               {
                  {"X-Api-Key", "123abc"},
                  {"X-Another-Secret", @"\bblaaaaah\b"}
               }
            };

            _httpContextServiceMock
               .Setup(m => m.GetHeaders())
               .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
