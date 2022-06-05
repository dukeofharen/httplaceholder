using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class HostConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void Validate_NoConditionFound_ShouldReturnNotExecuted()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();

        var conditions = new StubConditionsModel {Host = null};

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void Validate_ConditionFails_ShouldReturnInvalid()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();

        var host = "actualhost.com";
        clientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns(host);

        var conditions = new StubConditionsModel {Host = "host.com"};
        stringCheckerMock
            .Setup(m => m.CheckString(host, "host.com"))
            .Returns(false);

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void Validate_ConditionSucceeds_ShouldReturnValid()
    {
        // arrange
        var checker = _mocker.CreateInstance<HostConditionChecker>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();
        var clientDataResolverMock = _mocker.GetMock<IClientDataResolver>();

        var host = "actualhost.com";
        clientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns(host);

        var conditions = new StubConditionsModel {Host = "actualhost.com"};
        stringCheckerMock
            .Setup(m => m.CheckString(host, "actualhost.com"))
            .Returns(true);

        // act
        var result = checker.Validate(new StubModel {Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
