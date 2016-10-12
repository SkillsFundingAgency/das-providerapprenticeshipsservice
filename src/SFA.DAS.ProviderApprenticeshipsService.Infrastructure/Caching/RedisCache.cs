using System;
using System.Configuration;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using StackExchange.Redis;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching
{
    public class RedisCache : ICache
    {
        private IDatabase _cache;
        private object _lockObject = new object();

        public RedisCache()
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnection"]);
            _cache = connectionMultiplexer.GetDatabase();
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(key);
        }

        public T GetCustomValue<T>(string key)
        {
            var redisValue = _cache.StringGet(key);

            return JsonConvert.DeserializeObject<T>(redisValue);
        }

        public void SetCustomValue<T>(string key, T customType, int secondsInCache = 300)
        {
            if (!_cache.KeyExists(key))
            {
                lock (_lockObject)
                {
                    if (!_cache.KeyExists(key))
                    {
                        _cache.StringSet(key, JsonConvert.SerializeObject(customType),
                            TimeSpan.FromSeconds(secondsInCache));
                    }
                }
            }
        }
    }
}
