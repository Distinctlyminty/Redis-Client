using System;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;

namespace RedisClient.Cache
{
    public static class RedisCacheServiceCollectionExtensions
    {
        public static void AddRedisCacheService(this IServiceCollection services, RedisServiceOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (options == null) throw new ArgumentNullException(nameof(options));

            var serviceProvider = services.BuildServiceProvider();
            var telemetryClient = serviceProvider.GetService<TelemetryClient>();


            services.AddSingleton<IRedisCacheService, RedisCacheService>(ctx =>
            {
                RedisStore.Init(options);
                return new RedisCacheService(options, telemetryClient);
            });
        }
    }
}