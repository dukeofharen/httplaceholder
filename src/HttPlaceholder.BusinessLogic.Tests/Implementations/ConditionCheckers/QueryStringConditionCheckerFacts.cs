using System.Collections.Generic;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.BusinessLogic.Tests.Utilities;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

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
            TestObjectFactory.GetRequestLoggerFactory(),
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
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
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
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         Assert.AreEqual(ConditionValidationType.Invalid, result);
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
         Assert.AreEqual(ConditionValidationType.Valid, result);
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
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
