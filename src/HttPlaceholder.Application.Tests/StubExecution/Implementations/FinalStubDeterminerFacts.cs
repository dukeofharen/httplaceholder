﻿using System.Collections.Generic;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution.Implementations;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations;

[TestClass]
public class FinalStubDeterminerFacts
{
    private readonly FinalStubDeterminer _determiner = new();

    [TestMethod]
    public void FinalStubDeterminer_DetermineFinalStub_NoStubsFound_ShouldThrowValidationException()
    {
        // Act
        var exception = Assert.ThrowsException<ValidationException>(() =>
            _determiner.DetermineFinalStub(Array.Empty<(StubModel, IEnumerable<ConditionCheckResultModel>)>()));

        // Assert
        Assert.IsTrue(exception.Message.Contains("No stub found."));
    }

    [TestMethod]
    public void FinalStubDeterminer_DetermineFinalStub_OnlyOneStubFound_ShouldReturnThatStub()
    {
        // Arrange
        var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>
        {
            (new StubModel { Priority = 1 },
                new[] { ConditionCheckResultModel.Valid() })
        };

        // Act
        var result = _determiner.DetermineFinalStub(matchedStubs);

        // Assert
        Assert.AreEqual(matchedStubs[0].Item1, result);
    }

    [TestMethod]
    public void
        FinalStubDeterminer_DetermineFinalStub_MultipleStubsWithHighestPriority_ShouldPickOneWithMostValidConditions()
    {
        // Arrange
        var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>
        {
            (new StubModel { Priority = 1 },
                new[]
                {
                    ConditionCheckResultModel.NotExecuted(), ConditionCheckResultModel.NotExecuted(),
                    ConditionCheckResultModel.Valid()
                }),
            (new StubModel { Priority = 1 },
                new[]
                {
                    ConditionCheckResultModel.Valid(), ConditionCheckResultModel.NotExecuted(),
                    ConditionCheckResultModel.Valid()
                }),
            (new StubModel { Priority = 0 },
                new[]
                {
                    ConditionCheckResultModel.Valid(), ConditionCheckResultModel.Valid(),
                    ConditionCheckResultModel.Valid()
                })
        };

        // Act
        var result = _determiner.DetermineFinalStub(matchedStubs);

        // Assert
        Assert.AreEqual(matchedStubs[1].Item1, result);
    }

    [TestMethod]
    public void
        FinalStubDeterminer_DetermineFinalStub_MultipleStubsFound_OnlyOneWithHighestPriority_ShouldReturnStubWithHighestPriority()
    {
        // Arrange
        var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>
        {
            (new StubModel { Priority = 1 },
                new[]
                {
                    ConditionCheckResultModel.NotExecuted(), ConditionCheckResultModel.NotExecuted(),
                    ConditionCheckResultModel.Valid()
                }),
            (new StubModel { Priority = 0 },
                new[]
                {
                    ConditionCheckResultModel.Valid(), ConditionCheckResultModel.Valid(),
                    ConditionCheckResultModel.Valid()
                })
        };

        // Act
        var result = _determiner.DetermineFinalStub(matchedStubs);

        // Assert
        Assert.AreEqual(matchedStubs[0].Item1, result);
    }

    [TestMethod]
    public void FinalStubDeterminer_DetermineFinalStub_OnlyOneStub_ShouldReturnThatStub()
    {
        // Arrange
        var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>
        {
            (new StubModel { Priority = 0 },
                new[]
                {
                    ConditionCheckResultModel.Valid(), ConditionCheckResultModel.Valid(),
                    ConditionCheckResultModel.Valid()
                })
        };

        // Act
        var result = _determiner.DetermineFinalStub(matchedStubs);

        // Assert
        Assert.AreEqual(matchedStubs[0].Item1, result);
    }
}
