using System;
using Microsoft.Extensions.DependencyInjection;

namespace RedisClient.PubSub
{
    public static class RedisPubSubServiceCollectionExtensions
    {
        public static void AddRedisPubSubService(this IServiceCollection services, RedisServiceOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException(nameof(options.ConnectionString),
                    "Set RedisServiceOptions.ConnectionString");

            if (string.IsNullOrWhiteSpace(options.ServiceName))
                throw new ArgumentNullException(nameof(options.ServiceName));

            services.AddSingleton<IRedisPubSubService, RedisPubSubService>(ctx =>
            {
                RedisStore.Init(options);
                return new RedisPubSubService();
            });
        }
    }
}