using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Export;
using HttPlaceholder.Application.Export.Queries.ExportRequest;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.Export.Queries;

[TestClass]
public class ExportRequestQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_RequestNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<ExportRequestQueryHandler>();

        const string requqestId = "abc123";

        stubContextMock
            .Setup(m => m.GetRequestResultAsync(requqestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RequestResultModel)null);

        // Act
        var exception = await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            handler.Handle(new ExportRequestQuery(requqestId, RequestExportType.Curl), CancellationToken.None));

        // Assert
        Assert.AreEqual("Entity \"request\" (abc123) was not found.", exception.Message);
    }

    [DataTestMethod]
    [DataRow(RequestExportType.Har)]
    [DataRow(RequestExportType.Hurl)]
    [DataRow(RequestExportType.NotSet)]
    public async Task Handle_RequestExportTypeNotSupported_ShouldThrowNotImplementedException(RequestExportType requestExportType)
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<ExportRequestQueryHandler>();

        const string requqestId = "abc123";

        stubContextMock
            .Setup(m => m.GetRequestResultAsync(requqestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RequestResultModel());

        // Act
        var exception = await Assert.ThrowsExceptionAsync<NotImplementedException>(() =>
            handler.Handle(new ExportRequestQuery(requqestId, requestExportType), CancellationToken.None));

        // Assert
        Assert.AreEqual($"Converting of request to {requestExportType} not supported.", exception.Message);
    }

    [TestMethod]
    public async Task Handle_ExportToCurl()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var requestToCurlCommandServiceMock = _mocker.GetMock<IRequestToCurlCommandService>();
        var handler = _mocker.CreateInstance<ExportRequestQueryHandler>();

        const string requqestId = "abc123";

        var requestResult = new RequestResultModel();
        stubContextMock
            .Setup(m => m.GetRequestResultAsync(requqestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requestResult);

        const string curlCommand = "curl command";
        requestToCurlCommandServiceMock
            .Setup(m => m.Convert(requestResult))
            .Returns(curlCommand);

        // Act
        var result = await handler.Handle(new ExportRequestQuery(requqestId, RequestExportType.Curl),
            CancellationToken.None);

        // Assert
        Assert.AreEqual(curlCommand, result);
    }
}
