using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class DisplayUrlResponseVariableParsingHandlerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
    private DisplayUrlResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize() => _parsingHandler = new DisplayUrlResponseVariableParsingHandler(_httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public void RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "URL: ((display_url))";
        const string url = "http://localhost:5000/test.txt?var1=value1&var2=value2";

        var expectedResult = $"URL: {url}";

        _httpContextServiceMock
            .Setup(m => m.DisplayUrl)
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}