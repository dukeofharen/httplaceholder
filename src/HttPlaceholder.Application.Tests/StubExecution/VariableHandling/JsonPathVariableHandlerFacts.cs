using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class JsonPathVariableHandlerFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestMethod]
        public void Parse_NoMatches_ShouldReturnInputAsIs()
        {
            // Arrange
            const string input = "input";
            var handler = _mocker.CreateInstance<JsonPathVariableHandler>();

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = handler.Parse(input, matches);

            // Assert
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Parse_HasMatches_JsonIsCorrupt_ShouldReplaceVariablesWithEmptyString()
        {
            // Arrange
            const string input = "((jsonpath:$.values[0].title)) ((jsonpath:$.values[1].title))";

            var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
            mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns("wrong json");

            var handler = _mocker.CreateInstance<JsonPathVariableHandler>();

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = handler.Parse(input, matches);

            // Assert
            Assert.AreEqual(" ", result);
        }

        [TestMethod]
        public void Parse_HasMatches_JsonIsOk_ShouldParseInput()
        {
            // Arrange
            const string input = "((jsonpath:$.values[1].title)) ((jsonpath:$.values[0].title))";

            var mockHttpContextService = _mocker.GetMock<IHttpContextService>();
            mockHttpContextService
                .Setup(m => m.GetBody())
                .Returns(@"{
    ""values"": [
        {
            ""title"": ""Value1""
        },
        {
            ""title"": ""Value2""
        }
    ]
}");

            var handler = _mocker.CreateInstance<JsonPathVariableHandler>();

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = handler.Parse(input, matches);

            // Assert
            Assert.AreEqual("Value2 Value1", result);
        }
    }
}
