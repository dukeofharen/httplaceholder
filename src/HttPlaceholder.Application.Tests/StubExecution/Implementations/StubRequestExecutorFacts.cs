using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Extensibility.Notifications;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class StubRequestExecutorFacts
{
    private readonly Mock<IConditionChecker> _conditionCheckerMock1 = new();
    private readonly Mock<IConditionChecker> _conditionCheckerMock2 = new();
    private readonly AutoMocker _mocker = new();

    private readonly FullStubModel _stub1 = new() {Stub = new StubModel(), Metadata = new StubMetadataModel()};

    private readonly FullStubModel _stub2 = new() {Stub = new StubModel(), Metadata = new StubMetadataModel()};

    [TestInitialize]
    public void Initialize()
    {
        _mocker.Use<IEnumerable<IConditionChecker>>(
            new[] {_conditionCheckerMock1.Object, _conditionCheckerMock2.Object});
        _mocker.Use(TestObjectFactory.GetRequestLoggerFactory());

        var stubContextMock = _mocker.GetMock<IStubContext>();
        stubContextMock
            .Setup(m => m.GetStubsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {_stub1, _stub2});
    }

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionPassed_ShouldThrowException()
    {
        // Arrange
        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Invalid});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Invalid});

        var executor = _mocker.CreateInstance<StubRequestExecutor>();

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<RequestValidationException>(() =>
                executor.ExecuteRequestAsync(CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("and the request did not pass"));
    }

    [TestMethod]
    public async Task StubRequestExecutor_BeforeCheckingNotificationSentAndHandled_ShouldReturnCustomResponse()
    {
        // Arrange
        var executor = _mocker.CreateInstance<StubRequestExecutor>();
        var mediatorMock = _mocker.GetMock<IMediator>();

        var expectedResponse = new ResponseModel();
        mediatorMock
            .Setup(m => m.Publish(
                It.Is<BeforeCheckingStubConditionsNotification>(n => n.ConditionCheckers != null && n.Stubs != null),
                It.IsAny<CancellationToken>()))
            .Callback<BeforeCheckingStubConditionsNotification, CancellationToken>((n, _) =>
                n.Response = expectedResponse);

        // Act
        var result = await executor.ExecuteRequestAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResponse, result);
    }

    [TestMethod]
    public async Task StubRequestExecutor_ExecuteRequestAsync_NoConditionExecuted_ShouldPickFirstStub()
    {
        // Arrange
        var expectedResponseModel = new ResponseModel();

        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.NotExecuted});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(It.IsAny<StubModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.NotExecuted});

        var finalStubDeterminerMock = _mocker.GetMock<IFinalStubDeterminer>();
        finalStubDeterminerMock
            .Setup(m => m.DetermineFinalStub(
                It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s =>
                    s.Any(fs => fs.Item1 == _stub1.Stub))))
            .Returns(_stub1.Stub);

        var stubResponseGeneratorMock = _mocker.GetMock<IStubResponseGenerator>();
        stubResponseGeneratorMock
            .Setup(m => m.GenerateResponseAsync(_stub1.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponseModel);

        var executor = _mocker.CreateInstance<StubRequestExecutor>();

        // Act
        var result = await executor.ExecuteRequestAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResponseModel, result);
    }

    [TestMethod]
    public async Task StubRequestExecutor_ExecuteRequestAsync_MultipleValidStubs_ShouldPickStubWithHighestPriority()
    {
        // Arrange
        var expectedResponseModel = new ResponseModel();

        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(_stub1.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(_stub1.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});

        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});

        var finalStubDeterminerMock = _mocker.GetMock<IFinalStubDeterminer>();
        finalStubDeterminerMock
            .Setup(m => m.DetermineFinalStub(
                It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s =>
                    s.Any(fs => fs.Item1 == _stub2.Stub))))
            .Returns(_stub2.Stub);

        var stubResponseGeneratorMock = _mocker.GetMock<IStubResponseGenerator>();
        stubResponseGeneratorMock
            .Setup(m => m.GenerateResponseAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponseModel);

        var executor = _mocker.CreateInstance<StubRequestExecutor>();

        // Act
        var response = await executor.ExecuteRequestAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResponseModel, response);
    }

    [TestMethod]
    public async Task StubRequestExecutor_ExecuteRequestAsync_HappyFlow()
    {
        // Arrange
        var expectedResponseModel = new ResponseModel();
        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(_stub1.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Invalid});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(_stub1.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Invalid});

        _conditionCheckerMock1
            .Setup(m => m.ValidateAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});
        _conditionCheckerMock2
            .Setup(m => m.ValidateAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ConditionCheckResultModel {ConditionValidation = ConditionValidationType.Valid});

        var finalStubDeterminerMock = _mocker.GetMock<IFinalStubDeterminer>();
        finalStubDeterminerMock
            .Setup(m => m.DetermineFinalStub(
                It.Is<List<(StubModel, IEnumerable<ConditionCheckResultModel>)>>(s =>
                    s.Any(fs => fs.Item1 == _stub2.Stub))))
            .Returns(_stub2.Stub);

        var stubResponseGeneratorMock = _mocker.GetMock<IStubResponseGenerator>();
        stubResponseGeneratorMock
            .Setup(m => m.GenerateResponseAsync(_stub2.Stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponseModel);

        var executor = _mocker.CreateInstance<StubRequestExecutor>();

        // Act
        var response = await executor.ExecuteRequestAsync(CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResponseModel, response);

        _mocker.GetMock<IStubContext>()
            .Verify(m => m.IncreaseHitCountAsync(_stub2.Stub.Scenario, It.IsAny<CancellationToken>()));
        _mocker.GetMock<IMediator>()
            .Verify(m => m.Publish(It.Is<BeforeStubResponseReturnedNotification>(n => n.Response == response),
                It.IsAny<CancellationToken>()));
    }
}
