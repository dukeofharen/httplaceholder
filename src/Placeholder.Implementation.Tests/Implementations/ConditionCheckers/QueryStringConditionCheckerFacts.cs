using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class QueryStringConditionCheckerFacts
   {
      private Mock<ILogger<QueryStringConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private Mock<IStubManager> _stubContainerMock;
      private QueryStringConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<QueryStringConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _stubContainerMock = new Mock<IStubManager>();
         _checker = new QueryStringConditionChecker(
            _loggerMock.Object,
            _httpContextServiceMock.Object,
            _stubContainerMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
         _stubContainerMock.VerifyAll();
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_NoStubsFound_ShouldReturnNull()
      {
         // arrange
         var stubIds = new string[0];
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new StubModel[0]);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_ButNoQueryStringConditions_ShouldReturnNull()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = null
                     }
                  }
               }
            });

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_AllQueryStringsIncorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = new Dictionary<string, string>
                        {
                           { "q", "2" },
                           { "y", "3" }
                        }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_OneQueryStringValueMissing_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = new Dictionary<string, string>
                        {
                           { "q", "2" }
                        }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_OnlyOneQueryStringCorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = new Dictionary<string, string>
                        {
                           { "q", "1" },
                           { "y", "3" }
                        }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_HappyFlow_FullText()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = new Dictionary<string, string>
                        {
                           { "q", "1" },
                           { "y", "2" }
                        }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(1, result.Count());
      }

      [TestMethod]
      public void QueryStringConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var query = new Dictionary<string, string>
         {
            { "q", "1" },
            { "y", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Query = new Dictionary<string, string>
                        {
                           { "q", @"[0-9]" },
                           { "y", @"[0-9]" }
                        }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(query);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(1, result.Count());
      }
   }
}
