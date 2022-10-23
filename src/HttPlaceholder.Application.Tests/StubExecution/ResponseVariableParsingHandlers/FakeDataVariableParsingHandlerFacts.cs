using System.IO;
using System.Linq;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using Match = System.Text.RegularExpressions.Match;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseVariableParsingHandlers;

[TestClass]
public class FakeDataVariableParsingHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [DataTestMethod]
    [DataRow("", "", "", "")]
    [DataRow("first_name", "first_name", "", "")]
    [DataRow("en_US:first_name", "first_name", "en_US", "")]
    [DataRow("past:yyyy-MM-dd HH:mm:ss", "past", "", "yyyy-MM-dd HH:mm:ss")]
    [DataRow("words:3", "words", "", "3")]
    [DataRow("en_US:past:yyyy-MM-dd HH:mm:ss", "past", "en_US", "yyyy-MM-dd HH:mm:ss")]
    public void ParseFakeDataInput_HappyFlow(
        string input,
        string expectedGenerator,
        string expectedLocale,
        string expectedFormatting)
    {
        // Arrange
        var locales = new[] {"en_US"};
        var fakerServiceMock = _mocker.GetMock<IFakerService>();
        fakerServiceMock
            .Setup(m => m.GetLocales())
            .Returns(locales);

        var handler = _mocker.CreateInstance<FakeDataVariableParsingHandler>();

        // Act
        var result = handler.ParseFakeDataInput(input);

        // Assert
        Assert.AreEqual(expectedGenerator, result.generator);
        Assert.AreEqual(expectedLocale, result.locale);
        Assert.AreEqual(expectedFormatting, result.formatting);
    }

    [TestMethod]
    public void GetDescription_HappyFlow()
    {
        // Arrange
        var locales = new[] {"en_US", "nl"};
        const string description = "the description [LOCALES]";
        const string expectedDescription = "the description _en_US_, _nl_";

        var fileServiceMock = _mocker.GetMock<IFileService>();
        var fakerServiceMock = _mocker.GetMock<IFakerService>();
        var handler = _mocker.CreateInstance<FakeDataVariableParsingHandler>();

        fileServiceMock
            .Setup(m => m.ReadAllText(It.Is<string>(p =>
                p.EndsWith(Path.Combine("Files", "VarParser", "fake_data-description.md")))))
            .Returns(description);

        fakerServiceMock
            .Setup(m => m.GetLocales())
            .Returns(locales);

        // Act
        var result = handler.GetDescription();

        // Assert
        Assert.AreEqual(expectedDescription, result);

        // Act
        result = handler.GetDescription();

        // Assert
        Assert.AreEqual(expectedDescription, result);
        fileServiceMock.Verify(m => m.ReadAllText(It.IsAny<string>()), Times.Once);
    }

    [TestMethod]
    public void Examples_HappyFlow()
    {
        // Arrange
        var locales = new[] {"en_US", "nl"};
        var generators = new[]
        {
            new FakeDataGeneratorModel("gen1", (_, _, _) => string.Empty),
            new FakeDataGeneratorModel("gen2", "4", (_, _, _) => string.Empty)
        };

        var expectedExamples = new[]
        {
            "((fake_data:en_US:first_name))", "((fake_data:nl:first_name))", "((fake_data:gen1))",
            "((fake_data:gen2))", "((fake_data:gen2:4))"
        };

        var fakerServiceMock = _mocker.GetMock<IFakerService>();
        var handler = _mocker.CreateInstance<FakeDataVariableParsingHandler>();

        fakerServiceMock
            .Setup(m => m.GetLocales())
            .Returns(locales);

        fakerServiceMock
            .Setup(m => m.GetGenerators())
            .Returns(generators);

        // Act
        var result = handler.Examples;

        // Assert
        Assert.AreEqual(expectedExamples.Length, result.Length);
        foreach (var expectedExample in expectedExamples)
        {
            Assert.IsTrue(result.Contains(expectedExample));
        }

        // Act
        var secondResult = handler.Examples;

        // Assert
        Assert.AreEqual(result, secondResult);
        fakerServiceMock.Verify(m => m.GetLocales(), Times.Once);
    }

    [TestMethod]
    public async Task Parse_NoMatches_ShouldReturnInputAsIs()
    {
        // Arrange
        var handler = _mocker.CreateInstance<FakeDataVariableParsingHandler>();
        const string input = "the input";

        // Act
        var result = await handler.ParseAsync(input, Array.Empty<Match>(), new StubModel(), CancellationToken.None);

        // Assert
        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public async Task Parse_Matches_HappyFlow()
    {
        // Arrange
        var locales = new[] {"en_US", "nl"};

        var fakerServiceMock = _mocker.GetMock<IFakerService>();
        fakerServiceMock
            .Setup(m => m.GetLocales())
            .Returns(locales);

        var handler = _mocker.CreateInstance<FakeDataVariableParsingHandler>();
        const string input = "((fake_data:gen1)) ((fake_data:nl:gen2:5))";
        const string expectedOutput = "fake1 fake2";

        var stubModel = new StubModel {Scenario = "stub-scenario"};

        fakerServiceMock
            .Setup(m => m.GenerateFakeData("gen1", string.Empty, string.Empty))
            .Returns("fake1");
        fakerServiceMock
            .Setup(m => m.GenerateFakeData("gen2", "nl", "5"))
            .Returns("fake2");

        // Act
        var matches = ResponseVariableParser.VarRegex.Matches(input);
        var result = await handler.ParseAsync(input, matches, stubModel, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedOutput, result);
    }
}
