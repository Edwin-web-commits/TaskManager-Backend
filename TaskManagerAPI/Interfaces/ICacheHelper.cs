namespace TaskManagerAPI.Interfaces
{
    public interface ICacheHelper
    {
        Task<T> GetCacheAsync<T>(string key);
        Task RemoveCacheAsync(string key);
        Task SetCacheAsync<T>(string key, T value, TimeSpan expirationTime);
    }
}
