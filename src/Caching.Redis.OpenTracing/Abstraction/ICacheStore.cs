using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caching.Redis.OpenTracing.Abstractions
{
    /// <summary>
    /// Provides caching capabilities using providers
    /// </summary>
    public interface ICacheStore
    {
        /// <summary>
        ///  Get multiple keys async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync<T>(IEnumerable<string> keys);

        /// <summary>
        ///  Get multiple keys async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsParallelAsync<T>(IEnumerable<string> keys);

        /// <summary>
        /// Set multiple keys async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task SetAsync<T>(IDictionary<string, T> keys);

        /// <summary>
        /// Set multiple keys async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task SetAsParallelAsync<T>(IDictionary<string, T> keys);
        
        /// <summary>
        /// Delete multiple keys async
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task DeleteAllAsync(IEnumerable<string> keys);

        /// <summary>
        /// Set single key async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task SetAsync<T>(T value, string key);

        /// <summary>
        /// Get single key async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string keys);

        /// <summary>
        /// Get single key async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<T> GetOrCreateAsync<T>(string keys, Func<Task<T>> func);
    }
}
