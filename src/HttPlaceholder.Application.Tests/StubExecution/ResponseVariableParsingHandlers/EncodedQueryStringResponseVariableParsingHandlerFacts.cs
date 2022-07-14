using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class EncodedQueryStringResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void EncodedQueryStringHandlerFacts_Parse_HappyFlow()
    {
        // Arrange
        var parsingHandler = _mocker.CreateInstance<EncodedQueryStringResponseVariableParsingHandler>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        const string input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
        var queryDict = new Dictionary<string, string> {{"var1", "https://google.com"}, {"var3", "value3"}};
        const string expectedResult = "Query var 1: https%3A%2F%2Fgoogle.com, query var 2: , query var 3: value3";

        httpContextServiceMock
            .Setup(m => m.GetQueryStringDictionary())
            .Returns(queryDict);

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = parsingHandler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
