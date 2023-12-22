using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreManagementAPI.Configs;
using StoreManagementAPI.Models;
using System.Security.Cryptography;

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

        public Product GetProductByPID(string pid)
        {
            return _products.Find(p => p.Pid.Equals(pid)).FirstOrDefault();
        }

        public Product GetProductByName(string productName)
        {
            return _products.Find(p => p.Name.Equals(productName)).FirstOrDefault();
        }

        public Product GetProductByBarcode(string barcode)
        {
            return _products.Find(p => p.Barcode.Equals(barcode)).FirstOrDefault();
        }
    }
}
