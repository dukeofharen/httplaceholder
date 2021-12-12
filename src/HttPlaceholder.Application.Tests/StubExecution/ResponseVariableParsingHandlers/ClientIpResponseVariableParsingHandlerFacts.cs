using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class ClientIpResponseVariableParsingHandlerFacts
{
    private readonly Mock<IClientDataResolver> _clientIpResolverMock = new();
    private ClientIpResponseVariableParsingHandler _parsingHandler;

    [TestInitialize]
    public void Initialize() => _parsingHandler = new ClientIpResponseVariableParsingHandler(_clientIpResolverMock.Object);

    [TestCleanup]
    public void Cleanup() => _clientIpResolverMock.VerifyAll();

    [TestMethod]
    public void RequestBodyVariableHandler_Parse_HappyFlow()
    {
        // arrange
        const string input = "IP: ((client_ip))";
        const string ip = "11.22.33.44";

        const string expectedResult = $"IP: {ip}";

        _clientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        // act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = _parsingHandler.Parse(input, matches);

        // assert
        Assert.AreEqual(expectedResult, result);
    }
}
