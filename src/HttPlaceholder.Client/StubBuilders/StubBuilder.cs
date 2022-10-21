using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Stubs;

namespace HttPlaceholder.Client.StubBuilders;

/// <summary>
///     A class for building a <see cref="StubDto" /> in a fluent way.
/// </summary>
public sealed class StubBuilder
{
    private readonly StubDto _stub = new();

    private StubBuilder()
    {
    }

    /// <summary>
    ///     Creates a new <see cref="StubBuilder" /> instance.
    /// </summary>
    /// <returns>A <see cref="StubBuilder" /> instance.</returns>
    public static StubBuilder Begin() => new();

    /// <summary>
    ///     Sets the ID of the stub.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithId(string id)
    {
        _stub.Id = id;
        return this;
    }

    /// <summary>
    ///     Sets the priority of the stub.
    ///     If multiple stubs are matched, the one with the highest priority will be used.
    /// </summary>
    /// <param name="priority">The priority.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithPriority(int priority)
    {
        _stub.Priority = priority;
        return this;
    }

    /// <summary>
    ///     Sets the priority of the stub.
    /// </summary>
    /// <param name="priority">The priority as <see cref="PriorityType" />.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithPriority(PriorityType priority)
    {
        _stub.Priority = (int)priority;
        return this;
    }

    /// <summary>
    ///     Sets the tenant of the stub.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithTenant(string tenant)
    {
        _stub.Tenant = tenant;
        return this;
    }

    /// <summary>
    ///     Sets the description of the stub.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithDescription(string description)
    {
        _stub.Description = description;
        return this;
    }

    /// <summary>
    ///     Sets the stub as "enabled".
    /// </summary>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder IsEnabled()
    {
        _stub.Enabled = true;
        return this;
    }

    /// <summary>
    ///     Sets the stub as "disabled".
    /// </summary>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder IsDisabled()
    {
        _stub.Enabled = false;
        return this;
    }

    /// <summary>
    ///     Puts the stub under a specific scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder InScenario(string scenario)
    {
        _stub.Scenario = scenario;
        return this;
    }

    /// <summary>
    ///     Adds request conditions to the stub based on a <see cref="StubConditionsDto" />.
    /// </summary>
    /// <param name="conditions">The <see cref="StubConditionsDto" />.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithConditions(StubConditionsDto conditions)
    {
        _stub.Conditions = conditions;
        return this;
    }

    /// <summary>
    ///     Adds request conditions to the stub based on a <see cref="StubConditionBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="StubConditionBuilder" />.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithConditions(StubConditionBuilder builder) => WithConditions(builder.Build());

    /// <summary>
    ///     Adds a response definition to the stub based on a <see cref="StubResponseDto" />.
    /// </summary>
    /// <param name="response">The <see cref="StubResponseDto" />.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithResponse(StubResponseDto response)
    {
        _stub.Response = response;
        return this;
    }

    /// <summary>
    ///     Adds a response definition to the stub based on a <see cref="StubResponseBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="StubResponseBuilder" />.</param>
    /// <returns>The current <see cref="StubBuilder" />.</returns>
    public StubBuilder WithResponse(StubResponseBuilder builder) => WithResponse(builder.Build());

    /// <summary>
    ///     Builds the stub.
    /// </summary>
    /// <returns>The built <see cref="StubDto" />.</returns>
    public StubDto Build() => _stub;
}
