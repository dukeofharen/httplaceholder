using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.BusinessLogic.Implementations.VariableHandlers;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.VariableHandlers
{
    [TestClass]
    public class DisplayUrlVariableHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private DisplayUrlVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new DisplayUrlVariableHandler(_httpContextServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
        }

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            string input = "URL: ((display_url))";
            string url = "http://localhost:5000/test.txt?var1=value1&var2=value2";

            string expectedResult = $"URL: {url}";

            _httpContextServiceMock
                .Setup(m => m.DisplayUrl)
                .Returns(url);

            // act
            var matches = Constants.Regexes.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
