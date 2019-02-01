using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.VariableHandlers;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.VariableHandlers
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
            string input = "IP: ((client_ip))";
            string ip = "11.22.33.44";

            string expectedResult = $"IP: {ip}";

            _clientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns(ip);

            // act
            var matches = Constants.Regexes.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
