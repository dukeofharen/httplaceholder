using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.TestUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class StubRequestExecutorFacts
    {
        private Mock<IFinalStubDeterminer> _finalStubDeterminerMock;
        private Mock<ILogger<StubRequestExecutor>> _loggerMock;
        private Mock<IStubContext> _stubContextMock;
        private Mock<IStubResponseGenerator> _stubResponseGeneratorMock;
        private Mock<IConditionChecker> _conditionCheckerMock1;
        private Mock<IConditionChecker> _conditionCheckerMock2;
        private FullStubModel _stub1;
        private FullStubModel _stub2;
        private StubRequestExecutor _executor;

        [TestInitialize]
        public void Initialize()
        {
            _finalStubDeterminerMock = new Mock<IFinalStubDeterminer>();
            _loggerMock = new Mock<ILogger<StubRequestExecutor>>();
            _stubContextMock = new Mock<IStubContext>();
            _stubResponseGeneratorMock = new Mock<IStubResponseGenerator>();
            _conditionCheckerMock1 = new Mock<IConditionChecker>();
            _conditionCheckerMock2 = new Mock<IConditionChecker>();

            _executor = new StubRequestExecutor(
                new[] { _conditionCheckerMock1.Object, _conditionCheckerMock2.Object },
                _finalStubDeterminerMock.Object,
               _loggerMock.Object,
               TestObjectFactory.GetRequestLoggerFactory(),
               _stubContextMock.Object,
               _stubResponseGeneratorMock.Object);

            _stub1 = new FullStubModel
            {
                Stub = new StubModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Conditions = new StubConditionsModel(),
                    NegativeConditions = new StubConditionsModel(),
                    Priority = -1
                },
                Metadata = new StubMetadataModel()
            };
            _stub2 = new FullStubModel
            {
                Stub = new StubModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Conditions = new StubConditionsModel(),
                    NegativeConditions = new StubConditionsModel(),
                    Priority = 0
                },
                Metadata = new StubMetadataModel()
            };
            _stubContextMock
               .Setup(m => m.GetStubsAsync())
               .ReturnsAsync(new[] { _stub1, _stub2 });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _finalStubDeterminerMock.VerifyAll();
            _stubContextMock.VerifyAll();
            _stubResponseGeneratorMock.VerifyAll();
        }

        [TestMethod]
        public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionPassed_ShouldThrowException()
        {
            // arrange
            _conditionCheckerMock1
               .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });

            // act
            var exception = await Assert.ThrowsExceptionAsync<RequestValidationException>(() => _executor.ExecuteRequestAsync());

            // assert
            Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
        }

        [TestMethod]
        public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionExecuted_ShouldPickFirstStub()
        {
            // arrange
            var expectedResponseModel = new ResponseModel();

            _conditionCheckerMock1
               .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.NotExecuted });
            _conditionCheckerMock2
               .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<StubConditionsModel>()))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.NotExecuted });

            _finalStubDeterminerMock
                .Setup(m => m.DetermineFinalStub(It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s => s.Any(fs => fs.Item1 == _stub1.Stub))))
                .Returns(_stub1.Stub);
            _stubResponseGeneratorMock
               .Setup(m => m.GenerateResponseAsync(_stub1.Stub))
               .ReturnsAsync(expectedResponseModel);

            // act
            var result = await _executor.ExecuteRequestAsync();

            // assert
            Assert.AreEqual(expectedResponseModel, result);
        }

        [TestMethod]
        public async Task StubRequestExecutor_ExecuteRequestAsync_MultipleValidStubs_ShouldPickStubWithLowestPriority()
        {
            // arrange
            var expectedResponseModel = new ResponseModel();

            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });

            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });

            _finalStubDeterminerMock
                .Setup(m => m.DetermineFinalStub(It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s => s.Any(fs => fs.Item1 == _stub2.Stub))))
                .Returns(_stub2.Stub);
            _stubResponseGeneratorMock
               .Setup(m => m.GenerateResponseAsync(_stub2.Stub))
               .ReturnsAsync(expectedResponseModel);

            // act
            var response = await _executor.ExecuteRequestAsync();

            // assert
            Assert.AreEqual(expectedResponseModel, response);
        }

        [TestMethod]
        public async Task StubRequestExecutor_ExecuteRequestAsync_HappyFlow()
        {
            // arrange
            var expectedResponseModel = new ResponseModel();
            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub1.Stub.Id, _stub1.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });

            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.Conditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Valid });
            _conditionCheckerMock1
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });
            _conditionCheckerMock2
               .Setup(m => m.Validate(_stub2.Stub.Id, _stub2.Stub.NegativeConditions))
               .Returns(() => new ConditionCheckResultModel { ConditionValidation = ConditionValidationType.Invalid });

            _finalStubDeterminerMock
                .Setup(m => m.DetermineFinalStub(It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s => s.Any(fs => fs.Item1 == _stub2.Stub))))
                .Returns(_stub2.Stub);
            _stubResponseGeneratorMock
               .Setup(m => m.GenerateResponseAsync(_stub2.Stub))
               .ReturnsAsync(expectedResponseModel);

            // act
            var response = await _executor.ExecuteRequestAsync();

            // assert
            Assert.AreEqual(expectedResponseModel, response);
        }
    }
}
