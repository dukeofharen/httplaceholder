using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class CacheService : ICacheService, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public CacheService(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    public TObject GetScopedItem<TObject>(string key) => _httpContextService.GetItem<TObject>(key);

    public void SetScopedItem(string key, object item) => _httpContextService.SetItem(key, item);

    public bool DeleteScopedItem(string key) => _httpContextService.DeleteItem(key);
}
