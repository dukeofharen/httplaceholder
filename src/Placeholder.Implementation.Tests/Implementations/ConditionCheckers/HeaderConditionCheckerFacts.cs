using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class HeaderConditionCheckerFacts
   {
      private Mock<ILogger<HeaderConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private Mock<IStubManager> _stubContainerMock;
      private HeaderConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<HeaderConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _stubContainerMock = new Mock<IStubManager>();
         _checker = new HeaderConditionChecker(
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
      public void HeaderConditionCheckerValidateAsync_NoStubsFound_ShouldReturnNull()
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
      public void HeaderConditionCheckerValidateAsync_StubsFound_ButNoQueryStringConditions_ShouldReturnNull()
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
                     Headers = null
                  }
               }
            });

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public void HeaderConditionCheckerValidateAsync_StubsFound_AllHeadersIncorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Headers = new Dictionary<string, string>
                     {
                        { "X-Api-Key", "2" },
                        { "X-Another-Secret", "3" }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void HeaderConditionCheckerValidateAsync_StubsFound_OneHeaderValueMissing_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Headers = new Dictionary<string, string>
                     {
                        { "X-Api-Key", "2" }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void HeaderConditionCheckerValidateAsync_StubsFound_OnlyOneHeaderCorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" },
            { "X-Another-Secret", "2" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Headers = new Dictionary<string, string>
                     {
                        { "X-Api-Key", "1" },
                        { "X-Another-Secret", "3" }
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void HeaderConditionCheckerValidateAsync_StubsFound_HappyFlow()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "123abc" },
            { "X-Another-Secret", "blaaaaah 123" }
         };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                    Headers = new Dictionary<string, string>
                    {
                       { "X-Api-Key", "123abc" },
                       { "X-Another-Secret", @"\bblaaaaah\b" }
                    }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(1, result.Count());
      }
   }
}
