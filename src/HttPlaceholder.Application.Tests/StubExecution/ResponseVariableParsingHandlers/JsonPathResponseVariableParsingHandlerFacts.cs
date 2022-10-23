using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class JsonPathResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestMethod]
    public async Task Parse_NoMatches_ShouldReturnInputAsIs()
    {
        // Arrange
        const string input = "input";
        var handler = _mocker.CreateInstance<JsonPathResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task Parse_HasMatches_JsonIsCorrupt_ShouldReplaceVariablesWithEmptyString()
    {
        // Arrange
        const string input = "((jsonpath:$.values[0].title)) ((jsonpath:$.values[1].title))";

        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("wrong json");

        var handler = _mocker.CreateInstance<JsonPathResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(" ", result);
    }

    [TestMethod]
    public async Task Parse_HasMatches_JsonIsOk_ShouldParseInput()
    {
        // Arrange
        const string input = "((jsonpath:$.values[1].title)) ((jsonpath:$.values[0].title))";

        var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
        mockHttpContextService
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(@"{
    ""values"": [
        {
            ""title"": ""Value1""
        },
        {
            ""title"": ""Value2""
        }
    ]
}");

        var handler = _mocker.CreateInstance<JsonPathResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual("Value2 Value1", result);
    }
}
