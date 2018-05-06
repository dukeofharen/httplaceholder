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
   public class BodyConditionCheckerFacts
   {
      private Mock<ILogger<BodyConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private BodyConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<BodyConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new BodyConditionChecker(
            _loggerMock.Object,
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_ButNoBodyConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Body = null
            }
         };

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_AllBodyConditionsIncorrect_ShouldReturnInvalid()
      {
         // arrange
         string body = "this is a test";
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Body = new[]
               {
                  @"\bthat\b",
                  @"\btree\b"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_OnlyOneBodyConditionCorrect_ShouldReturnInvalid()
      {
         // arrange
         string body = "this is a test";
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Body = new[]
               {
                  @"\bthis\b",
                  @"\btree\b"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_HappyFlow_FullText()
      {
         // arrange
         string body = "this is a test";
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Body = new[]
               {
                  "this is a test"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }

      [TestMethod]
      public void BodyConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         string body = "this is a test";
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Body = new[]
               {
                  @"\bthis\b",
                  @"\btest\b"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.GetBody())
            .Returns(body);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
