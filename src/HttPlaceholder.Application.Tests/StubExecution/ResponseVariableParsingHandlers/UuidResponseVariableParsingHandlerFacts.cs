using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class UuidResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void UuidVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "((uuid)) ((uuid:nonsense))";

        var handler = _mocker.CreateInstance<UuidResponseVariableParsingHandler>();

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = handler.Parse(input, matches, new StubModel());

        // assert
        var parts = result.Split(' ');
        Assert.AreEqual(2, parts.Length);
        Assert.IsTrue(parts.All(p => Guid.TryParse(p, out _)));
        Assert.AreNotEqual(parts[0], parts[1]);
    }
}
