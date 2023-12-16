using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreManagementAPI.Configs;
using StoreManagementAPI.Models;

namespace StoreManagementAPI.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(IOptions<StoreManagementDBSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var database = client.GetDatabase(dbSettings.Value.DatabaseName);

            _products = database.GetCollection<Product>(dbSettings.Value.ProductsCollectionName);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _products.Find(product => true).ToListAsync();
        }
    }
}
