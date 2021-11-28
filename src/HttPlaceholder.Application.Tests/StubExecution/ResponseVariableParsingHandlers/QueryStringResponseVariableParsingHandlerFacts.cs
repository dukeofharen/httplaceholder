using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers
{
    [TestClass]
    public class QueryStringResponseVariableParsingHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private QueryStringResponseVariableParsingHandler _parsingHandler;

        [TestInitialize]
        public void Initialize() => _parsingHandler = new QueryStringResponseVariableParsingHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void QueryStringHandlerFacts_Parse_HappyFlow()
        {
            // arrange
            const string input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
            var queryDict = new Dictionary<string, string>
            {
                { "var1", "https://google.com" },
                { "var3", "value3" }
            };
            const string expectedResult = "Query var 1: https://google.com, query var 2: , query var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetQueryStringDictionary())
                .Returns(queryDict);

            // act
            var matches = ResponseVariableParser.VarRegex.Matches(input);
            var result = _parsingHandler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
