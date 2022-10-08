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
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Stubs.Commands;

[TestClass]
public class UpdateStubCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_HasValidationErrors_ShouldThrowValidationException()
    {
        // Arrange
        var mockStubModelValidator = _mocker.GetMock<IStubModelValidator>();
        var mockStubContext = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<UpdateStubCommandHandler>();

        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        var errors = new[] {"error1"};

        mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(errors);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual(errors.Single(), exception.ValidationErrors.Single());
        mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id, It.IsAny<CancellationToken>()), Times.Never);
        mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId, It.IsAny<CancellationToken>()), Times.Never);
        mockStubContext.Verify(m => m.AddStubAsync(stub, It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_OldStubDoesNotExist_ShouldThrowValidationException()
    {
        // Arrange
        var mockStubModelValidator = _mocker.GetMock<IStubModelValidator>();
        var mockStubContext = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<UpdateStubCommandHandler>();

        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(Array.Empty<string>());
        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FullStubModel) null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                handler.Handle(request, CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_OldStubIsReadonly_ShouldThrowValidationException()
    {
        // Arrange
        var mockStubModelValidator = _mocker.GetMock<IStubModelValidator>();
        var mockStubContext = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<UpdateStubCommandHandler>();

        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(Array.Empty<string>());

        var oldStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = false}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldStub);

        var newStub = new FullStubModel
        {
            Stub = stub, Metadata = new StubMetadataModel {ReadOnly = true}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(stub.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newStub);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        var errors = exception.ValidationErrors.ToArray();
        Assert.AreEqual(1, errors.Length);
        Assert.AreEqual("Stub with ID 'stub1' is read-only; it can not be updated through the API.", errors.Single());
    }

    [TestMethod]
    public async Task Handle_NewStubIsReadonly_ShouldThrowValidationException()
    {
        // Arrange
        var mockStubModelValidator = _mocker.GetMock<IStubModelValidator>();
        var mockStubContext = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<UpdateStubCommandHandler>();

        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(Array.Empty<string>());

        var oldStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = true}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldStub);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        var errors = exception.ValidationErrors.ToArray();
        Assert.AreEqual(1, errors.Length);
        Assert.AreEqual("Stub with ID 'new-stub-id' is read-only; it can not be updated through the API.", errors.Single());
    }

    [TestMethod]
    public async Task Handle_NoValidationErrors_ShouldUpdateStub()
    {
        // Arrange
        var mockStubModelValidator = _mocker.GetMock<IStubModelValidator>();
        var mockStubContext = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<UpdateStubCommandHandler>();

        var stub = new StubModel {Id = "stub1"};
        var request = new UpdateStubCommand("new-stub-id", stub);

        mockStubModelValidator
            .Setup(m => m.ValidateStubModel(stub))
            .Returns(Array.Empty<string>());

        var previousStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = false}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(request.StubId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousStub);

        var existingStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = false}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(stub.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStub);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id, It.IsAny<CancellationToken>()), Times.Once);
        mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId, It.IsAny<CancellationToken>()), Times.Once);
        mockStubContext.Verify(m => m.AddStubAsync(stub, It.IsAny<CancellationToken>()), Times.Once);
    }
}
