using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace McKIESales.API.Models {
    public class ShopContext {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _settings;

        public ShopContext (IOptions<MongoDBSettings> options, IMongoClient client){
            _settings = options.Value;
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>(_settings.ProductCollectionName);
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>(_settings.CategoriesCollectionName);
    }
}