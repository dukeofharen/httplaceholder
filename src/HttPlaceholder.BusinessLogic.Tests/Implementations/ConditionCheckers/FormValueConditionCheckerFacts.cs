using System.Collections.Generic;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ConditionCheckers
{
    [TestClass]
    public class FormValueConditionCheckerFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock;
        private FormValueConditionChecker _checker;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _checker = new FormValueConditionChecker(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void FormValueConditionChecker_Validate_StubsFound_ButNoFormConditions_ShouldReturnNotExecuted()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Form = null
            };

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void FormValueConditionChecker_Validate_AllConditionsIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Form = new[]
                {
                    new StubFormModel
                    {
                        Key = "key1",
                        Value = "value1"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = "value2"
                    }
                }
            };

            var form = new[]
            {
                ( "key3", new StringValues("value3") ),
                ( "key4", new StringValues("value4") )
            };
            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(form);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void FormValueConditionChecker_Validate_OneConditionIncorrect_ShouldReturnInvalid()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Form = new[]
                {
                    new StubFormModel
                    {
                        Key = "key1",
                        Value = "value1"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = "value2"
                    }
                }
            };

            var form = new[]
            {
                ( "key1", new StringValues("value1") ),
                ( "key2", new StringValues("value3") )
            };
            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(form);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        }

        [TestMethod]
        public void FormValueConditionChecker_Validate_AllConditionsCorrect_ShouldReturnValid_FullText()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Form = new[]
                {
                    new StubFormModel
                    {
                        Key = "key1",
                        Value = "value1"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = "value2"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = "value3"
                    }
                }
            };

            var form = new[]
            {
                ( "key1", new StringValues("value1") ),
                ( "key2", new StringValues(new[] { "value2", "value3" }) )
            };
            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(form);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        [TestMethod]
        public void FormValueConditionChecker_Validate_AllConditionsCorrect_ShouldReturnValid_Regex()
        {
            // arrange
            var conditions = new StubConditionsModel
            {
                Form = new[]
                {
                    new StubFormModel
                    {
                        Key = "key1",
                        Value = "value1"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = @"\bthis\b"
                    },
                    new StubFormModel
                    {
                        Key = "key2",
                        Value = "value3"
                    }
                }
            };

            var form = new[]
            {
                ( "key1", new StringValues("value1") ),
                ( "key2", new StringValues(new[] { "this is a value", "value3" }) )
            };
            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(form);

            // act
            var result = _checker.Validate("id", conditions);

            // assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }
    }
}
