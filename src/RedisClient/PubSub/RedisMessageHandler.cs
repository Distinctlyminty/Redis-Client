using System;
using System.Threading.Tasks;

namespace RedisClient.PubSub
{
    public class RedisMessageHandler : IRedisMessageHandler
    {
        private readonly IRedisPubSubService _redisPubSubService;

        public RedisMessageHandler(IRedisPubSubService redisPubSubService)
        {
            _redisPubSubService = redisPubSubService;
        }

        public string Channel => throw new NotImplementedException();

        public Task HandleAsync(string message)
        {
            _redisPubSubService.Subscribe("", (channel, value) => { });

            return Task.FromResult(true);
        }
    }
}