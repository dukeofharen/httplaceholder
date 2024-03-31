using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class RequestBodyResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "Posted content: ((request_body))";
        const string body = "POSTED BODY";

        const string expectedResult = "Posted content: POSTED BODY";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<RequestBodyResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_RegexSingleLine_HappyFlow()
    {
        // arrange
        const string input = "Posted content: ((request_body:'key=([a-z]*)'))";
        const string body = "key=value";

        const string expectedResult = "Posted content: value";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<RequestBodyResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_RegexMultiLine_HappyFlow()
    {
        // arrange
        const string input = "Posted content: ((request_body:'key2=([a-z0-9]*)'))";
        const string body = """
                            key1=value1
                            key2=value2
                            key3=value3
                            """;

        const string expectedResult = "Posted content: value2";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<RequestBodyResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task RequestBodyVariableHandler_Parse_RegexNoResultFound_HappyFlow()
    {
        // arrange
        const string input = "Posted content: ((request_body:'key4=([a-z0-9]*)'))";
        const string body = """
                            key1=value1
                            key2=value2
                            key3=value3
                            """;

        const string expectedResult = "Posted content: ";

        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var handler = _mocker.CreateInstance<RequestBodyResponseVariableParsingHandler>();

        httpContextServiceMock
            .Setup(m => m.GetBodyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
