using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class RootUrlResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "URL: ((root_url))";
        const string url = "http://localhost:5000";

        const string expectedResult = $"URL: {url}";

        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var handler = _mocker.CreateInstance<RootUrlResponseVariableParsingHandler>();

        urlResolverMock
            .Setup(m => m.GetRootUrl())
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
