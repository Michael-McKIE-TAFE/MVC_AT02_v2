namespace McKIESales.API.Models {
    public class MongoDBSettings {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductCollectionName { get; set; } = null!;
        public string CategoriesCollectionName { get; set; } = null!;
    }
}