using System;
using StackExchange.Redis;

namespace RedisClient.PubSub
{
    public interface IRedisPubSubService
    {
        void Subscribe(string channel, Action<RedisChannel, RedisValue> action);
        long Publish(string channel, string message);
    }
}