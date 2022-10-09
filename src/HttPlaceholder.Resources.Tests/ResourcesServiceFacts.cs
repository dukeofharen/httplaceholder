using HttPlaceholder.Common;
using HttPlaceholder.Resources.Implementations;

namespace HttPlaceholder.Resources.Tests;

[TestClass]
public class ResourcesServiceFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void ReadAsString_HappyFlow()
    {
        // Arrange
        var fileServiceMock = _mocker.GetMock<IFileService>();
        var service = _mocker.CreateInstance<ResourcesService>();

        var path = "Some/Path";
        var contents = "file contents";
        fileServiceMock
            .Setup(m => m.ReadAllText(It.Is<string>(p => p.Contains("Some") && p.Contains("Path"))))
            .Returns(contents);

        // Act
        var result = service.ReadAsString(path);

        // Assert
        Assert.AreEqual(contents, result);

        // Act
        result = service.ReadAsString(path);

        // Assert
        Assert.AreEqual(contents, result);
        fileServiceMock.Verify(m => m.ReadAllText(It.IsAny<string>()), Times.Once);
    }
}
