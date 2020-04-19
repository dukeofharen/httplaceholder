using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class VariableParserFacts
    {
        private readonly Mock<IVariableHandler> _handler1 = new Mock<IVariableHandler>();
        private readonly Mock<IVariableHandler> _handler2 = new Mock<IVariableHandler>();
        private VariableParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new VariableParser(new[]
            {
                _handler1.Object,
                _handler2.Object
            });

            _handler1
                .Setup(m => m.Name)
                .Returns("handler1");
            _handler2
                .Setup(m => m.Name)
                .Returns("handler2");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _handler1.VerifyAll();
            _handler2.VerifyAll();
        }

        [TestMethod]
        public void VaribaleParser_Parse_HappyFlow()
        {
            // arange
            const string input = @"((handler1:value1)) ((handler2))
((handler1:bla))
((handler-x))";

            _handler1
                .Setup(m =>
                m.Parse(input, It.Is<IEnumerable<Match>>(matches => matches.Any(match => match.Groups[2].Value == "value1" || match.Groups[2].Value == "bla"))))
                .Returns<string, IEnumerable<Match>>((r, m) => r);
            _handler2
               .Setup(m =>
               m.Parse(input, It.Is<IEnumerable<Match>>(matches => matches.Any(match => string.IsNullOrWhiteSpace(match.Groups[2].Value)))))
               .Returns<string, IEnumerable<Match>>((r, m) => r);

            // act
            var result = _parser.Parse(input);

            // assert
            Assert.AreEqual(input, result);
            _handler1.Verify(m => m.Parse(input, It.IsAny<IEnumerable<Match>>()), Times.Once);
            _handler2.Verify(m => m.Parse(input, It.IsAny<IEnumerable<Match>>()), Times.Once);
        }
    }
}
