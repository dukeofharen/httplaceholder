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
        // arrange
        var writer = _mocker.CreateInstance<FileResponseWriter>();
        var stub = new StubModel {Response = new StubResponseModel {File = null}};

        var response = new ResponseModel();

        // act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }

    [TestMethod]
    public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileFoundDirectly()
    {
        // arrange
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

        // act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(body, response.Body);
    }

    [TestMethod]
    public async Task FileResponseWriter_WriteToResponseAsync_HappyFlow_FileNotFoundDirectly_ButFoundInStubFolder()
    {
        // arrange
        var stubRootPathResolverMock = _mocker.GetMock<IStubRootPathResolver>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var writer = _mocker.CreateInstance<FileResponseWriter>();

        var stubRootPaths = new[] {"/var/stubs1", "/var/stubs2"};
        const string file = "image.png";
        var expectedFolder = Path.Combine(stubRootPaths[1], file);
        var body = new byte[] {1, 2, 3};
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
            .ReturnsAsync(true);

        fileServiceMock
            .Setup(m => m.ReadAllBytesAsync(expectedFolder, It.IsAny<CancellationToken>()))
            .ReturnsAsync(body);

        // act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(body, response.Body);
    }

    [TestMethod]
    public async Task
        FileResponseWriter_WriteToResponseAsync_FileNotFoundDirectly_AlsoNotFoundInStubFolder_ShouldReturnNoBody()
    {
        // arrange
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

        // act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
    }
}
