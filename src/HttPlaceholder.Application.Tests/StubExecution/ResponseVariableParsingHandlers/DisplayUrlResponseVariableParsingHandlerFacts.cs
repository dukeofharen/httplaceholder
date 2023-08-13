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

        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var handler = _mocker.CreateInstance<DisplayUrlResponseVariableParsingHandler>();

        urlResolverMock
            .Setup(m => m.GetDisplayUrl())
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_Regex_HappyFlow()
    {
        // arrange
        const string input = @"User ID: ((display_url:'\/users\/([0-9]{3})\/orders'))";
        const string url = "http://localhost:5000/users/123/orders?key=value123";

        const string expectedResult = "User ID: 123";

        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var handler = _mocker.CreateInstance<DisplayUrlResponseVariableParsingHandler>();

        urlResolverMock
            .Setup(m => m.GetDisplayUrl())
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_Regex_NoResultFound_HappyFlow()
    {
        // arrange
        const string input = @"User ID: ((display_url:'\/users\/([A-Z]{3})\/orders'))";
        const string url = "http://localhost:5000/users/123/orders?key=value123";

        const string expectedResult = "User ID: ";

        var urlResolverMock = _mocker.GetMock<IUrlResolver>();
        var handler = _mocker.CreateInstance<DisplayUrlResponseVariableParsingHandler>();

        urlResolverMock
            .Setup(m => m.GetDisplayUrl())
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
