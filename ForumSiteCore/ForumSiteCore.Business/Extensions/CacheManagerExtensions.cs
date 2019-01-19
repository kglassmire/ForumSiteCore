using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ForumSiteCore.Business.Extensions
{
    public static class CacheManagerExtensions
    {
        // in case they ever actually build this out
        // https://github.com/MichaCo/CacheManager/issues/209#issuecomment-351191145
        public static async Task<T> GetOrAddAsync<T>(
            this ICacheManager<T> cacheManager,
            string key,
            Func<string, Task<T>> valueFactory)
        {
            var result = default(T);

            if (cacheManager.Exists(key))
            {
                result = cacheManager.Get<T>(key);
            }
            else
            {
                result = await valueFactory(key).ConfigureAwait(false);
                cacheManager.Add(key, result);
            }

            return result;
        }

        public static async Task<T> GetOrAddAsync<T>(
            this ICacheManager<T> cacheManager,
            string key,
            Func<string, Task<T>> valueFactory,
            ExpirationMode expirationMode,
            TimeSpan timeout)
        {
            var result = default(T);

            if (cacheManager.Exists(key))
            {
                result = cacheManager.Get<T>(key);
            }
            else
            {
                result = await valueFactory(key).ConfigureAwait(false);
                cacheManager.Add(new CacheItem<T>(key, result, expirationMode, timeout));
            }

            return result;
        }
    }
}
