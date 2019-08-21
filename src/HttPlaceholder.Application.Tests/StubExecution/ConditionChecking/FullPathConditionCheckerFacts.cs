using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class FullPathConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private FullPathConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _checker = new FullPathConditionChecker(
               _httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void FullPathConditionChecker_Validate_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    FullPath = null
                }
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void FullPathConditionChecker_Validate_StubsFound_WrongPath_ShouldReturnInvalid()
        {
            // arrange
            string path = "/login?success=true";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    FullPath = "/login?success=false"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.FullPath)
               .Returns(path);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void FullPathConditionChecker_Validate_StubsFound_HappyFlow_CompleteUrl()
        {
            // arrange
            string path = "/login?success=true";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    FullPath = "/login?success=true"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.FullPath)
               .Returns(path);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void FullPathConditionChecker_Validate_StubsFound_HappyFlow_Regex()
        {
            // arrange
            string path = "/locatieserver/v3/suggest";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    FullPath = @"\blocatieserver\/v3\/suggest\b"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.FullPath)
               .Returns(path);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
