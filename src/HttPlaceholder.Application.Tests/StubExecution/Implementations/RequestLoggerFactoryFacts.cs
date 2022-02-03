using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class RequestLoggerFactoryFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void GetRequestLogger_RequestLoggerNotAddedYet_ShouldCreateRequestLogger()
    {
        // Arrange
        var factory = _mocker.CreateInstance<RequestLoggerFactory>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var dateTimeMock = _mocker.GetMock<IDateTime>();

        httpContextServiceMock
            .Setup(m => m.GetItem<IRequestLogger>("requestLogger"))
            .Returns((IRequestLogger)null);

        var now = DateTime.UtcNow;
        dateTimeMock
            .Setup(m => m.UtcNow)
            .Returns(now);

        // Act
        var result = factory.GetRequestLogger();

        // Assert
        httpContextServiceMock.Verify(m => m.SetItem("requestLogger", result));
        Assert.AreEqual(now, result.GetResult().RequestBeginTime);
    }

    [TestMethod]
    public void GetRequestLogger_RequestLoggerAlreadyAdded_ShouldReturnExistingRequestLogger()
    {
        // Arrange
        var factory = _mocker.CreateInstance<RequestLoggerFactory>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var requestLoggerMock = _mocker.GetMock<IRequestLogger>();
        httpContextServiceMock
            .Setup(m => m.GetItem<IRequestLogger>("requestLogger"))
            .Returns(requestLoggerMock.Object);

        // Act
        var result = factory.GetRequestLogger();

        // Assert
        Assert.AreEqual(requestLoggerMock.Object, result);
        httpContextServiceMock.Verify(m => m.SetItem("requestLogger", It.IsAny<IRequestLogger>()), Times.Never);
    }
}
