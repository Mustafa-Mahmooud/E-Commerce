namespace Talabat.Core.ServiceContract
{
    public interface ICache
    {
        Task CreateCache(string CacheKey, object Response, TimeSpan time);
        Task<string?> GetFromCacheAsync(string CacheKey);
    }
}