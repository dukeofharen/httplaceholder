using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Infrastructure.Implementations;
using Moq.AutoMock;

namespace HttPlaceholder.Infrastructure.Tests.Implementations;

[TestClass]
public class CacheServiceFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void GetScopedItem_HappyFlow()
    {
        // Arrange
        var mock = _mocker.GetMock<IHttpContextService>();
        var instance = _mocker.CreateInstance<CacheService>();

        var expectedResult = new StubModel();
        const string key = "stub";
        mock
            .Setup(m => m.GetItem<StubModel>(key))
            .Returns(expectedResult);

        // Act
        var result = instance.GetScopedItem<StubModel>(key);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void SetScopedItem_HappyFlow()
    {
        // Arrange
        var mock = _mocker.GetMock<IHttpContextService>();
        var instance = _mocker.CreateInstance<CacheService>();

        var input = new StubModel();
        const string key = "stub";

        // Act
        instance.SetScopedItem(key, input);

        // Assert
        mock.Verify(m => m.SetItem(key, input));
    }
}
