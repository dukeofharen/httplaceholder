using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class BasicAuthenticationConditionCheckerFacts
{
    private readonly Mock<IHttpContextService> _httpContextServiceMock = new();
    private BasicAuthenticationConditionChecker _checker;

    [TestInitialize]
    public void Initialize() =>
        _checker = new BasicAuthenticationConditionChecker(
            _httpContextServiceMock.Object);

    [TestCleanup]
    public void Cleanup() => _httpContextServiceMock.VerifyAll();

    [TestMethod]
    public async Task
        BasicAuthenticationConditionChecker_ValidateAsync_StubsFound_ButNoBasicAuthenticationCondition_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel { BasicAuthentication = null };

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task
        BasicAuthenticationConditionChecker_ValidateAsync_StubsFound_NoUsernameAndPasswordSet_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            BasicAuthentication = new StubBasicAuthenticationModel { Username = null, Password = null }
        };

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task BasicAuthenticationConditionChecker_ValidateAsync_NoAuthorizationHeader_ShouldReturnInvalid()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            BasicAuthentication = new StubBasicAuthenticationModel { Username = "username", Password = "password" }
        };

        var headers = new Dictionary<string, string> { { "X-Api-Key", "1" } };

        _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task
        BasicAuthenticationConditionChecker_ValidateAsync_BasicAuthenticationIncorrect_ShouldReturnInvalid()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            BasicAuthentication = new StubBasicAuthenticationModel { Username = "username", Password = "password" }
        };

        var headers = new Dictionary<string, string> { { "Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmRk" } };

        _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task BasicAuthenticationConditionChecker_ValidateAsync_HappyFlow()
    {
        // arrange
        var conditions = new StubConditionsModel
        {
            BasicAuthentication = new StubBasicAuthenticationModel { Username = "username", Password = "password" }
        };

        var headers = new Dictionary<string, string> { { "Authorization", "Basic dXNlcm5hbWU6cGFzc3dvcmQ=" } };

        _httpContextServiceMock
            .Setup(m => m.GetHeaders())
            .Returns(headers);

        // act
        var result =
            await _checker.ValidateAsync(new StubModel { Id = "id", Conditions = conditions }, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
