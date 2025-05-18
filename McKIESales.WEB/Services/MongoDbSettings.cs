namespace McKIESales.WEB.Services {
    /// <summary>
    /// This class stores the connection string and database name for MongoDB. 
    /// </summary>
    public class MongoDbSettings {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}