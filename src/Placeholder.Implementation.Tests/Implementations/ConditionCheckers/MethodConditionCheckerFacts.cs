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
   public class MethodConditionCheckerFacts
   {
      private Mock<ILogger<MethodConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private MethodConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<MethodConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new MethodConditionChecker(
            _loggerMock.Object,
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void MethodConditionChecker_ValidateAsync_StubsFound_ButNoMethodConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Method = null
            }
         };

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void MethodConditionChecker_ValidateAsync_StubsFound_WrongMethod_ShouldReturnInvalid()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Method = "POST"
            }
         };

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void MethodConditionChecker_ValidateAsync_StubsFound_HappyFlow()
      {
         // arrange
         var stub = new StubModel
         {
            Id = "2",
            Conditions = new StubConditionsModel
            {
               Method = "GET"
            }
         };

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
