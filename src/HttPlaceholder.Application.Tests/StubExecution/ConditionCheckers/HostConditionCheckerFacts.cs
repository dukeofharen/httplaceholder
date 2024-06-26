﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class HostConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_NoConditionFound_ShouldReturnNotExecuted()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();

        var conditions = new StubConditionsModel { Host = null };

        // act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionFails_ShouldReturnInvalid()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();

        const string host = "actualhost.com";
        clientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns(host);

        var conditions = new StubConditionsModel { Host = "host.com" };
        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(host, "host.com", out outputForLogging))
            .Returns(false);

        // act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ConditionSucceeds_ShouldReturnValid()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();

        const string host = "actualhost.com";
        clientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns(host);

        var conditions = new StubConditionsModel { Host = "actualhost.com" };
        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(host, "actualhost.com", out outputForLogging))
            .Returns(true);

        // act
        var result =
            await checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
