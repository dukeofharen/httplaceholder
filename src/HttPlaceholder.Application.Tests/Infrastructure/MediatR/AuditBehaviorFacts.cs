using System.Linq;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.MediatR;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.TestUtilities.Logging;
using HttPlaceholder.TestUtilities.Options;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.Tests.Infrastructure.MediatR;

[TestClass]
public class AuditBehaviorFacts
{
    private readonly MockLogger<AuditBehavior<TestRequest, TestResponse>> _logger = new();

    private readonly AutoMocker _mocker = new();
    private readonly TestResponse _response = new() { Output = "Output" };
    private readonly SettingsModel _settings = new() { Logging = new LoggingSettingsModel() };

    [TestInitialize]
    public void Setup()
    {
        _mocker.Use(MockSettingsFactory.GetOptionsMonitor(_settings));
        _mocker.Use<ILogger<AuditBehavior<TestRequest, TestResponse>>>(_logger);
    }

    [TestMethod]
    public async Task Handle_VerboseLoggingDisabled_ShouldNotAudit()
    {
        // Arrange
        var behavior = _mocker.CreateInstance<AuditBehavior<TestRequest, TestResponse>>();
        _settings.Logging.VerboseLoggingEnabled = false;
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(new TestRequest(), () =>
        {
            nextCalled = true;
            return Task.FromResult(_response);
        }, CancellationToken.None);

        // Assert
        Assert.IsTrue(nextCalled);
        Assert.AreEqual(_response, result);
        Assert.IsFalse(_logger.HasEntries());
    }

    [TestMethod]
    public async Task Handle_VerboseLoggingEnabled_HappyFlow()
    {
        // Arrange
        var behavior = _mocker.CreateInstance<AuditBehavior<TestRequest, TestResponse>>();
        _settings.Logging.VerboseLoggingEnabled = true;
        var nextCalled = false;

        const string ip = "1.2.3.4";
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();
        clientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        // Act
        var result = await behavior.Handle(new TestRequest { Input = "Input" }, () =>
        {
            nextCalled = true;
            return Task.FromResult(_response);
        }, CancellationToken.None);

        // Assert
        Assert.IsTrue(nextCalled);
        Assert.AreEqual(_response, result);

        var entry = _logger.Entries.Single();
        Assert.IsTrue(entry.State.Contains("""
                                           Audit:
                                           Handling request 'HttPlaceholder.Application.Tests.Infrastructure.MediatR.AuditBehaviorFacts+TestRequest'
                                           Input: {"Input":"Input"}
                                           IP: 1.2.3.4
                                           Duration: 0 ms
                                           """));
    }

    [TestMethod]
    public async Task Handle_VerboseLoggingEnabled_ExceptionOccurs_HappyFlow()
    {
        // Arrange
        var behavior = _mocker.CreateInstance<AuditBehavior<TestRequest, TestResponse>>();
        _settings.Logging.VerboseLoggingEnabled = true;
        var nextCalled = false;

        const string ip = "1.2.3.4";
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();
        clientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(ip);

        // Act
        await Assert.ThrowsExceptionAsync<Exception>(() => behavior.Handle(new TestRequest { Input = "Input" }, () =>
        {
            nextCalled = true;
            throw new Exception("ERROR!");
        }, CancellationToken.None));

        // Assert
        Assert.IsTrue(nextCalled);

        var entry = _logger.Entries.Single();
        Assert.IsTrue(entry.State.Contains("""
                                           Audit:
                                           Handling request 'HttPlaceholder.Application.Tests.Infrastructure.MediatR.AuditBehaviorFacts+TestRequest'
                                           Input: {"Input":"Input"}
                                           IP: 1.2.3.4
                                           System.Exception thrown: System.Exception: ERROR!
                                           """));
    }

    private class TestRequest : IRequest<TestResponse>
    {
        public string Input { get; set; }
    }

    private class TestResponse
    {
        public string Output { get; set; }
    }
}
