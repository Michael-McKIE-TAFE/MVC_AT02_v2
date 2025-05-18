namespace McKIESales.API.Models {
    /// <summary>
    /// This class holds the configuration settings for connecting to MongoDB, 
    /// including the `ConnectionString`, the `DatabaseName`, and the names of 
    /// the collections for products (`ProductCollectionName`) and categories 
    /// (`CategoriesCollectionName`). These settings are typically injected into 
    /// the application via dependency injection for use in the `ShopContext`.
    /// </summary>
    public class MongoDBSettings {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string ProductCollectionName { get; set; } = null!;
        public string CategoriesCollectionName { get; set; } = null!;
    }
}