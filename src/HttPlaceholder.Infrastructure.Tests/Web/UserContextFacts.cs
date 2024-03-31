using System.Security.Claims;
using HttPlaceholder.Infrastructure.Web;
using HttPlaceholder.TestUtilities;
using HttPlaceholder.TestUtilities.Http;
using Moq.AutoMock;

namespace HttPlaceholder.Infrastructure.Tests.Web;

[TestClass]
public class UserContextFacts
{
    private readonly AutoMocker _mocker = new();
    private readonly MockHttpContext _mockHttpContext = new();

    [TestInitialize]
    public void Setup() => _mocker.Use(TestObjectFactory.CreateHttpContextAccessor(_mockHttpContext));

    [TestMethod]
    public void User_HappyFlow()
    {
        // Arrange
        var context = _mocker.CreateInstance<UserContext>();
        var user = new ClaimsPrincipal();
        _mockHttpContext.User = user;

        // Act
        var result = context.User;

        // Assert
        Assert.AreEqual(user, result);
    }
}
