using System.Globalization;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class UtcNowResponseVariableParsingHandlerFacts
{
    private static readonly DateTime _now = new(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task UtcNowVariableHandler_Parse_HappyFlow_FormatSet()
    {
        // Arrange
        const string input = "((utcnow:dd-MM-yyyy HH:mm:ss))";

        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(_now);

        var handler = _mocker.CreateInstance<UtcNowResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual("21-08-2019 20:29:17", result);
    }

    [TestMethod]
    public async Task UtcNowVariableHandler_Parse_HappyFlow_NoFormatSet()
    {
        // Arrange
        const string input = "((utcnow))";

        var dateTimeMock = _mocker.GetMock<IDateTime>();
        dateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(_now);

        var handler = _mocker.CreateInstance<UtcNowResponseVariableParsingHandler>();

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(_now.ToString(CultureInfo.InvariantCulture), result);
    }
}
