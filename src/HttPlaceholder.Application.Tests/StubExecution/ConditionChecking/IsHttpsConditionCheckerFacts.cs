using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class IsHttpsConditionCheckerFacts
    {
        private readonly Mock<IClientDataResolver> _clientDataResolverMock = new Mock<IClientDataResolver>();
        private IsHttpsConditionChecker _checker;

        [TestInitialize]
        public void Initialize() => _checker = new IsHttpsConditionChecker(_clientDataResolverMock.Object);

        [TestCleanup]
        public void Cleanup() => _clientDataResolverMock.VerifyAll();

        [TestMethod]
        public void IsHttpsConditionChecker_Validate_NoConditionFound_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    IsHttps = null
                }
            };

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void IsHttpsConditionChecker_Validate_ConditionIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    IsHttps = true
                }
            };

            _clientDataResolverMock
               .Setup(m => m.IsHttps())
               .Returns(false);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void IsHttpsConditionChecker_Validate_ConditionCorrect_ShouldReturnValid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    IsHttps = true
                }
            };

            _clientDataResolverMock
               .Setup(m => m.IsHttps())
               .Returns(true);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
