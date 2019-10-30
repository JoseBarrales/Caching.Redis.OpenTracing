using Caching.Redis.OpenTracing.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caching.Redis.OpenTracing.Stores
{
    /// <summary>
    /// Provides an storage to avoid code changes when implementing cache
    /// </summary>
    public class NullStore : ICacheStore
    {
        public Task DeleteAllAsync(IEnumerable<string> keys)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsParallelAsync<T>(IEnumerable<string> keys)
        {
            return Task.FromResult(default(IEnumerable<T>));
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            return Task.FromResult(default(IEnumerable<T>));
        }

        public Task<T> GetAsync<T>(string keys)
        {
            return Task.FromResult(default(T));
        }

        public Task SetAsParallelAsync<T>(IDictionary<string, T> keys)
        {
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(IDictionary<string, T> keys)
        {
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(T value, string key)
        {
            return Task.CompletedTask;
        }

        public Task<T> GetOrCreateAsync<T>(string keys, Func<Task<T>> func)
        {
            return Task.FromResult(default(T));
        }
    }
}
