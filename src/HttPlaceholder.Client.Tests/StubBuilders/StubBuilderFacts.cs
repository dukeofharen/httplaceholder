using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Client.Tests.StubBuilders;

[TestClass]
public class StubBuilderFacts
{
    [TestMethod]
    public void WithId()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithId("stub-id")
            .Build();

        // Assert
        Assert.AreEqual("stub-id", stub.Id);
    }

    [TestMethod]
    public void WithPriorityAsInt()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithPriority(1)
            .Build();

        // Assert
        Assert.AreEqual(1, stub.Priority);
    }

    [TestMethod]
    public void WithPriorityAsPriorityType()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithPriority(PriorityType.High)
            .Build();

        // Assert
        Assert.AreEqual(10, stub.Priority);
    }

    [TestMethod]
    public void WithTenant()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithTenant("tenant1")
            .Build();

        // Assert
        Assert.AreEqual("tenant1", stub.Tenant);
    }

    [TestMethod]
    public void WithDescription()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithDescription("stub description")
            .Build();

        // Assert
        Assert.AreEqual("stub description", stub.Description);
    }

    [TestMethod]
    public void IsEnabled()
    {
        // Act
        var stub = StubBuilder.Begin()
            .IsEnabled()
            .Build();

        // Assert
        Assert.IsTrue(stub.Enabled);
    }

    [TestMethod]
    public void IsDisabled()
    {
        // Act
        var stub = StubBuilder.Begin()
            .IsDisabled()
            .Build();

        // Assert
        Assert.IsFalse(stub.Enabled);
    }

    [TestMethod]
    public void InScenario()
    {
        // Act
        const string scenario = "scenario-1";
        var stub = StubBuilder.Begin()
            .InScenario(scenario)
            .Build();

        // Assert
        Assert.AreEqual(scenario, stub.Scenario);
    }

    [TestMethod]
    public void WithConditionsAsDto()
    {
        // Act
        var conditions = new StubConditionsDto();
        var stub = StubBuilder.Begin()
            .WithConditions(conditions)
            .Build();

        // Assert
        Assert.AreEqual(conditions, stub.Conditions);
    }

    [TestMethod]
    public void WithConditionsAsBuilder()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithConditions(StubConditionBuilder.Begin().Build())
            .Build();

        // Assert
        Assert.IsNotNull(stub.Conditions);
    }

    [TestMethod]
    public void WithResponseAsDto()
    {
        // Act
        var response = new StubResponseDto();
        var stub = StubBuilder.Begin()
            .WithResponse(response)
            .Build();

        // Assert
        Assert.AreEqual(response, stub.Response);
    }

    [TestMethod]
    public void WithResponseAsBuilder()
    {
        // Act
        var stub = StubBuilder.Begin()
            .WithResponse(StubResponseBuilder.Begin().Build())
            .Build();

        // Assert
        Assert.IsNotNull(stub.Response);
    }
}