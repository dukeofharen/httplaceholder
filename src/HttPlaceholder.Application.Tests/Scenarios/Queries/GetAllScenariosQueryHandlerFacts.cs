using HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.Tests.Scenarios.Queries;

[TestClass]
public class GetAllScenariosQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<GetAllScenariosQueryHandler>();

        var scenarios = Array.Empty<ScenarioStateModel>();
        scenarioServiceMock
            .Setup(m => m.GetAllScenarios())
            .Returns(scenarios);

        // Act
        var result = await handler.Handle(new GetAllScenariosQuery(), CancellationToken.None);

        // Assert
        Assert.AreEqual(scenarios, result);
    }
}
