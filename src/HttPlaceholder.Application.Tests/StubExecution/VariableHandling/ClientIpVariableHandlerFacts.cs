﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class ClientIpVariableHandlerFacts
    {
        private readonly Mock<IClientDataResolver> _clientIpResolverMock = new Mock<IClientDataResolver>();
        private ClientIpVariableHandler _handler;

        [TestInitialize]
        public void Initialize() => _handler = new ClientIpVariableHandler(_clientIpResolverMock.Object);

        [TestCleanup]
        public void Cleanup() => _clientIpResolverMock.VerifyAll();

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            const string input = "IP: ((client_ip))";
            const string ip = "11.22.33.44";

            var expectedResult = $"IP: {ip}";

            _clientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns(ip);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
