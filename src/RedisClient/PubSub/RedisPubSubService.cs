using System;
using StackExchange.Redis;

namespace RedisClient.PubSub
{
    public class RedisPubSubService : IRedisPubSubService
    {
        public void Subscribe(string channel, Action<RedisChannel, RedisValue> action)
        {
            var sub = RedisStore.Cache.Multiplexer.GetSubscriber();

            sub.Subscribe(channel, action);
        }

        public long Publish(string channel, string message)
        {
            var pub = RedisStore.Cache.Multiplexer.GetSubscriber();

            var count = pub.Publish(channel, message);
            return count;
        }
    }
}