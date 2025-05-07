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
        
        /*public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        protected override void OnModelCreating (ModelBuilder modelBuilder){
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId);
            modelBuilder.Seed();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }*/
    }
}