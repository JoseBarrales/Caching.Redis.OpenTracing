namespace Caching.Redis.OpenTracing
{
    /// <summary>
    /// Options for CacheStore
    /// </summary>
    public class CacheStoreOptions
    {
        /// <summary>
        /// Sets cache expire in minutes, defaults to 15
        /// </summary>
        public int CacheExpire { get; set; } = 15;

        /// <summary>
        /// It's recommended to use twemproxy instead of single redis instance
        /// </summary>
        public string RedisHost { get; set; } = string.Empty;
        
        /// <summary>
        /// Prefix used to store keys
        /// </summary>
        public string RedisPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Cache storage class like redis, memory or none, default to redis
        /// </summary>
        public string CacheStorage { get; set; } = "redis";

        /// <summary>
        /// Specify the number of threads for parallel get/set, defaults to 4
        /// </summary>
        public int WithDegreeOfParallelism { get; set; } = 4;

        /// <summary>
        /// How many connections should be open against redis/twemproxy
        /// </summary>
        public int RedisMaxConnections { get; set; } = 30;

        /// <summary>
        /// Use to force recreate cache, overrides the data stored using the same key
        /// </summary>
        public int CacheOverride { get; set; } = 1;

        /// <summary>
        /// Specifies whether throw exceptions or not
        /// </summary>
        public bool RaiseException { get; set; } = false;

        /// <summary>
        /// Sets default serializer to Protocol buffers
        /// </summary>
        public CacheStoreSerializer Serializer { get; set; } = CacheStoreSerializer.MessagePack;
    }

    public enum CacheStoreSerializer
    {
         Protobuf = 1,
         MessagePack = 2
    }
}
