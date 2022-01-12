using System;
using StackExchange.Redis;

namespace RedisClient
{
    public class RedisStore
    {
        private static Lazy<ConnectionMultiplexer> LazyConnection;

        private static int _dbId;

        public static ConnectionMultiplexer Connection => LazyConnection.Value;

        public static IDatabase Cache => Connection.GetDatabase(_dbId);

        public static void Init(RedisServiceOptions redisServiceOptions)
        {
            if (LazyConnection == null)
            {
                var options = ConfigurationOptions.Parse(redisServiceOptions.ConnectionString);
                LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));

                _dbId = redisServiceOptions.DbId;
            }
        }
    }
}