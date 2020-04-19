using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class DisplayUrlVariableHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private DisplayUrlVariableHandler _handler;

        [TestInitialize]
        public void Initialize() => _handler = new DisplayUrlVariableHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            const string input = "URL: ((display_url))";
            const string url = "http://localhost:5000/test.txt?var1=value1&var2=value2";

            var expectedResult = $"URL: {url}";

            _httpContextServiceMock
                .Setup(m => m.DisplayUrl)
                .Returns(url);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
