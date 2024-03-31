using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class BaseConditionCheckerFacts
{
    [TestMethod]
    public async Task ValidateAsync_ShouldNotBeExecuted_ShouldReturnNotExecuted()
    {
        // Arrange
        var checker = new TestConditionChecker();
        var stub = new StubModel();
        checker.ShouldBeExecutedFunc = passedStub =>
        {
            Assert.AreEqual(stub, passedStub);
            return false;
        };

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ExceptionWhenPerformingValidation_ShouldReturnInvalid()
    {
        // Arrange
        var checker = new TestConditionChecker();
        var stub = new StubModel();
        checker.ShouldBeExecutedFunc = passedStub =>
        {
            Assert.AreEqual(stub, passedStub);
            return true;
        };
        checker.ValidationFunc = passedStub =>
        {
            Assert.AreEqual(stub, passedStub);
            throw new Exception("ERROR!!1!");
        };

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("ERROR!!1!", result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_HappyFlow_ShouldReturnValid()
    {
        // Arrange
        var checker = new TestConditionChecker();
        var stub = new StubModel();
        checker.ShouldBeExecutedFunc = passedStub =>
        {
            Assert.AreEqual(stub, passedStub);
            return true;
        };
        checker.ValidationFunc = passedStub =>
        {
            Assert.AreEqual(stub, passedStub);
            return ConditionCheckResultModel.Valid();
        };

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        Assert.IsTrue(string.IsNullOrWhiteSpace(result.Log));
    }

    private class TestConditionChecker : BaseConditionChecker
    {
        public Func<StubModel, ConditionCheckResultModel> ValidationFunc { get; set; }

        public Func<StubModel, bool> ShouldBeExecutedFunc { get; set; }

        public override int Priority { get; }

        protected override bool ShouldBeExecuted(StubModel stub) => ShouldBeExecutedFunc(stub);

        protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
            CancellationToken cancellationToken) =>
            ValidationFunc(stub).AsTask();
    }
}
