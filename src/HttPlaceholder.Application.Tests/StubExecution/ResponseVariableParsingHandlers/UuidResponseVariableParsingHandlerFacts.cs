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
    public async Task UuidVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "((uuid)) ((uuid:nonsense))";

        var handler = _mocker.CreateInstance<UuidResponseVariableParsingHandler>();

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, new StubModel(), CancellationToken.None);

        // assert
        var parts = result.Split(' ');
        Assert.AreEqual(2, parts.Length);
        Assert.IsTrue(parts.All(p => Guid.TryParse(p, out _)));
        Assert.AreNotEqual(parts[0], parts[1]);
    }
}
