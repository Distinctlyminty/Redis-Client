using System;
using System.Threading.Tasks;

namespace RedisClient.Cache
{
    public interface IRedisCacheService
    {
        Task<bool> HasKeyAsync(string key);

        Task<bool> SetAsync(string key, string value);

        Task<bool> SetAsync(string key, string value, DateTimeOffset expiresAt);

        Task<bool> SetAsync(string key, string value, TimeSpan duration);

        Task<bool> SetAsync<T>(string key, T value) where T : class;

        Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt) where T : class;

        Task<bool> SetAsync<T>(string key, T value, TimeSpan duration) where T : class;

        Task<string> GetAsync(string key);

        Task<T> GetAsync<T>(string key) where T : class;

        Task<bool> DeleteKeyAsync(string key);

        Task<bool> HasKeyAsync(string serviceName, string key);

        Task<bool> SetAsync(string serviceName, string key, string value);

        Task<bool> SetAsync(string serviceName, string key, string value, DateTimeOffset expiresAt);

        Task<bool> SetAsync(string serviceName, string key, string value, TimeSpan duration);

        Task<bool> SetAsync<T>(string serviceName, string key, T value) where T : class;

        Task<bool> SetAsync<T>(string serviceName, string key, T value, DateTimeOffset expiresAt) where T : class;

        Task<bool> SetAsync<T>(string serviceName, string key, T value, TimeSpan duration) where T : class;

        Task<string> GetAsync(string serviceName, string key);

        Task<T> GetAsync<T>(string serviceName, string key) where T : class;

        Task<bool> DeleteKeyAsync(string serviceName, string key);

        bool HasKeyStartingWith(string serviceName, string startWith);
    }
}