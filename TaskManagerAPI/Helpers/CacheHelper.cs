using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Helpers
{
    public class CacheHelper : ICacheHelper
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ICacheHelper> _logger;
        public CacheHelper(IDistributedCache cache, ILogger<ICacheHelper> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
            }

            var cachedValue = await _cache.GetStringAsync(key);
            if (cachedValue != null)
            {
                _logger.LogInformation($"Cache hit for key: {key}");
                try
                {
                    return JsonConvert.DeserializeObject<T>(cachedValue);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Failed to deserialize cached value for key: {key}");
                    return default;
                }
            }

            _logger.LogInformation($"Cache miss for key: {key}");
            return default;
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
            _logger.LogInformation($"Cache removed for key: {key}");
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expirationTime)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };
            await _cache.SetStringAsync(key, serializedValue, options);

            _logger.LogInformation($"Data cached with key: {key}");
        }
    }
}
