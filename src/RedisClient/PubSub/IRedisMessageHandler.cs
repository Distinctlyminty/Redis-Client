using System.Threading.Tasks;

namespace RedisClient.PubSub
{
    public interface IRedisMessageHandler
    {
        string Channel { get; }

        Task HandleAsync(string message);
    }
}