using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class ClientIpConditionCheckerFacts
{
    private readonly Mock<IClientDataResolver> _clientIpResolverMock = new();
    private ClientIpConditionChecker _checker;

    [TestInitialize]
    public void Initialize() =>
        _checker = new ClientIpConditionChecker(
            _clientIpResolverMock.Object);

    [TestCleanup]
    public void Cleanup() => _clientIpResolverMock.VerifyAll();

    [TestMethod]
    public async Task ClientIpConditionChecker_ValidateAsync_ConditionNotSet_ShouldReturnNotExecuted()
    {
        // arrange
        const string stubId = "stub1";
        var conditions = new StubConditionsModel
        {
            ClientIp = null
        };

        // act
        var result = await _checker.ValidateAsync(new StubModel{Id = stubId, Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ClientIpConditionChecker_ValidateAsync_SingleIp_NotEqual_ShouldReturnInvalid()
    {
        // arrange
        const string stubId = "stub1";
        const string clientIp = "127.0.0.1";
        var conditions = new StubConditionsModel
        {
            ClientIp = "127.0.0.2"
        };

        _clientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

        // act
        var result = await _checker.ValidateAsync(new StubModel{Id = stubId, Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ClientIpConditionChecker_ValidateAsync_SingleIp_Equal_ShouldReturnValid()
    {
        // arrange
        const string stubId = "stub1";
        const string clientIp = "127.0.0.1";
        var conditions = new StubConditionsModel
        {
            ClientIp = "127.0.0.1"
        };

        _clientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

        // act
        var result = await _checker.ValidateAsync(new StubModel{Id = stubId, Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ClientIpConditionChecker_ValidateAsync_IpRange_NotInRange_ShouldReturnInvalid()
    {
        // arrange
        const string stubId = "stub1";
        const string clientIp = "127.0.0.9";
        var conditions = new StubConditionsModel
        {
            ClientIp = "127.0.0.0/29"
        };

        _clientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

        // act
        var result = await _checker.ValidateAsync(new StubModel{Id = stubId, Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ClientIpConditionChecker_ValidateAsync_IpRange_InRange_ShouldReturnValid()
    {
        // arrange
        const string stubId = "stub1";
        const string clientIp = "127.0.0.6";
        var conditions = new StubConditionsModel
        {
            ClientIp = "127.0.0.0/29"
        };

        _clientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns(clientIp);

        // act
        var result = await _checker.ValidateAsync(new StubModel{Id = stubId, Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
