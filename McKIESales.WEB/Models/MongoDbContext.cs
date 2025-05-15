using McKIESales.WEB.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace McKIESales.WEB.Models {
    public class MongoDbContext {
        private readonly IMongoDatabase _database;

        public MongoDbContext (IOptions<MongoDbSettings> settings){
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}