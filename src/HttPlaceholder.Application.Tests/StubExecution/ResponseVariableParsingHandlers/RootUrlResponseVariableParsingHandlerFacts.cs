using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class RootUrlResponseVariableParsingHandlerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new();
    private RootUrlResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize() =>
        _parsingHandler = new RootUrlResponseVariableParsingHandler(_httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public void RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "URL: ((root_url))";
        const string url = "http://localhost:5000";

        const string expectedResult = $"URL: {url}";

        _httpContextServiceMock
            .Setup(m => m.RootUrl)
            .Returns(url);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches, new StubModel());

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
