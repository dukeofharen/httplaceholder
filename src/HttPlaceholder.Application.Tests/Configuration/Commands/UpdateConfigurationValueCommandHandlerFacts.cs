using HttPlaceholder.Application.Configuration.Commands.UpdateConfigurationValue;
using HttPlaceholder.Application.Configuration.Provider;
using HttPlaceholder.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HttPlaceholder.Application.Tests.Configuration.Commands;

[TestClass]
public class UpdateConfigurationValueCommandHandlerFacts
{
    private readonly Mock<IConfigurationRoot> _configurationRootMock = new();
    private readonly AutoMocker _mocker = new();
    private readonly HttPlaceholderConfigurationProvider _provider = new(new MemoryConfigurationSource());

    [TestInitialize]
    public void Initialize() => _mocker.Use<IConfiguration>(_configurationRootMock.Object);

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_MetadataNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<UpdateConfigurationValueCommandHandler>();
        var request = new UpdateConfigurationValueCommand("notFound", string.Empty);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("Configuration value with key"));
    }

    [TestMethod]
    public async Task Handle_ValueCantBeMutated_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<UpdateConfigurationValueCommandHandler>();
        var request = new UpdateConfigurationValueCommand("httpsPort", "something");

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("can not be mutated at this moment."));
    }

    [TestMethod]
    public async Task Handle_MetadataIsBooleanButValueIsNotBoolean_ShouldThrowArgumentException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<UpdateConfigurationValueCommandHandler>();
        var request = new UpdateConfigurationValueCommand("storeResponses", "something");

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("is of type boolean, but no boolean value was passed."));
    }

    [TestMethod]
    public async Task Handle_NoConfigProvider_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<UpdateConfigurationValueCommandHandler>();
        var request = new UpdateConfigurationValueCommand("storeResponses", "true");

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.IsTrue(exception.Message.Contains("unexpectedly not found."));
    }

    [DataTestMethod]
    [DataRow("true")]
    [DataRow("false")]
    [DataRow("TRUE")]
    [DataRow("FALSE")]
    public async Task Handle_HappyFlow(string value)
    {
        // Arrange
        var handler = _mocker.CreateInstance<UpdateConfigurationValueCommandHandler>();
        var request = new UpdateConfigurationValueCommand("storeResponses", value);

        _configurationRootMock
            .Setup(m => m.Providers)
            .Returns(new[] {_provider});

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsTrue(_provider.TryGet("Storage:StoreResponses", out var foundValue));
        Assert.AreEqual(value, foundValue);
    }
}
