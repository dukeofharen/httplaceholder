using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common.Utilities;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class ResponseVariableParserFacts
{
    private readonly Mock<IResponseVariableParsingHandler> _handler1 = new();
    private readonly Mock<IResponseVariableParsingHandler> _handler2 = new();
    private ResponseVariableParser _parser;

    [TestInitialize]
    public void Initialize()
    {
        _parser = new ResponseVariableParser(new[] { _handler1.Object, _handler2.Object });

        _handler1
            .Setup(m => m.Name)
            .Returns("handler1");
        _handler2
            .Setup(m => m.Name)
            .Returns("handler2");
    }

    [TestMethod]
    public async Task VariableParser_Parse_HappyFlow()
    {
        // arrange
        const string input = """
                             ((handler1:value1)) ((handler2))
                             ((handler1:bla))
                             ((handler-x))
                             """;

        var stub = new StubModel();
        _handler1
            .Setup(m =>
                m.ParseAsync(input,
                    It.Is<IEnumerable<Match>>(matches =>
                        matches.Any(match => match.Groups[2].Value == "value1" || match.Groups[2].Value == "bla")),
                    stub,
                    It.IsAny<CancellationToken>()))
            .Returns<string, IEnumerable<Match>, StubModel, CancellationToken>((r, _, _, _) => r.AsTask());
        _handler2
            .Setup(m =>
                m.ParseAsync(input,
                    It.Is<IEnumerable<Match>>(matches =>
                        matches.Any(match => string.IsNullOrWhiteSpace(match.Groups[2].Value))), stub,
                    It.IsAny<CancellationToken>()))
            .Returns<string, IEnumerable<Match>, StubModel, CancellationToken>((r, _, _, _) => r.AsTask());

        // act
        var result = await _parser.ParseAsync(input, stub, CancellationToken.None);

        // assert
        Assert.AreEqual(input, result);
        _handler1.Verify(m => m.ParseAsync(input, It.IsAny<IEnumerable<Match>>(), stub, It.IsAny<CancellationToken>()),
            Times.Once);
        _handler2.Verify(m => m.ParseAsync(input, It.IsAny<IEnumerable<Match>>(), stub, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public void VariableParser_VarRegex_HappyFlow()
    {
        // Arrange
        const string input = """
                             Query var 1: ((query:value)), query var 2: ((query)), query var 3: ((query))
                             ((request_body:'key2=([a-z0-9]*)')) ((request_body:key2=([a-z0-9]*) ))((request_body:key2=([a-z0-9]*) ))
                             """;
        var expectedResults = new[]
        {
            ("((query:value))", "query", "value"), ("((query))", "query", string.Empty),
            ("((query))", "query", string.Empty),
            ("((request_body:'key2=([a-z0-9]*)'))", "request_body", "key2=([a-z0-9]*)"),
            ("((request_body:key2=([a-z0-9]*) ))", "request_body", "key2=([a-z0-9]*)"),
            ("((request_body:key2=([a-z0-9]*) ))", "request_body", "key2=([a-z0-9]*)")
        };

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input).ToArray();

        // Assert
        Assert.AreEqual(6, matches.Length);
        for (var i = 0; i < matches.Length; i++)
        {
            var expectedResult = expectedResults[i];
            var match = matches[i];
            Assert.AreEqual(expectedResult.Item1, match.Groups[0].Value);
            Assert.AreEqual(expectedResult.Item2, match.Groups[1].Value);
            Assert.AreEqual(expectedResult.Item3, match.Groups[2].Value);
        }
    }
}
