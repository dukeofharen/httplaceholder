using System.IO;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Enums;
using ImageMagick;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class ImageResponseWriterFacts
{
    private static readonly string _executingAssemblyPath =
        OperatingSystem.IsWindows() ? @"C:\bin\httplaceholder" : "/bin/httplaceholder";

    private static readonly string _tempFolder = OperatingSystem.IsWindows() ? @"C:\temp" : "/tmp";
    private readonly AutoMocker _mocker = new();

    [TestInitialize]
    public void Initialize()
    {
        _mocker.GetMock<IAssemblyService>()
            .Setup(m => m.GetExecutingAssemblyRootPath())
            .Returns(_executingAssemblyPath);
        _mocker.GetMock<IFileService>()
            .Setup(m => m.GetTempPath())
            .Returns(_tempFolder);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_StubNotConfigured_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = new StubModel { Response = new StubResponseModel { Image = null } };
        var response = new ResponseModel();
        var writer = _mocker.CreateInstance<ImageResponseWriter>();

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual(0, response.Headers.Count);
        Assert.AreEqual(null, response.Body);
        Assert.IsFalse(response.BodyIsBinary);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_FileIsCached_ShouldReturnCachedFile()
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Image = new StubResponseImageModel { Type = ResponseImageType.Jpeg, Height = 512, Width = 512 }
            }
        };
        var response = new ResponseModel();

        var cachedBytes = new byte[] { 1, 2, 3, 4 };
        var expectedCachePath = Path.Combine(_tempFolder, $"{stub.Response.Image.Hash}.bin");
        var writer = _mocker.CreateInstance<ImageResponseWriter>();
        var fileServiceMock = _mocker.GetMock<IFileService>();
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedCachePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileServiceMock
            .Setup(m => m.ReadAllBytesAsync(expectedCachePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.AreEqual(1, response.Headers.Count);
        Assert.AreEqual(cachedBytes, response.Body);
        Assert.IsTrue(response.BodyIsBinary);
    }

    [DataTestMethod]
    [DataRow(ResponseImageType.Jpeg, "image/jpeg")]
    [DataRow(ResponseImageType.Png, "image/png")]
    [DataRow(ResponseImageType.Bmp, "image/bmp")]
    [DataRow(ResponseImageType.Gif, "image/gif")]
    public async Task WriteToResponseAsync_AllFileTypes(ResponseImageType? type, string expectedContentType)
    {
        // Arrange
        var stub = new StubModel
        {
            Response = new StubResponseModel
            {
                Image = new StubResponseImageModel { Type = type, Height = 512, Width = 512 }
            }
        };
        var response = new ResponseModel();

        var writer = _mocker.CreateInstance<ImageResponseWriter>();
        _mocker.GetMock<IAssemblyService>()
            .Setup(m => m.GetExecutingAssemblyRootPath())
            .Returns(AssemblyHelper.GetExecutingAssemblyRootPath);

        var expectedCachePath = Path.Combine(_tempFolder, $"{stub.Response.Image.Hash}.bin");
            var fileServiceMock = _mocker.GetMock<IFileService>();
        fileServiceMock
            .Setup(m => m.FileExistsAsync(expectedCachePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(response.BodyIsBinary);
        Assert.AreEqual(1, response.Headers.Count);
        await using var ms = new MemoryStream(response.Body);
        using var image = new MagickImage(ms);
        Assert.AreEqual(stub.Response.Image.Height, image.Height);
        Assert.AreEqual(stub.Response.Image.Width, image.Width);
        Assert.AreEqual(expectedContentType, response.Headers[HeaderKeys.ContentType]);
        fileServiceMock
            .Verify(m => m.WriteAllBytesAsync(expectedCachePath, response.Body, It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
