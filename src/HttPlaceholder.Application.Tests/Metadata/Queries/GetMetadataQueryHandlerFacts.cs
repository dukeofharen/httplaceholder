using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Metadata.Queries;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.Tests.Metadata.Queries;

[TestClass]
public class GetMetadataQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HappyFlow()
    {
        // Arrange
        var assemblyServiceMock = _mocker.GetMock<IAssemblyService>();
        var serviceProviderMock = _mocker.GetMock<IServiceProvider>();
        var envServiceMock = _mocker.GetMock<IEnvService>();
        var handler = _mocker.CreateInstance<GetMetadataQueryHandler>();

        var mock1 = CreateHandlerMock("handler1", "Handler 1", "Example 1", "Description 1");
        var mock2 = CreateHandlerMock("handler2", "Handler 2", "Example 2", "Description 2");

        const string version = "1.2.3.4";
        assemblyServiceMock
            .Setup(m => m.GetAssemblyVersion())
            .Returns(version);

        serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IResponseVariableParsingHandler>)))
            .Returns(new[] { mock1.Object, mock2.Object });

        const string runtimeVersion = ".NET 8.0.0";
        envServiceMock
            .Setup(m => m.GetRuntime())
            .Returns(runtimeVersion);

        // Act
        var result = await handler.Handle(new GetMetadataQuery(), CancellationToken.None);

        // Assert
        Assert.AreEqual(version, result.Version);
        Assert.AreEqual(runtimeVersion, result.RuntimeVersion);

        var variableHandlers = result.VariableHandlers.ToArray();
        Assert.AreEqual(2, variableHandlers.Length);
        Assert.AreEqual("handler1", variableHandlers[0].Name);
        Assert.AreEqual("Handler 1", variableHandlers[0].FullName);
        Assert.AreEqual("Example 1", variableHandlers[0].Examples.Single());
        Assert.AreEqual("Description 1", variableHandlers[0].Description);
        Assert.AreEqual("handler2", variableHandlers[1].Name);
        Assert.AreEqual("Handler 2", variableHandlers[1].FullName);
        Assert.AreEqual("Example 2", variableHandlers[1].Examples.Single());
        Assert.AreEqual("Description 2", variableHandlers[1].Description);
    }

    private static Mock<IResponseVariableParsingHandler> CreateHandlerMock(string name, string fullName, string example,
        string description)
    {
        var mock = new Mock<IResponseVariableParsingHandler>();
        mock
            .Setup(m => m.Name)
            .Returns(name);
        mock
            .Setup(m => m.FullName)
            .Returns(fullName);
        mock
            .Setup(m => m.Examples)
            .Returns([example]);
        mock
            .Setup(m => m.GetDescription())
            .Returns(description);
        return mock;
    }
}
