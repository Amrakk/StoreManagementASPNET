using StoreManagementAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace StoreManagementAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<StoreManagementDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _users = database.GetCollection<User>(settings.Value.UsersCollectionName);
        }

        public async Task<List<User>> Get()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<List<User>> Search(string search)
        {
            var filter = Builders<User>.Filter.Regex("Username", new BsonRegularExpression($".*{search}.*", "i"));
            return await _users.Find(filter).ToListAsync();
        }
            

        public async Task<User> GetById(string id) =>
            await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByEmail(string email) =>
            await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        public async Task<User> GetByUsername(string username) =>
            await _users.Find(user => user.Username == username).FirstOrDefaultAsync();

        public async Task<bool> Create(User user)
        {
            if(await GetByEmail(user.Email) != null)
                return false;

            await _users.InsertOneAsync(user);
            return true;
        }

        public async Task<bool> Update(string id, User userIn)
        {
            if(await GetById(id) == null)
                return false;

            await _users.ReplaceOneAsync(user => user.Id == id, userIn);
            return true;
        }


        public async Task<bool> Remove(string id)
        {
            if(await GetById(id) == null)
                return false;

            await _users.DeleteOneAsync(user => user.Id == id);
            return true;
        }


    }
}
