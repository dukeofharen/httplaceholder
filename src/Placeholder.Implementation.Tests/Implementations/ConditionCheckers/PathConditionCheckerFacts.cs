using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Implementation.Tests.Utilities;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class PathConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private PathConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new PathConditionChecker(
            TestObjectFactory.GetRequestLoggerFactory(),
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
      {
         // arrange
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Path = null
               }
            }
         };

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_WrongPath_ShouldReturnInvalid()
      {
         // arrange
         string path = "/login";
         var stub = new StubModel
         {
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Path = @"\blocatieserver\/v3\/suggest\b"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_HappyFlow_CompleteUrl()
      {
         // arrange
         string path = "/locatieserver/v3/suggest";
         var stub = new StubModel
         {
            Id = "2",
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Path = @"/locatieserver/v3/suggest"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         string path = "/locatieserver/v3/suggest";
         var stub = new StubModel
         {
            Id = "2",
            Conditions = new StubConditionsModel
            {
               Url = new StubUrlConditionModel
               {
                  Path = @"\blocatieserver\/v3\/suggest\b"
               }
            }
         };

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = _checker.Validate(stub);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
