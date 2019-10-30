using Caching.Redis.OpenTracing.Abstractions;
using static Caching.Redis.OpenTracing.Diagnostic.DiagnosticEvents;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caching.Redis.OpenTracing.Stores
{
    public class RedisStore : ICacheStore
    {
        private readonly IRedisCacheClient redisCache;
        private readonly IOptions<CacheStoreOptions> options;

        public RedisStore(IRedisCacheClient redisCache, IOptions<CacheStoreOptions> options)
        {
            this.redisCache = redisCache;
            this.options = options;
        }

        private IEnumerable<string> PrefixKeys(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                yield return $"{options.Value.RedisPrefix}{key}";
            }
        }

        public async Task DeleteAllAsync(IEnumerable<string> keys)
        {
            var activity = StartSingle(keys.Count(), Diagnostic.RedisDiagnosticActivityType.Delete);

            try
            {
                keys = PrefixKeys(keys);

                await redisCache.Db0.RemoveAllAsync(keys, StackExchange.Redis.CommandFlags.FireAndForget);
            }
            catch (Exception ex)
            {
                ReceivedError(keys.ToArray(), ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Delete);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsParallelAsync<T>(IEnumerable<string> keys)
        {
            var activity = StartSingle(keys.Count(), Diagnostic.RedisDiagnosticActivityType.Get);

            try
            {
                keys = PrefixKeys(keys);

                var tasks = await Task
                    .WhenAll(keys
                                .AsParallel()
                                .WithDegreeOfParallelism(options.Value.WithDegreeOfParallelism)
                                .Select(async slice => await redisCache.Db0.GetAsync<T>(slice))
                            )
                    .ContinueWith((t) => t.Result?.ToList());


                ReportSingle(tasks?.Where(x => x == null).Count(), Diagnostic.RedisDiagnosticActivityType.Misses);
                ReportSingle(tasks?.Where(x => x != null).Count(), Diagnostic.RedisDiagnosticActivityType.Hits);

                return tasks;
            }
            catch (Exception ex)
            {
                ReceivedError(keys.ToArray(), ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Get);
            }

            return default;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            keys = PrefixKeys(keys);

            var activity = StartSingle(keys.Count(), Diagnostic.RedisDiagnosticActivityType.Get);

            var result = new List<T>();

            try
            {
                await keys.ForEachAsync(async slice => result.Add(await redisCache.Db0.GetAsync<T>(slice)));

                return result;
            }
            catch (Exception ex)
            {
                ReceivedError(keys.ToArray(), ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Get);
            }

            return default;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var activity = StartSingle(key, Diagnostic.RedisDiagnosticActivityType.Get);

            try
            {
                var result = await redisCache.Db0.GetAsync<T>($"{options.Value.RedisPrefix}{key}");

                if (result == default)
                {
                    ReportSingle(1, Diagnostic.RedisDiagnosticActivityType.Misses);
                }
                else
                {
                    ReportSingle(1, Diagnostic.RedisDiagnosticActivityType.Hits);
                }

                return result;
            }
            catch (Exception ex)
            {
                ReceivedError(key, ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Get);
            }

            return default;
        }

        public async Task SetAsParallelAsync<T>(IDictionary<string, T> keys)
        {
            var activity = StartSingle(keys.Count, Diagnostic.RedisDiagnosticActivityType.Set);

            try
            {
                await keys
                        .AsParallel()
                        .WithDegreeOfParallelism(options.Value.WithDegreeOfParallelism)
                        .ForEachAsync(async slice => await redisCache
                          .Db0
                          .AddAsync(
                              $"{options.Value.RedisPrefix}{slice.Key}",
                              slice.Value,
                              DateTimeOffset.Now.AddMinutes(options.Value.CacheExpire),
                              StackExchange.Redis.When.Always,
                              StackExchange.Redis.CommandFlags.FireAndForget
                          ));
            }
            catch (Exception ex)
            {
                ReceivedError(keys.Keys.ToArray(), ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Set);
            }

        }

        public async Task SetAsync<T>(IDictionary<string, T> keys)
        {
            var activity = StartSingle(keys.Count, Diagnostic.RedisDiagnosticActivityType.Set);

            try
            {
                await keys
                 .ForEachAsync(async slice => await redisCache
                   .Db0
                   .AddAsync(
                      $"{options.Value.RedisPrefix}{slice.Key}",
                       slice.Value,
                       DateTimeOffset.Now.AddMinutes(options.Value.CacheExpire),
                       StackExchange.Redis.When.Always,
                       StackExchange.Redis.CommandFlags.FireAndForget
                   ));
            }
            catch (Exception ex)
            {
                ReceivedError(keys.Keys.ToArray(), ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Set);
            }
            
        }

        public async Task SetAsync<T>(T value, string key)
        {
            var activity = StartSingle(key, Diagnostic.RedisDiagnosticActivityType.Set);

            try
            {
                await redisCache.Db0.AddAsync(
                      $"{options.Value.RedisPrefix}{key}",
                      value,
                      DateTimeOffset.Now.AddMinutes(options.Value.CacheExpire),
                      StackExchange.Redis.When.Always,
                      StackExchange.Redis.CommandFlags.FireAndForget
                  );
            }
            catch (Exception ex)
            {
                ReceivedError(key, ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Set);
            }
            
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func)
        {
            var activity = StartSingle(key, Diagnostic.RedisDiagnosticActivityType.Set);

            try
            {
                key = $"{options.Value.RedisPrefix}{key}";

                var value = await GetAsync<T>(key);

                if (value == null)
                {
                    ReportSingle(1, Diagnostic.RedisDiagnosticActivityType.Misses);

                    value = await func.Invoke();

                    await SetAsync<T>(value, key);
                }
                else
                {
                    ReportSingle(key, Diagnostic.RedisDiagnosticActivityType.Hits);
                }

                return value;
            }
            catch (Exception ex)
            {
                ReceivedError(key, ex);

                if (options.Value.RaiseException)
                    throw;
            }
            finally
            {
                StopSingle(activity, Diagnostic.RedisDiagnosticActivityType.Set);
            }

            return default;
        }
    }
}
