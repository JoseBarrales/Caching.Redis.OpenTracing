using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using System.Collections.Concurrent;
using System.Linq;

namespace Caching.Redis.OpenTracing.Multiplexer
{
    public class RedisCacheConnectionPoolManager : IRedisCacheConnectionPoolManager
    {
        private static ConcurrentBag<ConnectionMultiplexer> connections;
        private readonly RedisConfiguration redisConfiguration;
        private readonly IOptions<CacheStoreOptions> options;

        public RedisCacheConnectionPoolManager(RedisConfiguration redisConfiguration, IOptions<CacheStoreOptions> options)
        {
            this.redisConfiguration = redisConfiguration;
            this.options = options;

            Initialize();
        }

        public void Dispose()
        {
            var activeConnections = connections.ToList();

            foreach (var connection in activeConnections)
            {
                connection.Dispose();
            }

            Initialize();
        }

        public IConnectionMultiplexer GetConnection()
        {
            var response = default(ConnectionMultiplexer);

            var loadedLazys = connections;

            if (loadedLazys.Count() == connections.Count)
            {
                response = connections.OrderBy(x => x.GetCounters().TotalOutstanding).First();
            }
            else
            {
                response = connections.First();
            }

            return response;
        }

        private void Initialize()
        {
            connections = new ConcurrentBag<ConnectionMultiplexer>();

            for (int i = 0; i < options.Value.RedisMaxConnections; i++)
            {
                connections.Add(ConnectionMultiplexer.Connect(redisConfiguration.ConfigurationOptions));
            }
        }

        public static RedisConfiguration GetRedisConfiguration(string host)
        {
            var config = new RedisConfiguration()
            {
                Hosts = new RedisHost[]
                {
                    new RedisHost() { Host = host, Port = 6379 }
                },
                ExcludeCommands = new[] {
                    "AUTH", "ECHO", "SELECT", "BGREWRITEAOF", "BGSAVE",
                    "CLIENTKILL", "CLIENTLIST", "CONFIGGET", "CONFIGSET",
                    "CONFIGRESETSTAT", "DBSIZE", "DEBUGOBJECT", "DEBUGSEGFAULT",
                    "FLUSHALL", "FLUSHDB", "INFO", "LASTSAVE", "MONITOR", "SAVE",
                    "SHUTDOWN", "SLAVEOF", "SLOWLOG", "SYNC", "TIME", "SUBSCRIBE", "CLIENT"
                },

            };

            config.ConfigurationOptions.Proxy = Proxy.Twemproxy;

            return config;
        }
    }
}
