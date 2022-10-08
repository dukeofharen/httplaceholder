using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.Stubs.Commands.AddStubs;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Stubs.Commands;

[TestClass]
public class AddStubsCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_StubsIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var request = new AddStubsCommand(null);

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual("No stubs posted.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_StubsIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var request = new AddStubsCommand(Array.Empty<StubModel>());

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual("No stubs posted.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_ValidationErrors_ShouldThrowValidationException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();

        var stub1 = new StubModel {Id = "stub1"};
        var stub2 = new StubModel {Id = ""};
        var request = new AddStubsCommand(new[] {stub1, stub2});

        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(stub1))
            .Returns(new[] {"ERROR1", "ERROR2"});
        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(stub2))
            .Returns(new[] {"ERROR3"});

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        var validationErrors = exception.ValidationErrors.ToArray();
        Assert.AreEqual(3, validationErrors.Length);

        Assert.AreEqual("stub1: ERROR1", validationErrors[0]);
        Assert.AreEqual("stub1: ERROR2", validationErrors[1]);
        Assert.AreEqual("ERROR3", validationErrors[2]);
    }

    [TestMethod]
    public async Task Handle_DuplicateStubs_ShouldThrowArgumentException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();

        var stub1 = new StubModel {Id = "stub1"};
        var stub2 = new StubModel {Id = "STub1"};
        var stub3 = new StubModel {Id = "stub2"};
        var stub4 = new StubModel {Id = "stuB2"};
        var request = new AddStubsCommand(new[] {stub1, stub2, stub3, stub4});

        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(It.IsAny<StubModel>()))
            .Returns(Array.Empty<string>());

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual("The following stub IDs are posted more than once: stub1, stub2", exception.Message);
    }

    [TestMethod]
    public async Task Handle_StubExistsInReadOnlySource_ShouldThrowValidationException()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        var stub1 = new StubModel {Id = "stub1"};
        var stub2 = new StubModel {Id = "stub2"};
        var request = new AddStubsCommand(new[] {stub1, stub2});

        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(It.IsAny<StubModel>()))
            .Returns(Array.Empty<string>());

        stubContextMock
            .Setup(m => m.GetStubsFromReadOnlySourcesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] {new FullStubModel {Stub = new StubModel {Id = stub2.Id.ToUpper()}}});

        // Act
        var exception = await Assert.ThrowsExceptionAsync<ValidationException>(() => handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.AreEqual("Validation failed:\nStub with ID already exists: STUB2", exception.Message);
    }

    [TestMethod]
    public async Task Handle_ShouldAddAndReturnStubsSuccessfully()
    {
        // Arrange
        var handler = _mocker.CreateInstance<AddStubsCommandHandler>();
        var stubModelValidatorMock = _mocker.GetMock<IStubModelValidator>();
        var stubContextMock = _mocker.GetMock<IStubContext>();

        var stub1 = new StubModel {Id = "stub1"};
        var stub2 = new StubModel {Id = "stub2"};
        var request = new AddStubsCommand(new[] {stub1, stub2});

        var stubResult1 = new FullStubModel();
        var stubResult2 = new FullStubModel();

        stubContextMock
            .Setup(m => m.AddStubAsync(stub1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResult1);
        stubContextMock
            .Setup(m => m.AddStubAsync(stub2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubResult2);

        stubModelValidatorMock
            .Setup(m => m.ValidateStubModel(It.IsAny<StubModel>()))
            .Returns(Array.Empty<string>());

        // Act
        var result = (await handler.Handle(request, CancellationToken.None)).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(stubResult1, result[0]);
        Assert.AreEqual(stubResult2, result[1]);
        stubContextMock.Verify(m => m.DeleteStubAsync(stub1.Id, It.IsAny<CancellationToken>()));
        stubContextMock.Verify(m => m.DeleteStubAsync(stub2.Id, It.IsAny<CancellationToken>()));
    }
}
