using System.IO;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations.OpenApiStubGeneratorFacts;

[TestClass]
public class OpenApiPetstoreFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task PetstoreOpenApi_HappyFlow()
    {
        // Arrange
        var generator = _mocker.CreateInstance<OpenApiStubGenerator>();
        var openapi = await File.ReadAllTextAsync("Resources/OpenAPI/petstore.yaml");

        // Act
        var stubs = await generator.GenerateOpenApiStubs(openapi, false);
    }
}
