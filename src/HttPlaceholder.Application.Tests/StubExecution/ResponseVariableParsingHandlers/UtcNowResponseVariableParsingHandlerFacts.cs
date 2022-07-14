using System;
using System.Globalization;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class UtcNowResponseVariableParsingHandlerFacts
{
    private static readonly DateTime _now = new(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
    private readonly Mock<IDateTime> _dateTimeMock = new();
    private UtcNowResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize()
    {
        _dateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(_now);

        _parsingHandler = new UtcNowResponseVariableParsingHandler(_dateTimeMock.Object);
    }

    [TestCleanup]
    public void Cleanup() => _dateTimeMock.VerifyAll();

    [TestMethod]
    public void UtcNowVariableHandler_Parse_HappyFlow_FormatSet()
    {
        // Arrange
        const string input = "((utcnow:dd-MM-yyyy HH:mm:ss))";

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual("21-08-2019 20:29:17", result);
    }

    [TestMethod]
    public void UtcNowVariableHandler_Parse_HappyFlow_NoFormatSet()
    {
        // Arrange
        const string input = "((utcnow))";

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches, new StubModel());

        // Assert
        Assert.AreEqual(_now.ToString(CultureInfo.InvariantCulture), result);
    }
}
