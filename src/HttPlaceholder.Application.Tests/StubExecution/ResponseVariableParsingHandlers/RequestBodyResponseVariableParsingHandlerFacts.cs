using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers
{
    [TestClass]
    public class RequestBodyResponseVariableParsingHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private RequestBodyResponseVariableParsingHandler _parsingHandler;

        [TestInitialize]
        public void Initialize() => _parsingHandler = new RequestBodyResponseVariableParsingHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void RequestBodyVariableHandler_Parse_HappyFlow()
        {
            // arrange
            const string input = "Posted content: ((request_body))";
            const string body = "POSTED BODY";

            const string expectedResult = "Posted content: POSTED BODY";

            _httpContextServiceMock
                .Setup(m => m.GetBody())
                .Returns(body);

            // act
            var matches = ResponseVariableParser.VarRegex.Matches(input);
            var result = _parsingHandler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
