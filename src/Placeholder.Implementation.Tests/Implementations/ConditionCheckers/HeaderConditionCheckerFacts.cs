using System.Collections.Generic;
using Placeholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class HeaderConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private HeaderConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new HeaderConditionChecker(
            TestObjectFactory.GetRequestLoggerFactory(),
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void HeaderConditionCheckerValidateAsync_StubsFound_ButNoQueryStringConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            Headers = null
         };

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
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
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
