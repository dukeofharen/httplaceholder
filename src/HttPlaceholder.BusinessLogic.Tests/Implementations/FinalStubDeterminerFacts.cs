using System.Collections.Generic;
using HttPlaceholder.BusinessLogic.Implementations;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations
{
    [TestClass]
    public class FinalStubDeterminerFacts
    {
        private FinalStubDeterminer _determiner;

        [TestInitialize]
        public void Initialize()
        {
            _determiner = new FinalStubDeterminer();
        }

        [TestMethod]
        public void FinalStubDeterminer_DetermineFinalStub_MultipleStubsWithHighestPriority_ShouldPickOneWithMostValidConditions()
        {
            // arrange
            var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
            matchedStubs.Add((new StubModel
            {
                Priority = 1
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.NotExecuted },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.NotExecuted },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));
            matchedStubs.Add((new StubModel
            {
                Priority = 1
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.NotExecuted },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));
            matchedStubs.Add((new StubModel
            {
                Priority = 0
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));

            // act
            var result = _determiner.DetermineFinalStub(matchedStubs);

            // assert
            Assert.AreEqual(matchedStubs[1].Item1, result);
        }

        [TestMethod]
        public void FinalStubDeterminer_DetermineFinalStub_MulitpleStubsFound_OnlyOneWithHighestPriority_ShouldReturnStubWithHighestPriority()
        {
            // arrange
            var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
            matchedStubs.Add((new StubModel
            {
                Priority = 1
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.NotExecuted },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.NotExecuted },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));
            matchedStubs.Add((new StubModel
            {
                Priority = 0
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));

            // act
            var result = _determiner.DetermineFinalStub(matchedStubs);

            // assert
            Assert.AreEqual(matchedStubs[0].Item1, result);
        }

        [TestMethod]
        public void FinalStubDeterminer_DetermineFinalStub_OnlyOneStub_ShouldReturnThatStub()
        {
            // arrange
            var matchedStubs = new List<(StubModel, IEnumerable<ConditionCheckResultModel>)>();
            matchedStubs.Add((new StubModel
            {
                Priority = 0
            }, new[]
            {
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid },
                new ConditionCheckResultModel{ ConditionValidation = ConditionValidationType.Valid }
            }));

            // act
            var result = _determiner.DetermineFinalStub(matchedStubs);

            // assert
            Assert.AreEqual(matchedStubs[0].Item1, result);
        }
    }
}
