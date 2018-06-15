using System.Collections.Generic;
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
   public class BasicAuthenticationConditionCheckerFacts
   {
      private Mock<IHttpContextService> _httpContextServiceMock;
      private BasicAuthenticationConditionChecker _checker;

      [TestInitialize]
      public void Initialize()
      {
         _httpContextServiceMock = new Mock<IHttpContextService>();
         _checker = new BasicAuthenticationConditionChecker(
            TestObjectFactory.GetRequestLoggerFactory(),
            _httpContextServiceMock.Object);
      }

      [TestCleanup]
      public void Cleanup()
      {
         _httpContextServiceMock.VerifyAll();
      }

      [TestMethod]
      public void BasicAuthenticationConditionChecker_Validate_StubsFound_ButNoBasicAuthenticationCondition_ShouldReturnNotExecuted()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            BasicAuthentication = null
         };

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.NotExecuted, result);
      }

      [TestMethod]
      public void BasicAuthenticationConditionChecker_Validate_NoAuthorizationHeader_ShouldReturnInvalid()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            BasicAuthentication = new StubBasicAuthenticationModel
            {
               Username = "username",
               Password = "password"
            }
         };

         var headers = new Dictionary<string, string>
         {
            { "X-Api-Key", "1" }
         };

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void BasicAuthenticationConditionChecker_Validate_BasicAuthenticationIncorrect_ShouldReturnInvalid()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            BasicAuthentication = new StubBasicAuthenticationModel
            {
               Username = "username",
               Password = "password"
            }
         };

         var headers = new Dictionary<string, string>
         {
            { "Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmRk" }
         };

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Invalid, result);
      }

      [TestMethod]
      public void BasicAuthenticationConditionChecker_Validate_HappyFlow()
      {
         // arrange
         var conditions = new StubConditionsModel
         {
            BasicAuthentication = new StubBasicAuthenticationModel
            {
               Username = "username",
               Password = "password"
            }
         };

         var headers = new Dictionary<string, string>
         {
            { "Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmQ=" }
         };

         _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

         // act
         var result = _checker.Validate("id", conditions);

         // assert
         Assert.AreEqual(ConditionValidationType.Valid, result);
      }
   }
}
