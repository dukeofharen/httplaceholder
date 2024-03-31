using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class QueryStringResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task QueryStringHandlerFacts_Parse_HappyFlow()
    {
        // arrange
        const string input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
        var queryDict = new Dictionary<string, string> { { "var1", "https://google.com" }, { "var3", "value3" } };
        const string expectedResult = "Query var 1: https://google.com, query var 2: , query var 3: value3";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<QueryStringResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(queryDict);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task QueryStringHandlerFacts_Parse_NumberOfMatchesIncorrect_ShouldReplaceWithEmptyString()
    {
        // arrange
        const string input = "Query var 1: ((query)), query var 2: ((query)), query var 3: ((query))";
        var queryDict = new Dictionary<string, string> { { "var1", "https://google.com" }, { "var3", "value3" } };
        const string expectedResult = "Query var 1: , query var 2: , query var 3: ";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<QueryStringResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(queryDict);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
