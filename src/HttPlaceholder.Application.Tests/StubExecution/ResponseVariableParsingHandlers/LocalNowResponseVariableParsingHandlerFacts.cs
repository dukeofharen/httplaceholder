using System;
using System.Globalization;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class LocalNowResponseVariableParsingHandlerFacts
{
    private static readonly DateTime _now = new DateTime(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
    private readonly Mock<IDateTime> _dateTimeMock = new Mock<IDateTime>();
    private LocalNowResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize()
    {
        _dateTimeMock
            .Setup(m => m.Now)
            .Returns(_now);

        _parsingHandler = new LocalNowResponseVariableParsingHandler(_dateTimeMock.Object);
    }

    [TestCleanup]
    public void Cleanup() => _dateTimeMock.VerifyAll();

    [TestMethod]
    public void LocalNowVariableHandler_Parse_HappyFlow_FormatSet()
    {
        // Arrange
        const string input = "((localnow:dd-MM-yyyy HH:mm:ss))";

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches);

        // Assert
        Assert.AreEqual("21-08-2019 20:29:17", result);
    }

    [TestMethod]
    public void LocalNowVariableHandler_Parse_HappyFlow_NoFormatSet()
    {
        // Arrange
        const string input = "((localnow))";

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches);

        // Assert
        Assert.AreEqual(_now.ToString(CultureInfo.InvariantCulture), result);
    }
}