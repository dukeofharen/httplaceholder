using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

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
        var stubContextMock = _mocker.GetMock<IStubContext>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel(), CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        stubContextMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ClearStateIsFalse_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub("scenario-1", false);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel(), CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        stubContextMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ScenarioNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(null, true);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel(), CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        stubContextMock.Verify(m => m.DeleteScenarioAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_ClearStateIsTrue_ShouldDeleteScenario()
    {
        // Arrange
        const string scenario = "scenario-1";
        var stub = CreateStub(scenario, true);
        var writer = _mocker.CreateInstance<ClearScenarioStateResponseWriter>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, new ResponseModel(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual("ClearScenarioStateResponseWriter", result.ResponseWriterName);
        stubContextMock.Verify(m => m.DeleteScenarioAsync(scenario, It.IsAny<CancellationToken>()));
    }

    private static StubModel CreateStub(string scenario, bool? clearState) =>
        new()
        {
            Scenario = scenario,
            Response = new StubResponseModel {Scenario = new StubResponseScenarioModel {ClearState = clearState}}
        };
}
