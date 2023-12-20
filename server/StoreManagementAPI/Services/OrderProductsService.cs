using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using StoreManagementAPI.Configs;
using StoreManagementAPI.Models;

namespace StoreManagementAPI.Services
{
    public class OrderProductsService
    {
        private readonly IMongoCollection<OrderProduct> _orderProducts;

        public OrderProductsService(IOptions<StoreManagementDBSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var database = client.GetDatabase(dbSettings.Value.DatabaseName);

            _orderProducts = database.GetCollection<OrderProduct>(dbSettings.Value.OrderProductsCollectionName);
        }

        public async Task<List<OrderProduct>> GetAllOrderProducts()
        {
            return await _orderProducts.Find(product => true).ToListAsync();
        }

        public async Task<List<OrderProduct>> GetByProductId(string id)
        {
            var filter = Builders<OrderProduct>.Filter.Regex("pid", new BsonRegularExpression(id));
            return await _orderProducts.Find(filter).ToListAsync();
        }
    }
}
