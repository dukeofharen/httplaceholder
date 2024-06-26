﻿using System.Linq;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Commands;

namespace HttPlaceholder.Application.Tests.Stubs.Commands;

[TestClass]
public class AddStubCommandHandlerFacts
{
    private readonly Mock<IStubContext> _mockStubContext = new();
    private readonly Mock<IStubModelValidator> _mockStubModelValidator = new();
    private AddStubCommandHandler _handler;

    [TestInitialize]
    public void Initialize() =>
        _handler = new AddStubCommandHandler(_mockStubContext.Object, _mockStubModelValidator.Object);

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
        var stub = new StubModel { Id = "stub1" };
        var request = new AddStubCommand(stub);

        var errors = new[] { "error1" };

        _mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(errors);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual(errors.Single(), exception.ValidationErrors.Single());
        _mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id, It.IsAny<CancellationToken>()), Times.Never);
        _mockStubContext.Verify(m => m.AddStubAsync(stub, It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_NoValidationErrors_ShouldAddStub()
    {
        // Arrange
        var stub = new StubModel { Id = "stub1" };
        var request = new AddStubCommand(stub);

        var errors = Array.Empty<string>();

        _mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(errors);

        var fullStub = new FullStubModel();
        _mockStubContext
            .Setup(m => m.AddStubAsync(stub, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fullStub);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.AreEqual(fullStub, result);
        _mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mockStubContext.Verify(m => m.AddStubAsync(stub, It.IsAny<CancellationToken>()), Times.Once);
    }
}
