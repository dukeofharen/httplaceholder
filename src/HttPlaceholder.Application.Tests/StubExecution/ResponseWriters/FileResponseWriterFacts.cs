using System.IO;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common;
using HttPlaceholder.TestUtilities.Options;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class FileResponseWriterFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly SettingsModel _settings = MockSettingsFactory.GetSettings();

    [TestInitialize]
    public void Initialize() =>
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
    {
        // Arrange
        var writer = _mocker.CreateInstance<FileResponseWriter>();
        var stub = new StubModel {Response = new StubResponseModel {File = null}};

        var response = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task FileResponseWriter_WriteToResponseAsync_FileFoundDirectly_NotAllowed()
    {
        // Arrange
        _settings.Stub.AllowGlobalFileSearch = false;
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var writer = _mocker.CreateInstance<FileResponseWriter>();

        var stub = new StubModel {Response = new StubResponseModel {File = @"C:\tmp\image.png"}};

        var response = new ResponseModel();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(stub.Response.File, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            writer.WriteToResponseAsync(stub, response, CancellationToken.None));

        // Assert
        Assert.AreEqual("Path 'C:\\tmp\\image.png' found, but can't be used because setting 'allowGlobalFileSearch' is turned off. Turn it on with caution.", exception.Message);
    }

    [TestMethod]
    public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileFoundDirectly()
    {
        // Arrange
        _settings.Stub.AllowGlobalFileSearch = true;
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var writer = _mocker.CreateInstance<FileResponseWriter>();

        var body = new byte[] {1, 2, 3};
        var stub = new StubModel {Response = new StubResponseModel {File = @"C:\tmp\image.png"}};

        var response = new ResponseModel();

        fileServiceMock
            .Setup(m => m.FileExistsAsync(stub.Response.File, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        fileServiceMock
            .Setup(m => m.ReadAllBytesAsync(stub.Response.File, It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(body, response.Body);
    }

    [DataTestMethod]
    [DataRow("image.png", "image.png")]
    [DataRow("../image.png", "image.png")]
    [DataRow("../../image.png", "image.png")]
    public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileNotFoundDirectly_ButFoundInStubFolder(string file, string actualFile)
    {
        // Arrange
        var stubRootPathResolverMock = _mocker.GetMock<IStubRootPathResolver>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var writer = _mocker.CreateInstance<FileResponseWriter>();

        var stubRootPaths = new[] {"/var/stubs1", "/var/stubs2"};
        var expectedPath = Path.Combine(stubRootPaths[1], actualFile);
        var body = new byte[] {1, 2, 3};
        var stub = new StubModel {Response = new StubResponseModel {File = file}};

        var response = new ResponseModel();

        stubRootPathResolverMock
            .Setup(m => m.GetStubRootPathsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubRootPaths);

        fileServiceMock
            .Setup(m => m.FileExistsAsync(file, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        fileServiceMock
            .Setup(m => m.ReadAllBytesAsync(expectedPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(body, response.Body);
    }

    [TestMethod]
    public async Task
        FileResponseWriter_WriteToResponseAsync_FileNotFoundDirectly_AlsoNotFoundInStubFolder_ShouldReturnNoBody()
    {
        // Arrange
        var stubRootPathResolverMock = _mocker.GetMock<IStubRootPathResolver>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var writer = _mocker.CreateInstance<FileResponseWriter>();

        const string file = "image.png";
        var stubRootPaths = new[] {"/var/stubs1", "/var/stubs2"};
        var expectedFolder = Path.Combine(stubRootPaths[0], file);
        var stub = new StubModel {Response = new StubResponseModel {File = file}};

        var response = new ResponseModel();

        stubRootPathResolverMock
            .Setup(m => m.GetStubRootPathsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stubRootPaths);

        fileServiceMock
            .Setup(m => m.FileExistsAsync(stub.Response.File, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedFolder, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }
}
