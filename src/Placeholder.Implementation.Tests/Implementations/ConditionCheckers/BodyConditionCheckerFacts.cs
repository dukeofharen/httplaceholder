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
   public class BodyConditionCheckerFacts
   {
      private Mock<ILogger<BodyConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private Mock<IStubManager> _stubContainerMock;
      private BodyConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<BodyConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _stubContainerMock = new Mock<IStubManager>();
         _checker = new BodyConditionChecker(
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
      public void BodyConditionChecker_ValidateAsync_NoStubsFound_ShouldReturnNull()
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
      public void BodyConditionChecker_ValidateAsync_StubsFound_ButNoBodyConditions_ShouldReturnNull()
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
                     Body = null
                  }
               }
            });

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_AllBodyConditionsIncorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string body = "this is a test";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Body = new []
                     {
                        @"\bthat\b",
                        @"\btree\b"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_OnlyOneBodyConditionCorrect_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string body = "this is a test";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Body = new []
                     {
                        @"\bthis\b",
                        @"\btree\b"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_HappyFlow_FullText()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string body = "this is a test";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Body = new []
                     {
                        "this is a test"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(1, result.Count());
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string body = "this is a test";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Body = new []
                     {
                        @"\bthis\b",
                        @"\btest\b"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(1, result.Count());
      }
   }
}
