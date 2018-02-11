using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CartApi.Model
{
    public class RedisCartRepository : ICartRepository
    {
        private readonly ILogger<RedisCartRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCartRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis, IDatabase database)
        {
            _logger = loggerFactory.CreateLogger<RedisCartRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<Cart> GetCartAsync(string cartId)
        {
            var data = await _database.StringGetAsync(cartId);
            return !data.IsNullOrEmpty 
                ? JsonConvert.DeserializeObject<Cart>(data)
                : null;
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();
            return data?.Select(k => k.ToString());
        }

        public async Task<Cart> UpdateCartAsync(Cart basket)
        {
            var data = JsonConvert.SerializeObject(basket);
            var created = await _database.StringSetAsync(basket.BuyerId, data);
            if (!created)
            {
                _logger.LogInformation($"Problem occour persisting the item with key {basket.BuyerId}");
                return null;
            }

            _logger.LogInformation("Basket item persisted successfully");
            return await GetCartAsync(basket.BuyerId);
        }

        public async Task<bool> DeleteCartAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}
