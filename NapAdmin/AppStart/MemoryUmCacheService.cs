using Microsoft.Extensions.Caching.Memory;
using ONEbook.UM.Helpers;

namespace NapAdmin.AppStart;

public class MemoryUmCacheService : IUMCacheService
{
    private readonly IMemoryCache memoryCache;

    public MemoryUmCacheService(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public T Get<T>(string key) => memoryCache.Get<T>(key);

    public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
        => memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration));

    public void Remove(string key) => memoryCache.Remove(key);
}
