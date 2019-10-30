using Caching.Redis.OpenTracing.Abstractions;
using Caching.Redis.OpenTracing.Multiplexer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.MsgPack;
using System;
using Caching.Redis.OpenTracing.Stores;
using System.Linq;
using StackExchange.Redis.Extensions.Protobuf;

namespace Caching.Redis.OpenTracing
{
    public static class CacheStoresExtensions
    {
        /// <summary>
        /// Add AddCacheStores to ServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddCacheStores(this IServiceCollection services, IOptions<CacheStoreOptions> options)
        {
            Console.WriteLine("Deregistering providers for ICacheStore");

            services.Remove<ICacheStore>();
            services.Remove<ISerializer>();

            services
                .AddOptions()
                .AddOptions<CacheStoreOptions>();

            if (options.Value.CacheStorage.Equals("redis", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Registering RedisStore provider for ICacheStore");

                Console.WriteLine($"Using {options.Value.Serializer} serializer.");

                if (options.Value.Serializer == CacheStoreSerializer.Protobuf)
                {
                    services.AddSingleton<ISerializer, ProtobufSerializer>();
                }
                else
                {
                    services.AddSingleton<ISerializer, MsgPackObjectSerializer>();
                }

                services.AddSingleton(RedisCacheConnectionPoolManager.GetRedisConfiguration(options.Value.RedisHost));
                services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
                services.AddSingleton<IRedisCacheConnectionPoolManager, RedisCacheConnectionPoolManager>();
                services.AddSingleton<IRedisDefaultCacheClient, StackExchange.Redis.Extensions.Core.Implementations.RedisDefaultCacheClient>();
                services.AddScoped<ICacheStore, RedisStore>();
            }
            else if (options.Value.CacheStorage.Equals("memory", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Registering MemoryStore provider for ICacheStore");

                services.AddMemoryCache();
                services.AddScoped<ICacheStore, MemoryStore>();
            }
            else
            {
                Console.WriteLine("Registering NullStore provider for ICacheStore");

                services.AddScoped<ICacheStore, NullStore>();
            }

            return services;


        }

        /// <summary>
        /// Add AddCacheStores to ServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddCacheStores(this IServiceCollection services, Action<CacheStoreOptions> builder)
        {

            var options = new CacheStoreOptions();

            builder?.Invoke(options);

            return AddCacheStores(services, Options.Create(options));

        }

        /// <summary>
        /// Deregister previusly registered services for
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection Remove<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.Where(descriptor => descriptor.ServiceType == typeof(T)).ToList();

            foreach (var descriptor in serviceDescriptor)
            {
                if (descriptor != null)
                    services.Remove(descriptor);
            }

            return services;
        }
    }
}
