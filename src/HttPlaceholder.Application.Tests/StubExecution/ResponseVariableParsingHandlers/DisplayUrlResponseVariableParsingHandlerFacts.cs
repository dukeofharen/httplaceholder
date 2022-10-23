using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class DisplayUrlResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "URL: ((display_url))";
        const string url = "http://localhost:5000/test.txt?var1=value1&var2=value2";

        const string expectedResult = $"URL: {url}";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<DisplayUrlResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.DisplayUrl)
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
