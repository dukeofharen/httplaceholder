using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.Stubs.Commands;

[TestClass]
public class UpdateStubCommandHandlerFacts
{
    private readonly Mock<IStubContext> _mockStubContext = new();
    private readonly Mock<IStubModelValidator> _mockStubModelValidator = new();
    private UpdateStubCommandHandler _handler;

    [TestInitialize]
    public void Initialize() =>
        _handler = new UpdateStubCommandHandler(_mockStubContext.Object, _mockStubModelValidator.Object);

    [TestCleanup]
    public void Cleanup()
    {
        _mockStubContext.VerifyAll();
        _mockStubModelValidator.VerifyAll();
    }

    [TestMethod]
    public async Task Handle_HasValidationErrors_ShouldThrowValidationException()
    {
        // Arrange
        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        var errors = new[] {"error1"};

        _mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(errors);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual(errors.Single(), exception.ValidationErrors.Single());
        _mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id), Times.Never);
        _mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId), Times.Never);
        _mockStubContext.Verify(m => m.AddStubAsync(stub), Times.Never);
    }

    [TestMethod]
    public async Task Handle_NoValidationErrors_ShouldUpdateStub()
    {
        // Arrange
        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        var errors = Array.Empty<string>();

        _mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(errors);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id), Times.Once);
        _mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId), Times.Once);
        _mockStubContext.Verify(m => m.AddStubAsync(stub), Times.Once);
    }
}