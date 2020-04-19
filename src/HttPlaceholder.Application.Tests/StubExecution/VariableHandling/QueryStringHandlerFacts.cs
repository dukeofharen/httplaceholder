using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class QueryStringHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private QueryStringVariableHandler _handler;

        [TestInitialize]
        public void Initialize() => _handler = new QueryStringVariableHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void QueryStringHandlerFacts_Parse_HappyFlow()
        {
            // arrange
            var input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
            var queryDict = new Dictionary<string, string>
            {
                { "var1", "https://google.com" },
                { "var3", "value3" },
            };
            var expectedResult = "Query var 1: https://google.com, query var 2: , query var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetQueryStringDictionary())
                .Returns(queryDict);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
