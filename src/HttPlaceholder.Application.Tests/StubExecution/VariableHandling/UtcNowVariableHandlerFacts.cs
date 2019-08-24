using System;
using System.Globalization;
using HttPlaceholder.Application.StubExecution.VariableHandling.Implementations;
using HttPlaceholder.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.VariableHandling
{
    [TestClass]
    public class UtcNowVariableHandlerFacts
    {
        private static DateTime _now = new DateTime(2019, 8, 21, 20, 29, 17, DateTimeKind.Local);
        private readonly Mock<IDateTime> _dateTimeMock = new Mock<IDateTime>();
        private UtcNowVariableHandler _handler;

        [TestInitialize]
        public void Initialize()
        {
            var now = new DateTime(2019, 8, 21, 20, 29, 17, DateTimeKind.Utc);
            _dateTimeMock
                .Setup(m => m.UtcNow)
                .Returns(now);

            _handler = new UtcNowVariableHandler(_dateTimeMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dateTimeMock.VerifyAll();
        }

        [TestMethod]
        public void UtcNowVariableHandler_Parse_HappyFlow_FormatSet()
        {
            // Arrange
            string input = "((utcnow:dd-MM-yyyy HH:mm:ss))";

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // Assert
            Assert.AreEqual("21-08-2019 20:29:17", result);
        }

        [TestMethod]
        public void UtcNowVariableHandler_Parse_HappyFlow_NoFormatSet()
        {
            // Arrange
            string input = "((utcnow))";

            // Act
            var matches = VariableParser.VarRegex.Matches(input);
            string result = _handler.Parse(input, matches);

            // Assert
            Assert.AreEqual(_now.ToString(CultureInfo.InvariantCulture), result);
        }
    }
}
