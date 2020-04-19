using System;
using System.Globalization;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using HttPlaceholder.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class LocalNowVariableHandlerFacts
    {
        private static readonly DateTime Now = new DateTime(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
        private readonly Mock<IDateTime> _dateTimeMock = new Mock<IDateTime>();
        private LocalNowVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            _dateTimeMock
                .Setup(m => m.Now)
                .Returns(Now);

            _handler = new LocalNowVariableHandler(_dateTimeMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dateTimeMock.VerifyAll();
        }

        [TestMethod]
        public void LocalNowVariableHandler_Parse_HappyFlow_FormatSet()
        {
            // Arrange
            var input = "((localnow:dd-MM-yyyy HH:mm:ss))";

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // Assert
            Assert.AreEqual("21-08-2019 20:29:17", result);
        }

        [TestMethod]
        public void LocalNowVariableHandler_Parse_HappyFlow_NoFormatSet()
        {
            // Arrange
            var input = "((localnow))";

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            var result = _handler.Parse(input, matches);

            // Assert
            Assert.AreEqual(Now.ToString(CultureInfo.InvariantCulture), result);
        }
    }
}
