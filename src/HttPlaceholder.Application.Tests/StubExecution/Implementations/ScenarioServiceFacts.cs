using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class ScenarioServiceFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task IncreaseHitCountAsync_ScenarioIsEmpty_ShouldNotIncreaseHitCount()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var scenarioNotifyMock = _mocker.GetMock<IScenarioNotify>();
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        await service.IncreaseHitCountAsync(string.Empty);

        // Assert
        scenarioStateStoreMock.Verify(m => m.GetScenarioLock(It.IsAny<string>()), Times.Never());
        scenarioNotifyMock.Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Never);
    }

    [TestMethod]
    public async Task IncreaseHitCountAsync_ScenarioIsNotEmpty_ScenarioIsFound_ShouldIncreaseHitCount()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var scenarioNotifyMock = _mocker.GetMock<IScenarioNotify>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenario = "scenario-1";

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenario))
            .Returns(new object());

        var scenarioState = new ScenarioStateModel {Scenario = scenario, HitCount = 2};
        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenario))
            .Returns(scenarioState);

        ScenarioStateModel capturedScenarioStateModel = null;
        scenarioStateStoreMock
            .Setup(m => m.UpdateScenario(scenario, It.IsAny<ScenarioStateModel>()))
            .Callback<string, ScenarioStateModel>((_, ssm) => capturedScenarioStateModel = ssm);

        // Act
        await service.IncreaseHitCountAsync(scenario);

        // Assert
        Assert.IsNotNull(capturedScenarioStateModel);
        Assert.AreEqual(3, capturedScenarioStateModel.HitCount);
        scenarioNotifyMock.Verify(m => m.ScenarioSetAsync(scenarioState), Times.Once);
    }

    [TestMethod]
    public async Task IncreaseHitCountAsync_ScenarioIsNotEmpty_ScenarioIsNotFound_ShouldCreateScenarioAndIncreaseHitCount()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var scenarioNotifyMock = _mocker.GetMock<IScenarioNotify>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenario = "SCENARIO-1";

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenario))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenario))
            .Returns((ScenarioStateModel)null);

        ScenarioStateModel capturedCreatedScenarioStateModel = null;
        scenarioStateStoreMock
            .Setup(m => m.AddScenario(scenario, It.IsAny<ScenarioStateModel>()))
            .Callback<string, ScenarioStateModel>((_, ssm) => capturedCreatedScenarioStateModel = ssm)
            .Returns<string, ScenarioStateModel>((_, ssm) => ssm);

        ScenarioStateModel capturedScenarioStateModel = null;
        scenarioStateStoreMock
            .Setup(m => m.UpdateScenario(scenario, It.IsAny<ScenarioStateModel>()))
            .Callback<string, ScenarioStateModel>((_, ssm) => capturedScenarioStateModel = ssm);

        // Act
        await service.IncreaseHitCountAsync(scenario);

        // Assert
        Assert.IsNotNull(capturedCreatedScenarioStateModel);
        Assert.IsNotNull(capturedScenarioStateModel);
        Assert.AreEqual(1, capturedCreatedScenarioStateModel.HitCount);
        Assert.AreEqual("Start", capturedCreatedScenarioStateModel.State);
        Assert.AreEqual(scenario.ToLower(), capturedCreatedScenarioStateModel.Scenario);
        Assert.AreEqual(1, capturedScenarioStateModel.HitCount);
        Assert.AreEqual("Start", capturedScenarioStateModel.State);
        Assert.AreEqual(scenario.ToLower(), capturedScenarioStateModel.Scenario);
        scenarioNotifyMock.Verify(m => m.ScenarioSetAsync(capturedScenarioStateModel), Times.Once);
    }

    [TestMethod]
    public void GetHitCount_ScenarioNotSet_ShouldReturnNull()
    {
        // Arrange
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        var result = service.GetHitCount(string.Empty);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetHitCount_ScenarioSet_ShouldReturnHitCount()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();
        const string scenario = "SCENARIO-1";

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenario))
            .Returns(new object());

        var scenarioState = new ScenarioStateModel {Scenario = scenario, HitCount = 2};
        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenario))
            .Returns(scenarioState);

        // Act
        var result = service.GetHitCount(scenario);

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetAllScenarios_HappyFlow()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        var scenarios = Array.Empty<ScenarioStateModel>();
        scenarioStateStoreMock
            .Setup(m => m.GetAllScenarios())
            .Returns(scenarios);

        // Act
        var result = service.GetAllScenarios();

        // Assert
        Assert.AreEqual(scenarios, result);
    }

    [TestMethod]
    public void GetScenario_HappyFlow()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel(scenarioName);
        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns(scenario);

        // Act
        var result = service.GetScenario(scenarioName);

        // Assert
        Assert.AreEqual(scenario, result);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioNotSet_ShouldNotAddOrUpdateScenario()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        await service.SetScenarioAsync(string.Empty, null);

        scenarioStateStoreMock.Verify(m => m.GetScenarioLock(It.IsAny<string>()), Times.Never);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Never);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioStateNotSet_ShouldNotAddOrUpdateScenario()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        await service.SetScenarioAsync("scenario-1", null);

        scenarioStateStoreMock.Verify(m => m.GetScenarioLock(It.IsAny<string>()), Times.Never);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Never);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioDoesNotExist_ShouldAddScenario()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var input = new ScenarioStateModel();

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns((ScenarioStateModel)null);

        // Act
        await service.SetScenarioAsync(scenarioName, input);

        // Assert
        scenarioStateStoreMock.Verify(m => m.AddScenario(scenarioName, input));
        scenarioStateStoreMock.Verify(m => m.UpdateScenario(scenarioName, input), Times.Never);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Once);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioInputStateIsNotSet_ShouldSetStateToDefaultState()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var input = new ScenarioStateModel {State = null};

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns((ScenarioStateModel)null);

        // Act
        await service.SetScenarioAsync(scenarioName, input);

        // Assert
        scenarioStateStoreMock.Verify(m =>
            m.AddScenario(scenarioName, It.Is<ScenarioStateModel>(model => model.State == Constants.DefaultScenarioState)));
        scenarioStateStoreMock.Verify(m => m.UpdateScenario(scenarioName, input), Times.Never);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Once);
    }

    [TestMethod]
    public async Task SetScenarioAsync_ScenarioExists_ShouldUpdateScenario()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var input = new ScenarioStateModel();

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns(new ScenarioStateModel());

        // Act
        await service.SetScenarioAsync(scenarioName, input);

        // Assert
        scenarioStateStoreMock.Verify(m => m.AddScenario(scenarioName, input), Times.Never);
        scenarioStateStoreMock.Verify(m => m.UpdateScenario(scenarioName, input));
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Once);
    }

    [TestMethod]
    public async Task
        SetScenarioAsync_ScenarioExists_ProvidedScenarioHitCountIsNotSet_ShouldUpdateScenarioWithExistingHitCount()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var input = new ScenarioStateModel {HitCount = -1};

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns(new ScenarioStateModel {HitCount = 11});

        // Act
        await service.SetScenarioAsync(scenarioName, input);

        // Assert
        scenarioStateStoreMock.Verify(m => m.AddScenario(scenarioName, input), Times.Never);
        scenarioStateStoreMock.Verify(m => m.UpdateScenario(scenarioName, input));
        Assert.AreEqual(11, input.HitCount);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Once);
    }

    [TestMethod]
    public async Task
        SetScenarioAsync_ScenarioExists_ProvidedScenarioStateIsNotSet_ShouldUpdateScenarioWithExistingState()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        const string scenarioName = "scenario-1";
        var input = new ScenarioStateModel {State = null};

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns(new ScenarioStateModel {State = "current-state"});

        // Act
        await service.SetScenarioAsync(scenarioName, input);

        // Assert
        scenarioStateStoreMock.Verify(m => m.AddScenario(scenarioName, input), Times.Never);
        scenarioStateStoreMock.Verify(m => m.UpdateScenario(scenarioName, input));
        Assert.AreEqual("current-state", input.State);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioSetAsync(It.IsAny<ScenarioStateModel>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioNotSet_ShouldReturnFalse()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        var result = await service.DeleteScenarioAsync(string.Empty);

        // Assert
        Assert.IsFalse(result);
        scenarioStateStoreMock.Verify(m => m.GetScenarioLock(It.IsAny<string>()), Times.Never);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioDeletedAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteScenarioAsync_ScenarioSet_ShouldDeleteScenario()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();
        const string scenarioName = "scenario-1";

        scenarioStateStoreMock
            .Setup(m => m.GetScenarioLock(scenarioName))
            .Returns(new object());

        scenarioStateStoreMock
            .Setup(m => m.DeleteScenario(scenarioName))
            .Returns(true);

        // Act
        var result = await service.DeleteScenarioAsync(scenarioName);

        // Assert
        Assert.IsTrue(result);
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.ScenarioDeletedAsync(scenarioName), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAllScenariosAsync_HappyFlow()
    {
        // Arrange
        var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
        var service = _mocker.CreateInstance<ScenarioService>();

        // Act
        await service.DeleteAllScenariosAsync();

        // Assert
        scenarioStateStoreMock.Verify(m => m.DeleteAllScenarios());
        _mocker.GetMock<IScenarioNotify>().Verify(m => m.AllScenariosDeletedAsync(), Times.Once);
    }
}
