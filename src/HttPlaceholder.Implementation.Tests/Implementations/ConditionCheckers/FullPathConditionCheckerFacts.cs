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
   public class FullPathConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private FullPathConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new FullPathConditionChecker(
            TestObjectFactory.GetRequestLoggerFactory(),
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void FullPathConditionChecker_Validate_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            Url = new StubUrlConditionModel
            {
               FullPath = null
            }
         };

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void FullPathConditionChecker_Validate_StubsFound_WrongPath_ShouldReturnInvalid()
      {
         // arrange
         string path = "/login?success=true";
         var conditions = new StubConditionsModel
         {
            Url = new StubUrlConditionModel
            {
               FullPath = "/login?success=false"
            }
         };

         _httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void FullPathConditionChecker_Validate_StubsFound_HappyFlow_CompleteUrl()
      {
         // arrange
         string path = "/login?success=true";
         var conditions = new StubConditionsModel
         {
            Url = new StubUrlConditionModel
            {
               FullPath = "/login?success=true"
            }
         };

         _httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }

      [TestMethod]
      public void FullPathConditionChecker_Validate_StubsFound_HappyFlow_Regex()
      {
         // arrange
         string path = "/locatieserver/v3/suggest";
         var conditions = new StubConditionsModel
         {
            Url = new StubUrlConditionModel
            {
               FullPath = @"\blocatieserver\/v3\/suggest\b"
            }
         };

         _httpContextServiceMock
            .Setup(m => m.FullPath)
            .Returns(path);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
