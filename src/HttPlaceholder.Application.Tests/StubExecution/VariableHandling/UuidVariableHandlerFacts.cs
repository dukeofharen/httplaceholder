using System;
using System.Linq;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class UuidVariableHandlerFacts
    {
        private readonly UuidVariableHandler _handler = new UuidVariableHandler();

        [TestMethod]
        public void UuidVariableHandler_Parse_HappyFlow()
        {
            // arrange
            var input = "((uuid)) ((uuid:nonsense))";

            // act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // assert
            var parts = result.Split(' ');
            Assert.AreEqual(2, parts.Length);
            Assert.IsTrue(parts.All(p => Guid.TryParse(p, out _)));
            Assert.AreNotEqual(parts[0], parts[1]);
        }
    }
}
