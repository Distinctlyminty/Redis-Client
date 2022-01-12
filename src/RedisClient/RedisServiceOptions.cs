namespace RedisClient
{
    public class RedisServiceOptions
    {
        public string ServiceName { get; set; }

        public string ConnectionString { get; set; }

        public int DbId { get; set; } = -1;
    }
}