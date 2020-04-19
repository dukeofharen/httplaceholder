using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class HostConditionCheckerFacts
    {
        private readonly Mock<IClientDataResolver> _clientDataResolverMock = new Mock<IClientDataResolver>();
        private HostConditionChecker _checker;

        [TestInitialize]
        public void Initialize() => _checker = new HostConditionChecker(_clientDataResolverMock.Object);

        [TestCleanup]
        public void Cleanup() => _clientDataResolverMock.VerifyAll();

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
            const string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "google.com"
            };

            _clientDataResolverMock
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
            const string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "httplaceholder.com"
            };

            _clientDataResolverMock
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
            const string host = "httplaceholder.com";
            var conditions = new StubConditionsModel
            {
                Host = "http(.*)"
            };

            _clientDataResolverMock
               .Setup(m => m.GetHost())
               .Returns(host);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
