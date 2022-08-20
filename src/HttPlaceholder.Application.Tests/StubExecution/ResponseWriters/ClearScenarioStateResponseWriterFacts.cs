using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ClearScenarioStateResponseWriterFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task WriteToResponseAsync_ClearStateNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub("scenario-1", null);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        scenarioServiceMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ClearStateIsFalse_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub("scenario-1", false);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        scenarioServiceMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ScenarioNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(null, true);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        scenarioServiceMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ClearStateIsTrue_ShouldDeleteScenario()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stub = CreateStub(scenario, true);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel());

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        scenarioServiceMock.Verify(m => m.DeleteScenarioAsync(scenario));
    }

    private static StubModel CreateStub(string scenario, bool? clearState) =>
        new()
        {
            Scenario = scenario,
            Response = new StubResponseModel
            {
                Scenario = new StubResponseScenarioModel {ClearState = clearState}
            }
        };
}