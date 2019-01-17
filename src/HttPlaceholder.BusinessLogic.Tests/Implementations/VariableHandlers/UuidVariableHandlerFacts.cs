using System;
using System.Linq;
using HttPlaceholder.BusinessLogic.Implementations.VariableHandlers;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.VariableHandlers
{
    [TestClass]
    public class UuidVariableHandlerFacts
    {
        private readonly UuidVariableHandler _handler = new UuidVariableHandler();

        [TestMethod]
        public void UuidVariableHandler_Parse_HappyFlow()
        {
            // arrange
            string input = "((uuid)) ((uuid:nonsense))";

            // act
            var matches = Constants.Regexes.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // assert
            var parts = result.Split(' ');
            Assert.AreEqual(2, parts.Length);
            Assert.IsTrue(parts.All(p => Guid.TryParse(p, out _)));
            Assert.AreNotEqual(parts[0], parts[1]);
        }
    }
}
