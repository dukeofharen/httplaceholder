﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class IsHttpsConditionCheckerFacts
{
    private readonly Mock<IClientDataResolver> _clientDataResolverMock = new();
    private IsHttpsConditionChecker _checker;

    [TestInitialize]
    public void Initialize() => _checker = new IsHttpsConditionChecker(_clientDataResolverMock.Object);

    [TestCleanup]
    public void Cleanup() => _clientDataResolverMock.VerifyAll();

    [TestMethod]
    public async Task IsHttpsConditionChecker_ValidateAsync_NoConditionFound_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel { Url = new StubUrlConditionModel { IsHttps = null } };

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task IsHttpsConditionChecker_ValidateAsync_ConditionIncorrect_ShouldReturnInvalid()
    {
        // arrange
        var conditions = new StubConditionsModel { Url = new StubUrlConditionModel { IsHttps = true } };

        _clientDataResolverMock
            .Setup(m => m.IsHttps())
            .Returns(false);

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task IsHttpsConditionChecker_ValidateAsync_ConditionCorrect_ShouldReturnValid()
    {
        // arrange
        var conditions = new StubConditionsModel { Url = new StubUrlConditionModel { IsHttps = true } };

        _clientDataResolverMock
            .Setup(m => m.IsHttps())
            .Returns(true);

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
