using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class QueryStringConditionCheckerFacts
   {
      private Mock<ILogger<QueryStringConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private QueryStringConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<QueryStringConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new QueryStringConditionChecker(
            _loggerMock.Object,
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_ButNoQueryStringConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = null
               }
            }
         };

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_AllQueryStringsIncorrect_ShouldReturnInvalid()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = new Dictionary<string, string>
                  {
                     {"q", "2"},
                     {"y", "3"}
                  }
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
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_OneQueryStringValueMissing_ShouldReturnInvalid()
      {
         // arrange
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = new Dictionary<string, string>
                  {
                     {"q", "2"}
                  }
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_OnlyOneQueryStringCorrect_ShouldReturnInvalid()
      {
         // arrange
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = new Dictionary<string, string>
                  {
                     {"q", "1"},
                     {"y", "3"}
                  }
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_HappyFlow_FullText()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = new Dictionary<string, string>
                  {
                     {"q", "1"},
                     {"y", "2"}
                  }
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
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Query = new Dictionary<string, string>
                  {
                     {"q", "1"},
                     {"y", "2"}
                  }
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
