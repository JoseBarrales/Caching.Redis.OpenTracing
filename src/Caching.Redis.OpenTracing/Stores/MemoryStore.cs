using StackExchange.Redis.Extensions.Core.Extensions;
using Caching.Redis.OpenTracing.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Caching.Redis.OpenTracing.Stores
{
    /// <summary>
    /// You should consider redis before using this provider
    /// </summary>
    public class MemoryStore : ICacheStore
    {
        private readonly IMemoryCache memoryCache;
        private readonly IOptions<CacheStoreOptions> options;

        public MemoryStore(IMemoryCache memoryCache, IOptions<CacheStoreOptions> options)
        {
            this.memoryCache = memoryCache;
            this.options = options;
        }

        public Task DeleteAllAsync(IEnumerable<string> keys)
        {
            keys.ForEach(memoryCache.Remove);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsParallelAsync<T>(IEnumerable<string> keys)
        {
            return Task.FromResult(keys.Select(key => memoryCache.Get<T>(key)));
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            return Task.FromResult(keys.Select(key => memoryCache.Get<T>(key)));
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(memoryCache.Get<T>(key));
        }

        public Task SetAsParallelAsync<T>(IDictionary<string, T> keys)
        {
            foreach (var key in keys)
            {
                memoryCache.Set(key.Key, key.Value, TimeSpan.FromMinutes(options.Value.CacheExpire));
            }

            return Task.CompletedTask;
        }

        public Task SetAsync<T>(IDictionary<string, T> keys)
        {
            foreach (var key in keys)
            {
                memoryCache.Set(key.Key, key.Value, TimeSpan.FromMinutes(options.Value.CacheExpire));
            }

            return Task.CompletedTask;
        }

        public Task SetAsync<T>(T value, string key)
        {
            memoryCache.Set(key, value, TimeSpan.FromMinutes(options.Value.CacheExpire));

            return Task.CompletedTask;
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func)
        {
            return Task.FromResult(memoryCache.Get<T>(key));
        }
    }
}