using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <summary>
///     A class that is used to set and get stub related data for a specific HTTP request.
/// </summary>
public class StubRequestContext : IStubRequestContext, ISingletonService
{
    private static string _distributionKey = "";
    private const string DistributionKeyKey = "distributionKey";
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    ///     Constructs a <see cref="StubRequestContext"/>.
    /// </summary>
    public StubRequestContext(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    // /// <inheritdoc />
    // public string DistributionKey
    // {
    //     get => _httpContextService.GetItem<string>(DistributionKeyKey);
    //     set => _httpContextService.SetItem(DistributionKeyKey, value);
    // }

    /// <inheritdoc />
    public string DistributionKey
    {
        get => _distributionKey;

        set => _distributionKey = value;
    }
}
