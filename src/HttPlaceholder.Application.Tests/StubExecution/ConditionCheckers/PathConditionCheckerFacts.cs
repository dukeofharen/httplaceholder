﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class PathConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public void PathConditionChecker_Validate_StubsFound_ButNoPathConditions_ShouldReturnNotExecuted()
    {
        // arrange
        var checker = _mocker.CreateInstance<PathConditionChecker>();
        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Path = null
            }
        };

        // act
        var result = checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public void PathConditionChecker_Validate_StubsFound_WrongPath_ShouldReturnInvalid()
    {
        // arrange
        const string path = "/login";

        var checker = _mocker.CreateInstance<PathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Path = @"\blocatieserver\/v3\/suggest\b"
            }
        };

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.Path, out outputForLogging))
            .Returns(false);

        // act
        var result = checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
    }

    [TestMethod]
    public void PathConditionChecker_Validate_StubsFound_HappyFlow_CompleteUrl()
    {
        // arrange
        const string path = "/locatieserver/v3/suggest";

        var checker = _mocker.CreateInstance<PathConditionChecker>();
        var httpContextServiceMock = _mocker.GetMock<IHttpContextService>();
        var stringCheckerMock = _mocker.GetMock<IStringChecker>();

        var conditions = new StubConditionsModel
        {
            Url = new StubUrlConditionModel
            {
                Path = @"/locatieserver/v3/suggest"
            }
        };

        httpContextServiceMock
            .Setup(m => m.Path)
            .Returns(path);

        string outputForLogging;
        stringCheckerMock
            .Setup(m => m.CheckString(path, conditions.Url.Path, out outputForLogging))
            .Returns(true);

        // act
        var result = checker.Validate(new StubModel{Id = "id", Conditions = conditions});

        // assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }
}
