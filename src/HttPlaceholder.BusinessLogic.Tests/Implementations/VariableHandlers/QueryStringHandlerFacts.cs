using System.Collections.Generic;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.VariableHandlers;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.VariableHandlers
{
    [TestClass]
    public class QueryStringHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private QueryStringVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new QueryStringVariableHandler(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void QueryStringHandlerFacts_Parse_HappyFlow()
        {
            // arrange
            string input = "Query var 1: ((query:var1)), query var 2: ((query:var2)), query var 3: ((query:var3))";
            var queryDict = new Dictionary<string, string>
            {
                { "var1", "value1" },
                { "var3", "value3" },
            };
            string expectedResult = "Query var 1: value1, query var 2: , query var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetQueryStringDictionary())
                .Returns(queryDict);

            // act
            var matches = Constants.Regexes.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
