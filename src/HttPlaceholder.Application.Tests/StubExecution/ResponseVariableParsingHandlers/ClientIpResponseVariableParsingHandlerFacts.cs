using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class ClientIpResponseVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "IP: ((client_ip))";
        const string ip = "11.22.33.44";

        const string expectedResult = $"IP: {ip}";

        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();
        var handler = _mocker.CreateInstance<ClientIpResponseVariableParsingHandler>();

        clientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = handler.Parse(input, matches, new StubModel());

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
