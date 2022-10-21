using System;
using System.IO;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class BaseVariableParsingHandlerFacts
{
    [TestMethod]
    public void GetDescription_HappyFlow()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var handler = new TestVariableParsingHandler(fileServiceMock.Object);

        const string description = "the description";
        fileServiceMock
            .Setup(m => m.ReadAllText(It.Is<string>(p =>
                p.EndsWith(Path.Combine("Files", "VarParser", "TestVariableParsingHandler-description.md")))))
            .Returns(description);

        // Act
        var result = handler.GetDescription();

        // Assert
        Assert.AreEqual(description, result);

        // Act
        result = handler.GetDescription();

        // Assert
        Assert.AreEqual(description, result);
        fileServiceMock.Verify(m => m.ReadAllText(It.IsAny<string>()), Times.Once);
    }

    private class TestVariableParsingHandler : BaseVariableParsingHandler
    {
        public TestVariableParsingHandler(IFileService fileService) : base(fileService)
        {
        }

        public override string Name => "TestVariableParsingHandler";
        public override string FullName { get; }
        public override string[] Examples { get; }

        protected override string InsertVariables(string input, Match[] matches, StubModel stub) =>
            throw new NotImplementedException();
    }
}
