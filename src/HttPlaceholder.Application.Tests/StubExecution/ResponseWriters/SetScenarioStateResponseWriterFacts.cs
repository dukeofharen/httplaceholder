using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class SetScenarioStateResponseWriterFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public async Task WriteToResponseAsync_SetScenarioStateNotSet_ShouldReturnNotExecuted()
        {
            // Arrange
            var stub = CreateStub("scenario-1", string.Empty);
            var writer = _mocker.CreateInstance<SetScenarioStateResponseWriter>();
            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

            // Act
            var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

            // Assert
            Assert.IsFalse(result.Executed);
            Assert.AreEqual("SetScenarioStateResponseWriter", result.ResponseWriterName);
            scenarioServiceMock.Verify(m => m.GetScenario(It.IsAny<string>()), Times.Never);
            scenarioServiceMock.Verify(m => m.SetScenario(It.IsAny<string>(), It.IsAny<ScenarioStateModel>()),
                Times.Never);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_ScenarioNotSet_ShouldReturnNotExecuted()
        {
            // Arrange
            var stub = CreateStub(string.Empty, "new-state");
            var writer = _mocker.CreateInstance<SetScenarioStateResponseWriter>();
            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

            // Act
            var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

            // Assert
            Assert.IsFalse(result.Executed);
            Assert.AreEqual("SetScenarioStateResponseWriter", result.ResponseWriterName);
            scenarioServiceMock.Verify(m => m.GetScenario(It.IsAny<string>()), Times.Never);
            scenarioServiceMock.Verify(m => m.SetScenario(It.IsAny<string>(), It.IsAny<ScenarioStateModel>()),
                Times.Never);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_ScenarioNotFound_ShouldCreateNewScenario()
        {
            // Arrange
            const string scenario = "scenario-1";
            const string state = "new-state";
            var stub = CreateStub(scenario, state);
            var writer = _mocker.CreateInstance<SetScenarioStateResponseWriter>();
            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

            scenarioServiceMock
                .Setup(m => m.GetScenario(scenario))
                .Returns((ScenarioStateModel)null);
            ScenarioStateModel actualScenarioStateModel = null;
            scenarioServiceMock
                .Setup(m => m.SetScenario(scenario, It.IsAny<ScenarioStateModel>()))
                .Callback<string, ScenarioStateModel>((_, m) => actualScenarioStateModel = m);

            // Act
            var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual("SetScenarioStateResponseWriter", result.ResponseWriterName);
            Assert.IsNotNull(actualScenarioStateModel);
            Assert.AreEqual(state, actualScenarioStateModel.State);
            Assert.AreEqual(scenario, actualScenarioStateModel.Scenario);
            Assert.AreEqual(0, actualScenarioStateModel.HitCount);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_ScenarioFound_ShouldUpdateScenario()
        {
            // Arrange
            const string scenario = "scenario-1";
            const string state = "new-state";
            var stub = CreateStub(scenario, state);
            var writer = _mocker.CreateInstance<SetScenarioStateResponseWriter>();
            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

            var currentScenarioState = new ScenarioStateModel(scenario) {State = Constants.DefaultScenarioState};
            scenarioServiceMock
                .Setup(m => m.GetScenario(scenario))
                .Returns(currentScenarioState);
            scenarioServiceMock
                .Setup(m => m.SetScenario(scenario, currentScenarioState));

            // Act
            var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual("SetScenarioStateResponseWriter", result.ResponseWriterName);
            Assert.AreEqual(state, currentScenarioState.State);
        }

        private static StubModel CreateStub(string scenario, string setScenarioState) =>
            new StubModel
            {
                Scenario = scenario,
                Response = new StubResponseModel
                {
                    Scenario = new StubResponseScenarioModel {SetScenarioState = setScenarioState}
                }
            };
    }
}
