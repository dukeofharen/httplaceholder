using System;
using System.Globalization;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class LocalNowResponseVariableParsingHandlerFacts
{
    private static readonly DateTime _now = new(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void LocalNowVariableHandler_Parse_HappyFlow_FormatSet()
    {
        // Arrange
        const string input = "((localnow:dd-MM-yyyy HH:mm:ss))";

        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.Now)
            .Returns(_now);

        var handler = _mocker.CreateInstance<LocalNowResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = handler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual("21-08-2019 20:29:17", result);
    }

    [TestMethod]
    public void LocalNowVariableHandler_Parse_HappyFlow_NoFormatSet()
    {
        // Arrange
        const string input = "((localnow))";

        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.Now)
            .Returns(_now);

        var handler = _mocker.CreateInstance<LocalNowResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = handler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual(_now.ToString(CultureInfo.InvariantCulture), result);
    }
}
