using StoreManagementAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using StoreManagementAPI.Configs;

namespace StoreManagementAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly string _defaultAvatarURL;

        public UserService(IOptions<StoreManagementDBSettings> dbSettings, IOptions<AppSettings> appSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var database = client.GetDatabase(dbSettings.Value.DatabaseName);

            _users = database.GetCollection<User>(dbSettings.Value.UsersCollectionName);
            _defaultAvatarURL = appSettings.Value.DefaultAvatarURL;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<List<User>> SearchUser(string search)
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

        public async Task<bool> AddUser(User user)
        {
            if(await GetByEmail(user.Email) != null)
                return false;

            user.Id = ObjectId.GenerateNewId().ToString();
            user.Avatar = _defaultAvatarURL;

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

        public bool IsValidRole(string role)
        {
            return Enum.TryParse(role, out Role result);
        }

    }
}
