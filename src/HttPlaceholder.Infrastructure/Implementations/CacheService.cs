using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class CacheService(IHttpContextService httpContextService) : ICacheService, ISingletonService
{
    public TObject GetScopedItem<TObject>(string key) => httpContextService.GetItem<TObject>(key);

    public void SetScopedItem(string key, object item) => httpContextService.SetItem(key, item);

    public bool DeleteScopedItem(string key) => httpContextService.DeleteItem(key);
}
