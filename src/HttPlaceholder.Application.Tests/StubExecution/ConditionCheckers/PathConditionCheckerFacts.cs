using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers
{
    [TestClass]
    public class PathConditionCheckerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private PathConditionChecker _checker;

        [TestInitialize]
        public void Initialize() =>
            _checker = new PathConditionChecker(
                _httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void PathConditionChecker_Validate_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Path = null
                }
            };

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void PathConditionChecker_Validate_StubsFound_WrongPath_ShouldReturnInvalid()
        {
            // arrange
            const string path = "/login";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Path = @"\blocatieserver\/v3\/suggest\b"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.Path)
               .Returns(path);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void PathConditionChecker_Validate_StubsFound_HappyFlow_CompleteUrl()
        {
            // arrange
            const string path = "/locatieserver/v3/suggest";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Path = @"/locatieserver/v3/suggest"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.Path)
               .Returns(path);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void PathConditionChecker_Validate_StubsFound_HappyFlow_Regex()
        {
            // arrange
            const string path = "/locatieserver/v3/suggest";
            var conditions = new StubConditionsModel
            {
                Url = new StubUrlConditionModel
                {
                    Path = @"\blocatieserver\/v3\/suggest\b"
                }
            };

            _httpContextServiceMock
               .Setup(m => m.Path)
               .Returns(path);

            // act
            var result = _checker.Validate(new StubModel{Id = "id", Conditions = conditions});

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
