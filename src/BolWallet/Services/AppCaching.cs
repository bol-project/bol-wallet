using Bol.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace BolWallet.Services;

public class AppCaching(IMemoryCache memoryCache) : CachingService(memoryCache), IAppCaching
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
