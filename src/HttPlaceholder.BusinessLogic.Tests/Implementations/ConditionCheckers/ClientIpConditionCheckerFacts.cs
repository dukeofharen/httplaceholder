using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ConditionCheckers
{
   [TestClass]
   public class ClientIpConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private ClientIpConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new ClientIpConditionChecker(_httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void ClientIpConditionChecker_Validate_ConditionNotSet_ShouldReturnNotExecuted()
      {
         // arrange
         string stubId = "stub1";
         var conditions = new StubConditionsModel
         {
            ClientIp = null
         };

         // act
         var result = _checker.Validate(stubId, conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
      }

      [TestMethod]
      public void ClientIpConditionChecker_Validate_SingleIp_NotEqual_ShouldReturnInvalid()
      {
         // arrange
         string stubId = "stub1";
         string clientIp = "127.0.0.1";
         var conditions = new StubConditionsModel
         {
            ClientIp = "127.0.0.2"
         };

         _httpContextServiceMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

         // act
         var result = _checker.Validate(stubId, conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
      }

      [TestMethod]
      public void ClientIpConditionChecker_Validate_SingleIp_Equal_ShouldReturnValid()
      {
         // arrange
         string stubId = "stub1";
         string clientIp = "127.0.0.1";
         var conditions = new StubConditionsModel
         {
            ClientIp = "127.0.0.1"
         };

         _httpContextServiceMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

         // act
         var result = _checker.Validate(stubId, conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
      }

      [TestMethod]
      public void ClientIpConditionChecker_Validate_IpRange_NotInRange_ShouldReturnInvalid()
      {
         // arrange
         string stubId = "stub1";
         string clientIp = "127.0.0.9";
         var conditions = new StubConditionsModel
         {
            ClientIp = "127.0.0.0/29"
         };

         _httpContextServiceMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

         // act
         var result = _checker.Validate(stubId, conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
      }

      [TestMethod]
      public void ClientIpConditionChecker_Validate_IpRange_InRange_ShouldReturnValid()
      {
         // arrange
         string stubId = "stub1";
         string clientIp = "127.0.0.6";
         var conditions = new StubConditionsModel
         {
            ClientIp = "127.0.0.0/29"
         };

         _httpContextServiceMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

         // act
         var result = _checker.Validate(stubId, conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
      }
   }
}
