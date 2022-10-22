using System.IO;
using System.Linq;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

namespace HttPlaceholder.Application.Tests.StubExecution.OpenAPIParsing;

[TestClass]
public class OpenApiParserFacts
{
    private readonly OpenApiParser _parser = new();

    [TestMethod]
    public async Task ParseOpenApiDefinition_HappyFlow()
    {
        // Arrange
        var input = await File.ReadAllTextAsync("Resources/OpenAPI/petstore.yaml");

        // Act
        var result = _parser.ParseOpenApiDefinition(input);

        // Assert
        Assert.AreEqual("http://petstore.swagger.io/v1", result.Server.Url);

        var lines = result.Lines.ToArray();
        Assert.AreEqual(6, lines.Length);
    }
}
