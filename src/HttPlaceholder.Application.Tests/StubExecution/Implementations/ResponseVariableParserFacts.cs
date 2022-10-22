using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
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
        _parser = new ResponseVariableParser(new[] {_handler1.Object, _handler2.Object});

        _handler1
            .Setup(m => m.Name)
            .Returns("handler1");
        _handler2
            .Setup(m => m.Name)
            .Returns("handler2");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _handler1.VerifyAll();
        _handler2.VerifyAll();
    }

    [TestMethod]
    public void VariableParser_Parse_HappyFlow()
    {
        // arrange
        const string input = @"((handler1:value1)) ((handler2))
((handler1:bla))
((handler-x))";

        var stub = new StubModel();
        _handler1
            .Setup(m =>
                m.Parse(input,
                    It.Is<IEnumerable<Match>>(matches =>
                        matches.Any(match => match.Groups[2].Value == "value1" || match.Groups[2].Value == "bla")),
                    stub))
            .Returns<string, IEnumerable<Match>, StubModel>((r, _, _) => r);
        _handler2
            .Setup(m =>
                m.Parse(input,
                    It.Is<IEnumerable<Match>>(matches =>
                        matches.Any(match => string.IsNullOrWhiteSpace(match.Groups[2].Value))), stub))
            .Returns<string, IEnumerable<Match>, StubModel>((r, _, _) => r);

        // act
        var result = _parser.Parse(input, stub);

        // assert
        Assert.AreEqual(input, result);
        _handler1.Verify(m => m.Parse(input, It.IsAny<IEnumerable<Match>>(), stub), Times.Once);
        _handler2.Verify(m => m.Parse(input, It.IsAny<IEnumerable<Match>>(), stub), Times.Once);
    }
}
