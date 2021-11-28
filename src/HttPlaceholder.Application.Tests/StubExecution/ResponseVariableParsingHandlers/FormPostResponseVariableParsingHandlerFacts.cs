using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandler;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers
{
    [TestClass]
    public class FormPostResponseVariableParsingHandlerFacts
    {
        private readonly Mock<IHttpContextService> _httpContextServiceMock = new Mock<IHttpContextService>();
        private FormPostResponseVariableParsingHandler _parsingHandler;

        [TestInitialize]
        public void Initialize() => _parsingHandler = new FormPostResponseVariableParsingHandler(_httpContextServiceMock.Object);

        [TestCleanup]
        public void Cleanup() => _httpContextServiceMock.VerifyAll();

        [TestMethod]
        public void FormPostVariableHandler_Parse_HappyFlow()
        {
            // arrange
            const string input = "Form var 1: ((form_post:var1)), Form var 2: ((form_post:var2)), Form var 3: ((form_post:var3))";

            var formTuples = new[]
            {
                ("var1", new StringValues("https://google.com")),
                ("var3", new StringValues("value3"))
            };

            const string expectedResult = "Form var 1: https://google.com, Form var 2: , Form var 3: value3";

            _httpContextServiceMock
                .Setup(m => m.GetFormValues())
                .Returns(formTuples);

            // act
            var matches = ResponseVariableParser.VarRegex.Matches(input);
            var result = _parsingHandler.Parse(input, matches);

            // assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
