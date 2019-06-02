using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class RequestBodyVariableHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private RequestBodyVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _handler = new RequestBodyVariableHandler(_httpContextServiceMock.Object);
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
            string input = "Posted content: ((request_body))";
            string body = "POSTED BODY";

            string expectedResult = "Posted content: POSTED BODY";

            _httpContextServiceMock
                .Setup(m => m.GetBody())
                .Returns(body);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
