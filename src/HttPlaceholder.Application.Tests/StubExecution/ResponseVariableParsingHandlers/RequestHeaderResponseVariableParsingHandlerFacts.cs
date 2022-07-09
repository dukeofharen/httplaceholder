using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class RequestHeaderResponseVariableParsingHandlerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new();
    private RequestHeaderResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize() => _parsingHandler = new RequestHeaderResponseVariableParsingHandler(_httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public void RequestHeaderVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "Header var 1: ((request_header:var1)), header var 2: ((request_header:var2)), header var 3: ((request_header:var3)), header var 4: ((request_header:var4))";
        var headerDict = new Dictionary<string, string>
        {
            { "var1", "https://google.com" },
            { "var3", "value3" },
            { "VAr4", "value4" }
        };
        const string expectedResult = "Header var 1: https://google.com, header var 2: , header var 3: value3, header var 4: value4";

        _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headerDict);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches, new StubModel());

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
