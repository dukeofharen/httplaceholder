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
        mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id), Times.Never);
        mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId), Times.Never);
        mockStubContext.Verify(m => m.AddStubAsync(stub), Times.Never);
    }

    [TestMethod]
    public async Task Handle_StubIsReadonly_ShouldThrowValidationException()
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

        var existingStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = true}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(stub.Id))
            .ReturnsAsync(existingStub);

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

        var existingStub = new FullStubModel
        {
            Stub = new StubModel(), Metadata = new StubMetadataModel {ReadOnly = false}
        };
        mockStubContext
            .Setup(m => m.GetStubAsync(stub.Id))
            .ReturnsAsync(existingStub);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        mockStubContext.Verify(m => m.DeleteStubAsync(stub.Id), Times.Once);
        mockStubContext.Verify(m => m.DeleteStubAsync(request.StubId), Times.Once);
        mockStubContext.Verify(m => m.AddStubAsync(stub), Times.Once);
    }
}
