using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class BasicAuthenticationConditionCheckerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private BasicAuthenticationConditionChecker _checker;

        [TestInitialize]
        public void Initialize() =>
            _checker = new BasicAuthenticationConditionChecker(
                _httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void
            BasicAuthenticationConditionChecker_Validate_StubsFound_ButNoBasicAuthenticationCondition_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel {BasicAuthentication = null};

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void
            BasicAuthenticationConditionChecker_Validate_StubsFound_NoUsernameAndPasswordSet_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                BasicAuthentication = new StubBasicAuthenticationModel {Username = null, Password = null}
            };

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void BasicAuthenticationConditionChecker_Validate_NoAuthorizationHeader_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                BasicAuthentication = new StubBasicAuthenticationModel
                {
                    Username = "username", Password = "password"
                }
            };

            var headers = new Dictionary<string, string> {{"X-Api-Key", "1"}};

            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void BasicAuthenticationConditionChecker_Validate_BasicAuthenticationIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                BasicAuthentication = new StubBasicAuthenticationModel
                {
                    Username = "username", Password = "password"
                }
            };

            var headers = new Dictionary<string, string> {{"Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmRk"}};

            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void BasicAuthenticationConditionChecker_Validate_HappyFlow()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                BasicAuthentication = new StubBasicAuthenticationModel
                {
                    Username = "username", Password = "password"
                }
            };

            var headers = new Dictionary<string, string> {{"Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmQ="}};

            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
