using System.Collections.Generic;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class EncodedQueryStringHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private EncodedQueryStringVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new EncodedQueryStringVariableHandler(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void EncodedQueryStringHandlerFacts_Parse_HappyFlow()
        {
            // arrange
            string input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
            var queryDict = new Dictionary<string, string>
            {
                { "var1", "https://google.com" },
                { "var3", "value3" },
            };
            string expectedResult = "Query var 1: https%3A%2F%2Fgoogle.com, query var 2: , query var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetQueryStringDictionary())
                .Returns(queryDict);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
