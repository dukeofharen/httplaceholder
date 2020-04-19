using HttPlaceholder.Application.Interfaces.Http;
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
        public void Initialize() => _handler = new RequestBodyVariableHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            var input = "Posted content: ((request_body))";
            var body = "POSTED BODY";

            var expectedResult = "Posted content: POSTED BODY";

            _httpContextServiceMock
                .Setup(m => m.GetBody())
                .Returns(body);

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
