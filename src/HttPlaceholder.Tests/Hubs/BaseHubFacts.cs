using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Authentication;
using HttPlaceholder.Hubs.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Tests.Hubs;

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
        var loginServiceMock = _mocker.GetMock<ILoginService>();
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
        var loginServiceMock = _mocker.GetMock<ILoginService>();
        loginServiceMock
            .Setup(m => m.CheckLoginCookie())
            .Returns(true);

        // Act / Assert
        await hub.OnConnectedAsync();
    }

    public class TestHub : BaseHub
    {
        public TestHub(ILoginService loginService) : base(loginService)
        {
        }
    }
}
