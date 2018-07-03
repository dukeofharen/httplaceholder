using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HttPlaceholder.Implementation.Implementations.ConditionCheckers;
using HttPlaceholder.Implementation.Tests.Utilities;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class MethodConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private MethodConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new MethodConditionChecker(
            TestObjectFactory.GetRequestLoggerFactory(),
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void MethodConditionChecker_Validate_StubsFound_ButNoMethodConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            Method = null
         };

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void MethodConditionChecker_Validate_StubsFound_WrongMethod_ShouldReturnInvalid()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            Method = "POST"
         };

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void MethodConditionChecker_Validate_StubsFound_HappyFlow()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            Method = "GET"
         };

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
