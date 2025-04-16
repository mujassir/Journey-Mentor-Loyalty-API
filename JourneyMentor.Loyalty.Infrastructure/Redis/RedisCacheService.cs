using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace JourneyMentor.Loyalty.Infrastructure.Redis
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _options;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json, _options);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value, _options);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(key, json, options);
        }
    }
}
