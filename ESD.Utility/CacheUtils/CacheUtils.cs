using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Utility.CacheUtils
{
    public static class CacheUtils
    {
        public static async Task<T> GetCacheValueAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            try
            {
                string result = await cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(result))
                {
                    return null;
                }
                var deserializedObj = JsonConvert.DeserializeObject<T>(result);
                return deserializedObj;
            }
            catch (Exception)
            {

                return null;
            }
           
        }

        public static async Task SetCacheValueAsync<T>(this IDistributedCache cache, string key, T value, int? AbsoluteExpiration = null, int? SlidingExpiration = null) where T : class
        {
            string result = JsonConvert.SerializeObject(value);
            if (AbsoluteExpiration.HasValue || SlidingExpiration.HasValue)
            {
                DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
                if (AbsoluteExpiration.HasValue)
                {
                    // Remove item from cache after duration
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(AbsoluteExpiration.Value);
                }
                if (SlidingExpiration.HasValue)
                {
                    // Remove item from cache if unsued for the duration
                    cacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(SlidingExpiration.Value);
                }
                await cache.SetStringAsync(key, result, cacheEntryOptions);
            }
            else
            {
                await cache.SetStringAsync(key, result);
            }

        }
    }
}
