using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ConditionCheckers
{
    [TestClass]
    public class HostConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock;
        private HostConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _checker = new HostConditionChecker(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void HostConditionChecker_Validate_NoConditionFound_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Host = null
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void HostConditionChecker_Validate_ConditionIncorrect_ShouldReturnInvalid()
        {
            // arrange
            string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "google.com"
            };

            _httpContextServiceMock
               .Setup(m => m.GetHost())
               .Returns(host);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void HostConditionChecker_Validate_ConditionCorrect_ShouldReturnValid()
        {
            // arrange
            string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "httplaceholder.com"
            };

            _httpContextServiceMock
               .Setup(m => m.GetHost())
               .Returns(host);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void HostConditionChecker_Validate_ConditionCorrectRegex_ShouldReturnValid()
        {
            // arrange
            string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "http(.*)"
            };

            _httpContextServiceMock
               .Setup(m => m.GetHost())
               .Returns(host);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}