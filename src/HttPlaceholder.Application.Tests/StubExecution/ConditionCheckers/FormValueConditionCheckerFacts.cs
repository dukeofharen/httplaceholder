using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.TestUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class FormValueConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_StubsFound_ButNoFormConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var conditions = new StubConditionsModel {Form = null};
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();

        // act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_FormKeyNotFound_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();

        var conditions = new StubConditionsModel {Form = new[] {new StubFormModel {Key = "key3", Value = "val1.1"}}};
        var form = new (string, StringValues)[] {("key1", "val3"), ("key2", "val4")};
        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(form);


        // Act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_AllConditionsFail_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Form = new[]
            {
                new StubFormModel {Key = "key1", Value = "val1.1"},
                new StubFormModel {Key = "key1", Value = "val1.2"},
                new StubFormModel {Key = "key2", Value = "val2"}
            }
        };
        var form = new (string, StringValues)[] {("key1", "val3"), ("key2", "val4")};
        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(form);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("val3", "val1.1", out outputForLogging))
            .Returns(false);
        stringCheckerMock
            .Setup(m => m.CheckString("val3", "val1.2", out outputForLogging))
            .Returns(false);
        stringCheckerMock
            .Setup(m => m.CheckString("val4", "val2", out outputForLogging))
            .Returns(false);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_OneConditionFails_ShouldReturnInvalid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Form = new[]
            {
                new StubFormModel {Key = "key1", Value = "val1.1"},
                new StubFormModel {Key = "key1", Value = "val1.2"},
                new StubFormModel {Key = "key2", Value = "val2"}
            }
        };
        var form = new (string, StringValues)[] {("key1", "val3"), ("key2", "val4")};
        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(form);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("val3", "val1.1", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("val3", "val1.2", out outputForLogging))
            .Returns(false);
        stringCheckerMock
            .Setup(m => m.CheckString("val4", "val2", out outputForLogging))
            .Returns(true);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_AllConditionsSucceed_ShouldReturnValid()
    {
        // Arrange
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Form = new[]
            {
                new StubFormModel {Key = "key1", Value = "val 3"},
                new StubFormModel {Key = "key2", Value = "val4"}
            }
        };
        var form = new (string, StringValues)[] {("key1", "val%203"), ("key2", "val4")};
        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(form);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString("val 3", "val 3", out outputForLogging))
            .Returns(true);
        stringCheckerMock
            .Setup(m => m.CheckString("val4", "val4", out outputForLogging))
            .Returns(true);

        // Act
        var result =
            await checker.ValidateAsync(new StubModel {Id = "id", Conditions = conditions}, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    public static IEnumerable<object[]> GetPresentData()
    {
        yield return new object[]
        {
            new[]
            {
                new StubFormModel {Key = "key1", Value = TestObjectFactory.CreateStringCheckingModel(true)},
                new StubFormModel {Key = "key2", Value = TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new[] {("key1", new StringValues("somevalue")), ("key3", new StringValues("somevalue"))}, true
        };
        yield return new object[]
        {
            new[]
            {
                new StubFormModel {Key = "key1", Value = TestObjectFactory.CreateStringCheckingModel(true)},
                new StubFormModel {Key = "key2", Value = TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new[] {("key1", new StringValues("somevalue"))}, true
        };
        yield return new object[]
        {
            new[]
            {
                new StubFormModel {Key = "key1", Value = TestObjectFactory.CreateStringCheckingModel(true)},
                new StubFormModel {Key = "key2", Value = TestObjectFactory.CreateStringCheckingModel(false)}
            },
            new[] {("key1", new StringValues("somevalue")), ("key2", new StringValues("somevalue"))}, false
        };
    }

    [DataTestMethod]
    [DynamicData(nameof(GetPresentData), DynamicDataSourceType.Method)]
    public async Task ValidateAsync_Present(
        StubFormModel[] formConditions,
        (string, StringValues)[] formValues,
        bool shouldSucceed)
    {
        // Arrange
        var checker = _mocker.CreateInstance<FormValueConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        httpContextServiceMock
            .Setup(m => m.GetFormValues())
            .Returns(formValues);

        // Act
        var result = await checker.ValidateAsync(
            new StubModel {Conditions = new StubConditionsModel {Form = formConditions}}, CancellationToken.None);

        // Assert
        Assert.AreEqual(shouldSucceed ? ConditionValidationType.Valid : ConditionValidationType.Invalid,
            result.ConditionValidation);
    }
}
