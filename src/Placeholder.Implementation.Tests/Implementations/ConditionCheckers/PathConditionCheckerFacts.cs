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
   public class PathConditionCheckerFacts
   {
      private Mock<ILogger<PathConditionChecker>> _loggerMock;
      private Mock<IHttpContextService> _httpContextServiceMock;
      private Mock<IStubManager> _stubContainerMock;
      private PathConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _loggerMock = new Mock<ILogger<PathConditionChecker>>();
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _stubContainerMock = new Mock<IStubManager>();
         _checker = new PathConditionChecker(
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
      public void PathConditionChecker_ValidateAsync_NoStubsFound_ShouldReturnNull()
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
      public void PathConditionChecker_ValidateAsync_StubsFound_ButNoPathConditions_ShouldReturnNull()
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
                        Path = null
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
      public void PathConditionChecker_ValidateAsync_StubsFound_WrongPath_ShouldReturnEmptyList()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string path = "/login";
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
                        Path = @"\blocatieserver\/v3\/suggest\b"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = _checker.Validate(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_HappyFlow_CompleteUrl()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string path = "/locatieserver/v3/suggest";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Id = "2",
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Path = @"/locatieserver/v3/suggest"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = (_checker.Validate(stubIds)).ToArray();

         // assert
         Assert.AreEqual(1, result.Length);
         Assert.AreEqual("2", result.Single());
      }

      [TestMethod]
      public void PathConditionChecker_ValidateAsync_StubsFound_HappyFlow_Regex()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         string path = "/locatieserver/v3/suggest";
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Id = "2",
                  Conditions = new StubConditionsModel
                  {
                     Url = new StubUrlConditionModel
                     {
                        Path = @"\blocatieserver\/v3\/suggest\b"
                     }
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

         // act
         var result = (_checker.Validate(stubIds)).ToArray();

         // assert
         Assert.AreEqual(1, result.Length);
         Assert.AreEqual("2", result.Single());
      }
   }
}
