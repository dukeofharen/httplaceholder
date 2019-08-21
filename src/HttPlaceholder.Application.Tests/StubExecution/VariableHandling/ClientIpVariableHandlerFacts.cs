using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class ClientIpVariableHandlerFacts
    {
        private readonly Mock<IClientIpResolver> _clientIpResolverMock = new Mock<IClientIpResolver>();
        private ClientIpVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new ClientIpVariableHandler(_clientIpResolverMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _clientIpResolverMock.VerifyAll();
        }

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            var input = "IP: ((client_ip))";
            var ip = "11.22.33.44";

            var expectedResult = $"IP: {ip}";

            _clientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns(ip);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
