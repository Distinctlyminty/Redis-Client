using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisClient.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly string _serviceName;
        private readonly TelemetryClient _telemetryClient;

        public RedisCacheService(RedisServiceOptions options, TelemetryClient telemetryClient)
        {
            _serviceName = options.ServiceName;
            _telemetryClient = telemetryClient;
        }

        public bool HasKeyStartingWith(string serviceName, string startWith)
        {
            try
            {
                // somehow the "KEYS" method isn't exposed on database but on individual servers
                // so we enumerate every server (there is only one for now)
                // and check if there is a key starting with the pattern
                var endpoints = RedisStore.Connection.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = RedisStore.Connection.GetServer(endpoint);
                    // database = 0, this is the default db, we don't use others
                    // pageSize = 1 because we only need one result
                    var redisValue = RedisValue.Unbox($"{serviceName}-{startWith}*");
                    var keys = server.Keys(0, redisValue, 1);
                    if (keys != null)
                        if (keys.Any())
                            // Only return when a key is found
                            return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                throw;
            }
        }

        public async Task<bool> HasKeyAsync(string key)
        {
            try
            {
                return await RedisStore.Cache.KeyExistsAsync($"{_serviceName}-{key}");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            try
            {
                return await RedisStore.Cache.StringSetAsync($"{_serviceName}-{key}", value);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync(string key, string value, DateTimeOffset expiresAt)
        {
            try
            {
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                return await SetAsync(key, value, expiration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan duration)
        {
            try
            {
                return await RedisStore.Cache.StringSetAsync($"{_serviceName}-{key}", value, duration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value) where T : class
        {
            try
            {
                var serializedObject = Serialize(value);

                return await RedisStore.Cache.StringSetAsync($"{_serviceName}-{key}", serializedObject);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt) where T : class
        {
            try
            {
                var serializedObject = Serialize(value);
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                return await SetAsync(key, serializedObject, expiration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan duration) where T : class
        {
            try
            {
                var serializedObject = Serialize(value);

                return await RedisStore.Cache.StringSetAsync($"{_serviceName}-{key}", serializedObject, duration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        public async Task<string> GetAsync(string key)
        {
            try
            {
                var result = await RedisStore.Cache.StringGetAsync($"{_serviceName}-{key}");

                if (result.HasValue)
                    return result.ToString();
                return null;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return null;
            }
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                var result = await RedisStore.Cache.StringGetAsync($"{_serviceName}-{key}");

                if (result.HasValue)
                {
                    var DeserializedObject = Deserialize<T>(result);

                    return DeserializedObject;
                }

                return null;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return null;
            }
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            try
            {
                return await RedisStore.Cache.KeyDeleteAsync($"{_serviceName}-{key}");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> HasKeyAsync(string serviceName, string key)
        {
            try
            {
                return await RedisStore.Cache.KeyExistsAsync($"{serviceName}-{key}");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string serviceName, string key, string value)
        {
            try
            {
                return await RedisStore.Cache.StringSetAsync($"{serviceName}-{key}", value);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string serviceName, string key, string value, DateTimeOffset expiresAt)
        {
            try
            {
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                return await SetAsync(serviceName, key, value, expiration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string serviceName, string key, string value, TimeSpan duration)
        {
            try
            {
                return await RedisStore.Cache.StringSetAsync($"{serviceName}-{key}", value, duration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string serviceName, string key, T value) where T : class
        {
            try
            {
                var serializedObject = Serialize(value);

                return await RedisStore.Cache.StringSetAsync($"{serviceName}-{key}", serializedObject);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string serviceName, string key, T value, DateTimeOffset expiresAt)
            where T : class
        {
            try
            {
                var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                return await SetAsync(serviceName, key, value, expiration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string serviceName, string key, T value, TimeSpan duration) where T : class
        {
            try
            {
                var serializedObject = Serialize(value);

                return await RedisStore.Cache.StringSetAsync($"{serviceName}-{key}", serializedObject, duration);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string serviceName, string key)
        {
            try
            {
                var result = await RedisStore.Cache.StringGetAsync($"{serviceName}-{key}");

                if (result.HasValue)
                    return result.ToString();
                return null;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string serviceName, string key) where T : class
        {
            try
            {
                var result = await RedisStore.Cache.StringGetAsync($"{serviceName}-{key}");

                if (result.HasValue)
                {
                    var DeserializedObject = Deserialize<T>(result);

                    return DeserializedObject;
                }

                return null;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        ///     Do not use...
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteKeyAsync(string serviceName, string key)
        {
            try
            {
                return await RedisStore.Cache.KeyDeleteAsync($"{serviceName}-{key}");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return false;
            }
        }

        protected string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        protected T Deserialize<T>(string serializedData) where T : class
        {
            if (serializedData == null) return null;

            return JsonConvert.DeserializeObject<T>(serializedData);
        }
    }
}