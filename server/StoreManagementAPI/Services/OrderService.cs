using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreManagementAPI.Configs;
using StoreManagementAPI.Models;

namespace StoreManagementAPI.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;

        public OrderService(IOptions<StoreManagementDBSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var database = client.GetDatabase(dbSettings.Value.DatabaseName);

            _orders = database.GetCollection<Order>(dbSettings.Value.OrdersCollectionName);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        public async Task<List<Order>> GetByCustomerId(string id)
        {
            return await _orders.Find(order => order.Customer.CustId == id).ToListAsync();
        }
    }
}
