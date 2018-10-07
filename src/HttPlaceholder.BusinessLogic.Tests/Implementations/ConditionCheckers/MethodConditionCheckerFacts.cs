using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ConditionCheckers
{
    [TestClass]
    public class MethodConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock;
        private MethodConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _checker = new MethodConditionChecker(
               _httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void MethodConditionChecker_Validate_StubsFound_ButNoMethodConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Method = null
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void MethodConditionChecker_Validate_StubsFound_WrongMethod_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Method = "POST"
            };

            _httpContextServiceMock
               .Setup(m => m.Method)
               .Returns("GET");

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void MethodConditionChecker_Validate_StubsFound_HappyFlow()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Method = "GET"
            };

            _httpContextServiceMock
               .Setup(m => m.Method)
               .Returns("GET");

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}