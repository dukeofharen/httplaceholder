using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Placeholder.Implementation.Implementations.ConditionCheckers;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class MethodConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private Mock<IStubContainer> _stubContainerMock;
      private MethodConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _stubContainerMock = new Mock<IStubContainer>();
         _checker = new MethodConditionChecker(
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
      public async Task MethodConditionChecker_ValidateAsync_NoStubsFound_ShouldReturnNull()
      {
         // arrange
         var stubIds = new string[0];
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new StubModel[0]);

         // act
         var result = await _checker.ValidateAsync(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public async Task MethodConditionChecker_ValidateAsync_StubsFound_ButNoMethodConditions_ShouldReturnNull()
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
                     Method = null
                  }
               },
               new StubModel
               {
                  Conditions = new StubConditionsModel
                  {
                     Method = null
                  }
               }
            });

         // act
         var result = await _checker.ValidateAsync(stubIds);

         // assert
         Assert.IsNull(result);
      }

      [TestMethod]
      public async Task MethodConditionChecker_ValidateAsync_StubsFound_WrongMethod_ShouldReturnEmptyList()
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
                     Method = "POST"
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = await _checker.ValidateAsync(stubIds);

         // assert
         Assert.AreEqual(0, result.Count());
      }

      [TestMethod]
      public async Task MethodConditionChecker_ValidateAsync_StubsFound_HappyFlow()
      {
         // arrange
         var stubIds = new[] { "1", "2" };
         _stubContainerMock
            .Setup(m => m.GetStubsByIds(stubIds))
            .Returns(new[]
            {
               new StubModel
               {
                  Id = "2",
                  Conditions = new StubConditionsModel
                  {
                     Method = "GET"
                  }
               }
            });

         _httpContextServiceMock
            .Setup(m => m.Method)
            .Returns("GET");

         // act
         var result = (await _checker.ValidateAsync(stubIds)).ToArray();

         // assert
         Assert.AreEqual(1, result.Length);
         Assert.AreEqual("2", result.Single());
      }
   }
}
