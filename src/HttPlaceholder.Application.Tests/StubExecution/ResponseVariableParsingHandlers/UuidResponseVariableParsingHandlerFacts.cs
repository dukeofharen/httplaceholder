using System;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers
{
    [TestClass]
    public class UuidResponseVariableParsingHandlerFacts
    {
        private readonly UuidResponseVariableParsingHandler _parsingHandler = new UuidResponseVariableParsingHandler();

        [TestMethod]
        public void UuidVariableHandler_Parse_HappyFlow()
        {
            // arrange
            const string input = "((uuid)) ((uuid:nonsense))";

            // act
            var matches = ResponseVariableParser.VarRegex.Matches(input);
            var result = _parsingHandler.Parse(input, matches);

            // assert
            var parts = result.Split(' ');
            Assert.AreEqual(2, parts.Length);
            Assert.IsTrue(parts.All(p => Guid.TryParse(p, out _)));
            Assert.AreNotEqual(parts[0], parts[1]);
        }
    }
}
