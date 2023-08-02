using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Web.Shared.Hubs;

namespace HttPlaceholder.Web.Shared.Tests.Hubs;

[TestClass]
public class BaseHubFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task OnConnectedAsync_NotAuthorized_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var hub = _mocker.CreateInstance<TestHub>();
        var loginServiceMock = _mocker.GetMock<ILoginCookieService>();
        loginServiceMock
            .Setup(m => m.CheckLoginCookie())
            .Returns(false);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => hub.OnConnectedAsync());
    }

    [TestMethod]
    public async Task OnConnectedAsync_Authorized_ShouldContinue()
    {
        // Arrange
        var hub = _mocker.CreateInstance<TestHub>();
        var loginServiceMock = _mocker.GetMock<ILoginCookieService>();
        loginServiceMock
            .Setup(m => m.CheckLoginCookie())
            .Returns(true);

        // Act / Assert
        await hub.OnConnectedAsync();
    }

    private class TestHub : BaseHub
    {
        public TestHub(ILoginCookieService loginCookieService) : base(loginCookieService)
        {
        }
    }
}
