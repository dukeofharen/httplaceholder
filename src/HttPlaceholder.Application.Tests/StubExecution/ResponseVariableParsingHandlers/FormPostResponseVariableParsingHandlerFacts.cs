using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class FormPostResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void FormPostVariableHandler_Parse_HappyFlow()
    {
        // Arrange
        var parsingHandler = _mocker.CreateInstance<FormPostResponseVariableParsingHandler>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        const string input = "Form var 1: ((form_post:var1)), Form var 2: ((form_post:var2)), Form var 3: ((form_post:var3))";

        var formTuples = new[]
        {
            ("var1", new StringValues("https://google.com")),
            ("var3", new StringValues("value3"))
        };

        const string expectedResult = "Form var 1: https://google.com, Form var 2: , Form var 3: value3";

        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(formTuples);

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = parsingHandler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
