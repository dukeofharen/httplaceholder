using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Domain.Entities;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class ScenarioHitCountVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Parse_NoMatches_ShouldReturnInputAsIs()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ScenarioHitCountVariableParsingHandler>();
        const string input = "the input";

        // Act
        var result = await handler.ParseAsync(input, Array.Empty<Match>(), new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task Parse_Matches_ShouldParseHitCount()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ScenarioHitCountVariableParsingHandler>();
        var stubContextMock = _mocker.GetMock<IStubContext>();
        const string input =
            "((scenario_hitcount)) ((scenario_hitcount:scenario_exists)) ((scenario_hitcount:scenario_doesnt_exist))";
        const string expectedResult = "3 1337 ";

        var stubModel = new StubModel { Scenario = "stub-scenario" };

        stubContextMock
            .Setup(m => m.GetScenarioAsync(stubModel.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScenarioStateModel { HitCount = 3 });
        stubContextMock
            .Setup(m => m.GetScenarioAsync("scenario_exists", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ScenarioStateModel { HitCount = 1337 });
        stubContextMock
            .Setup(m => m.GetScenarioAsync("scenario_doesnt_exist", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScenarioStateModel)null);

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, stubModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task Parse_Matches_ShouldReadFromHttpContext_ShouldParseHitCount()
    {
        // Arrange
        var handler = _mocker.CreateInstance<ScenarioHitCountVariableParsingHandler>();
        var cacheServiceMock = _mocker.GetMock<ICacheService>();

        const string input =
            "((scenario_hitcount))";
        const string expectedResult = "18";

        var stubModel = new StubModel { Scenario = "stub-scenario" };

        cacheServiceMock
            .Setup(m => m.GetScopedItem<ScenarioStateModel>(CachingKeys.ScenarioState))
            .Returns(new ScenarioStateModel { HitCount = 18 });

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, stubModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
