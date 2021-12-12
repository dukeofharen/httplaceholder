using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;

namespace HttPlaceholder.Client.StubBuilders;

/// <summary>
/// A class for building a <see cref="StubDto"/> in a fluent way.
/// </summary>
public sealed class StubBuilder
{
    private readonly StubDto _stub = new();

    private StubBuilder()
    {
    }

    public static StubBuilder Begin() => new();

    public StubBuilder WithId(string id)
    {
        _stub.Id = id;
        return this;
    }

    public StubBuilder WithPriority(int priority)
    {
        _stub.Priority = priority;
        return this;
    }

    public StubBuilder WithPriority(PriorityType priority)
    {
        _stub.Priority = (int)priority;
        return this;
    }

    public StubBuilder WithTenant(string tenant)
    {
        _stub.Tenant = tenant;
        return this;
    }

    public StubBuilder WithDescription(string description)
    {
        _stub.Description = description;
        return this;
    }

    public StubBuilder IsEnabled()
    {
        _stub.Enabled = true;
        return this;
    }

    public StubBuilder IsDisabled()
    {
        _stub.Enabled = false;
        return this;
    }

    public StubBuilder InScenario(string scenario)
    {
        _stub.Scenario = scenario;
        return this;
    }

    public StubBuilder WithConditions(StubConditionsDto conditions)
    {
        _stub.Conditions = conditions;
        return this;
    }

    public StubBuilder WithConditions(StubConditionBuilder builder) => WithConditions(builder.Build());

    public StubBuilder WithResponse(StubResponseDto response)
    {
        _stub.Response = response;
        return this;
    }

    public StubBuilder WithResponse(StubResponseBuilder builder) => WithResponse(builder.Build());

    public StubDto Build() => _stub;
}